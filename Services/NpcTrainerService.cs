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
    public class NpcTrainerService : INpcTrainerService
    {
        private readonly IRepository<OwnedPokemon> _ownedRepo;
        private readonly IRepository<PokemonSpecies> _speciesRepo;
        private readonly IRepository<Evolution> _evolutionRepo;
        private readonly ApplicationDbContext _context;
        private readonly Random _rng = new();

        public NpcTrainerService(IRepository<OwnedPokemon> ownedRepo, IRepository<PokemonSpecies> speciesRepo, IRepository<Evolution> evolutionRepo, ApplicationDbContext context)
        {
            _ownedRepo = ownedRepo;
            _speciesRepo = speciesRepo;
            _evolutionRepo = evolutionRepo;
            _context = context;
        }

        public async Task<NpcTrainerDto> GenerateNpcTrainerAsync(string userId, string archetype)
        {
            var userTeam = await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam));
            if (!userTeam.Any()) throw new Exception("You need a team to battle a trainer.");

            int lowestLevel = userTeam.Min(p => p.Level);
            int teamSize = userTeam.Count();
            var allEvolutions = (await _evolutionRepo.GetAllAsync()).ToList();
            var npcTeam = new List<NpcPokemonDto>();

            var archetypeTypes = new Dictionary<string, PokemonType?>(StringComparer.OrdinalIgnoreCase)
            {
                { "Bug Catcher", PokemonType.Bug }, { "Fire Tamer", PokemonType.Fire }, { "Swimmer", PokemonType.Water },
                { "Dragon Tamer", PokemonType.Dragon }, { "Youngster", PokemonType.Normal }, { "Lass", PokemonType.Grass },
                { "Psychic", PokemonType.Psychic }, { "Hiker", PokemonType.Rock }, { "Bird Keeper", PokemonType.Flying },
                { "Super Nerd", PokemonType.Electric }, { "Blackbelt", PokemonType.Fighting }, { "Team Rocket Grunt", PokemonType.Poison }
            };
            archetypeTypes.TryGetValue(archetype, out var requiredType);

            for (int i = 0; i < teamSize; i++)
            {
                var roll = _rng.Next(1, 101);
                Rarity selectedRarity = roll <= 50 ? Rarity.Common : roll <= 75 ? Rarity.Uncommon : roll <= 90 ? Rarity.Rare : roll <= 97 ? Rarity.PseudoLegendary : roll <= 99 ? Rarity.Mythical : Rarity.Legendary;

                var evolvedIds = allEvolutions.Select(e => e.ToSpeciesId).ToHashSet();
                var basePool = (await _speciesRepo.GetAllAsync()).Where(s => s.Rarity == selectedRarity && !evolvedIds.Contains(s.Id)).ToList();

                if (requiredType.HasValue)
                    basePool = basePool.Where(s => s.PrimaryType == requiredType.Value || s.SecondaryType == requiredType.Value).ToList();

                if (!basePool.Any()) continue;

                var chosenBase = basePool[_rng.Next(basePool.Count)];
                var finalSpecies = SimulateEvolutionForNpc(chosenBase, lowestLevel, allEvolutions);
                var gender = finalSpecies.BaseGender == Gender.Genderless ? Gender.Genderless : (_rng.Next(0, 2) == 0 ? Gender.Male : Gender.Female);
                int maxHp = ((2 * finalSpecies.BaseHp * lowestLevel) / 100) + lowestLevel + 10;

                var moves = finalSpecies.Moves
                    .OrderBy(x => _rng.Next())
                    .Take(_rng.Next(1, 5))
                    .Select(MoveDto.FromEntity)
                    .ToList();

                AbilityDto? ability = finalSpecies.Abilities.Any()
                    ? AbilityDto.FromEntity(finalSpecies.Abilities.ElementAt(_rng.Next(finalSpecies.Abilities.Count())))
                    : null;

                npcTeam.Add(new NpcPokemonDto
                {
                    SpeciesId = finalSpecies.Id,
                    Name = finalSpecies.Name,
                    Level = lowestLevel,
                    Gender = gender,
                    MaxHp = maxHp,
                    CurrentHp = maxHp,
                    PrimaryType = finalSpecies.PrimaryType,
                    SecondaryType = finalSpecies.SecondaryType,
                    Moves = moves,
                    Ability = ability,
                    BaseAttack = finalSpecies.BaseAttack,
                    BaseDefense = finalSpecies.BaseDefense,
                    BaseSpecialAttack = finalSpecies.BaseSpecialAttack,
                    BaseSpecialDefense = finalSpecies.BaseSpecialDefense,
                    BaseSpeed = finalSpecies.BaseSpeed
                });
            }

            return new NpcTrainerDto { ArchetypeName = archetype, Team = npcTeam };
        }

        public async Task HandleVictoryAsync(string userId)
        {
            var userTeam = await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam));
            if (!userTeam.Any()) throw new Exception("No team found.");

            foreach (var pokemon in userTeam)
            {
                if (pokemon.Level < 100)
                {
                    pokemon.Level += 1;
                    _ownedRepo.Update(pokemon);
                }
            }
            await _context.SaveChangesAsync();
        }

        private PokemonSpecies SimulateEvolutionForNpc(PokemonSpecies currentSpecies, int targetLevel, List<Evolution> allEvolutions)
        {
            var possibleEvo = allEvolutions.FirstOrDefault(e => e.FromSpeciesId == currentSpecies.Id && e.RequiredLevel <= targetLevel && e.RequiredLevel > 1);
            if (possibleEvo != null)
            {
                var nextStage = _context.Set<PokemonSpecies>().FirstOrDefault(s => s.Id == possibleEvo.ToSpeciesId);
                if (nextStage != null) return SimulateEvolutionForNpc(nextStage, targetLevel, allEvolutions);
            }
            return currentSpecies;
        }
    }
}