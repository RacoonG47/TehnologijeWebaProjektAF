using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Services
{
    public class CatchService : ICatchService
    {
        private readonly IRepository<PokemonSpecies> _speciesRepo;
        private readonly IRepository<Evolution> _evolutionRepo;
        private readonly IRepository<OwnedPokemon> _ownedRepo;
        private readonly ApplicationDbContext _context;
        private readonly Random _rng = new();

        public CatchService(IRepository<PokemonSpecies> speciesRepo, IRepository<Evolution> evolutionRepo, IRepository<OwnedPokemon> ownedRepo, ApplicationDbContext context)
        {
            _speciesRepo = speciesRepo;
            _evolutionRepo = evolutionRepo;
            _ownedRepo = ownedRepo;
            _context = context;
        }

        public async Task<WildEncounterDto> GenerateWildEncounterAsync(string userId, PokemonType type)
        {
            var userTeam = await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam));

            int playerMaxLevel = userTeam.Any() ? Math.Max(1, userTeam.Min(p => p.Level)) : 5;

            var allSpecies = (await _speciesRepo.GetAllAsync(q => q.Include(s => s.Moves).Include(s => s.Abilities))).ToList();
            var typedSpecies = allSpecies.Where(s => s.PrimaryType == type || s.SecondaryType == type).ToList();
            var evolvedIds = (await _evolutionRepo.GetAllAsync()).Select(e => e.ToSpeciesId).ToHashSet();
            var baseSpecies = typedSpecies.Where(s => !evolvedIds.Contains(s.Id)).ToList();

            if (!baseSpecies.Any()) throw new Exception("No pokemon found for this type.");

            var rarityChances = new Dictionary<Rarity, int>
            {
                { Rarity.Common, 50 }, { Rarity.Uncommon, 25 }, { Rarity.Rare, 15 },
                { Rarity.PseudoLegendary, 7 }, { Rarity.Mythical, 2 }, { Rarity.Legendary, 1 }
            };
            var orderedRarities = new[] { Rarity.Legendary, Rarity.Mythical, Rarity.PseudoLegendary, Rarity.Rare, Rarity.Uncommon, Rarity.Common };

            for (int i = 0; i < orderedRarities.Length; i++)
            {
                var currentRarity = orderedRarities[i];
                if (!baseSpecies.Any(s => s.Rarity == currentRarity))
                {
                    int chanceToRedistribute = rarityChances[currentRarity];
                    rarityChances[currentRarity] = 0;
                    if (i > 0)
                    {
                        var nextHighest = orderedRarities[i - 1];
                        rarityChances[nextHighest] += chanceToRedistribute;
                    }
                }
            }

            int roll = _rng.Next(1, 101);
            int cumulative = 0;
            Rarity selectedRarity = Rarity.Common;
            foreach (var rarity in orderedRarities.Reverse())
            {
                cumulative += rarityChances[rarity];
                if (roll <= cumulative) { selectedRarity = rarity; break; }
            }

            var pool = baseSpecies.Where(s => s.Rarity == selectedRarity).ToList();

            if (!pool.Any()) throw new Exception("No pokemon available for this type.");

            var chosenSpecies = pool[_rng.Next(pool.Count)];

            int level = _rng.Next(1, playerMaxLevel + 1);

            Gender gender = chosenSpecies.BaseGender == Gender.Genderless
                ? Gender.Genderless
                : (_rng.Next(0, 2) == 0 ? Gender.Male : Gender.Female);
            int maxHp = ((2 * chosenSpecies.BaseHp * level) / 100) + level + 10;

            var moves = chosenSpecies.Moves.ToList();
            var selectedMoves = moves.Count > 0
                ? moves.OrderBy(x => _rng.Next()).Take(Math.Min(4, moves.Count)).Select(MoveDto.FromEntity).ToList()
                : new List<MoveDto>();

            AbilityDto? ability = null;
            var abilities = chosenSpecies.Abilities.ToList();
            if (abilities.Count > 0)
            {
                ability = AbilityDto.FromEntity(abilities[_rng.Next(abilities.Count)]);
            }

            return new WildEncounterDto
            {
                SpeciesId = chosenSpecies.Id,
                Name = chosenSpecies.Name,
                PrimaryType = chosenSpecies.PrimaryType,
                SecondaryType = chosenSpecies.SecondaryType,
                Level = level,
                Gender = gender,
                MaxHp = maxHp,
                CurrentHp = maxHp,
                Rarity = chosenSpecies.Rarity,
                BaseAttack = chosenSpecies.BaseAttack,
                BaseDefense = chosenSpecies.BaseDefense,
                BaseSpecialAttack = chosenSpecies.BaseSpecialAttack,
                BaseSpecialDefense = chosenSpecies.BaseSpecialDefense,
                BaseSpeed = chosenSpecies.BaseSpeed,
                Moves = selectedMoves,
                Ability = ability
            };
        }

        public async Task<CatchResultDto> AttemptCatchAsync(string userId, int speciesId, int level, int currentHp, int maxHp, Gender gender)
        {
            var species = await _speciesRepo.GetByIdAsync(speciesId);
            if (species == null) return new CatchResultDto { Success = false, Message = "Invalid species." };

            int baseChance = species.Rarity switch
            {
                Rarity.Common => 50,
                Rarity.Uncommon => 40,
                Rarity.Rare => 30,
                Rarity.PseudoLegendary => 20,
                Rarity.Mythical => 15,
                Rarity.Legendary => 10,
                _ => 0
            };
            double hpLostPercent = 1.0 - ((double)currentHp / maxHp);
            int finalCatchChance = baseChance + (int)(hpLostPercent * 100);

            int roll = _rng.Next(1, 101);
            if (roll > finalCatchChance)
                return new CatchResultDto { Success = false, Message = $"Failed to catch {species.Name}! (Roll: {roll}, Chance: {finalCatchChance}%)" };

            var userTeam = (await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam))).ToList();
            bool addToTeam = userTeam.Count < 6;

            var ownedPokemon = new OwnedPokemon
            {
                UserId = userId,
                SpeciesId = speciesId,
                Nickname = species.Name,
                Level = Math.Max(1, level), 
                Gender = gender,
                IsInTeam = addToTeam,
                Moves = new List<Move>(),
                AbilityId = null
            };
            await _ownedRepo.AddAsync(ownedPokemon);
            await _context.SaveChangesAsync();

            return new CatchResultDto
            {
                Success = true,
                Message = addToTeam ? $"Caught {species.Name} and added to team!" : $"Caught {species.Name} and sent to storage!",
                OwnedPokemonId = ownedPokemon.Id
            };
        }
    }
}