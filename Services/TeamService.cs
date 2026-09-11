using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Services
{
    public class TeamService : ITeamService
    {
        private readonly IRepository<OwnedPokemon> _ownedRepo;
        private readonly IRepository<HeldItem> _itemRepo;
        private readonly ApplicationDbContext _context;

        public TeamService(IRepository<OwnedPokemon> ownedRepo, IRepository<HeldItem> itemRepo, ApplicationDbContext context)
        {
            _ownedRepo = ownedRepo;
            _itemRepo = itemRepo;
            _context = context;
        }

        public async Task<List<OwnedPokemonDTO>> GetUserTeamAsync(string userId)
        {
            var team = await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam)
                .Include(p => p.Species)
                .Include(p => p.Moves)
                .Include(p => p.Ability)
                .Include(p => p.HeldItem));
            return team.Select(MapToDto).ToList();
        }

        public async Task<List<OwnedPokemonDTO>> GetUserStorageAsync(string userId)
        {
            var storage = await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && !p.IsInTeam)
                .Include(p => p.Species)
                .Include(p => p.Moves)
                .Include(p => p.Ability)
                .Include(p => p.HeldItem));
            return storage.Select(MapToDto).ToList();
        }

        public async Task SwapPokemonAsync(string userId, int ownedPokemonId, bool addToTeam)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId);
            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            if (addToTeam)
            {
                var count = (await _ownedRepo.GetAllAsync(q => q.Where(p => p.UserId == userId && p.IsInTeam))).Count();
                if (count >= 6) throw new Exception("Team is full!");
                pokemon.IsInTeam = true;
            }
            else
            {
                pokemon.IsInTeam = false;
            }

            _ownedRepo.Update(pokemon);
            await _context.SaveChangesAsync();
        }

        public async Task ReleasePokemonAsync(string userId, int ownedPokemonId)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId);
            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            _ownedRepo.Delete(pokemon);
            await _context.SaveChangesAsync();
        }

        public async Task<AvailableOptionsDto> GetAvailableOptionsAsync(string userId, int ownedPokemonId)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId,
                q => q.Include(p => p.Species).ThenInclude(s => s.Moves)
                      .Include(p => p.Species).ThenInclude(s => s.Abilities));

            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            var allItems = (await _itemRepo.GetAllAsync()).ToList();

            return new AvailableOptionsDto
            {
                AvailableMoves = pokemon.Species.Moves.Select(MoveDto.FromEntity).ToList(),
                AvailableAbilities = pokemon.Species.Abilities.Select(AbilityDto.FromEntity).ToList(),
                AllItems = allItems.Select(i => new HeldItemDto { Id = i.Id, Name = i.Name }).ToList()
            };
        }

        public async Task UpdatePokemonSetupAsync(string userId, int ownedPokemonId, UpdateSetupDto dto)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId,
                q => q.Include(p => p.Species).ThenInclude(s => s.Moves)
                      .Include(p => p.Species).ThenInclude(s => s.Abilities));

            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            var selectedMoves = pokemon.Species.Moves.Where(m => dto.SelectedMoveIds.Contains(m.Id)).ToList();

            if (dto.SelectedAbilityId.HasValue)
            {
                if (!pokemon.Species.Abilities.Any(a => a.Id == dto.SelectedAbilityId.Value))
                    throw new Exception("Invalid ability.");
            }

            pokemon.Moves = selectedMoves;
            pokemon.AbilityId = dto.SelectedAbilityId;
            pokemon.HeldItemId = dto.SelectedItemId;

            _ownedRepo.Update(pokemon);
            await _context.SaveChangesAsync();
        }

        private static OwnedPokemonDTO MapToDto(OwnedPokemon p)
        {
            return new OwnedPokemonDTO
            {
                Id = p.Id,
                Nickname = p.Nickname,
                SpeciesName = p.Species.Name,
                PrimaryType = p.Species.PrimaryType,
                SecondaryType = p.Species.SecondaryType,
                Level = p.Level,
                Gender = p.Gender,
                IsInTeam = p.IsInTeam,
                Moves = p.Moves.Select(MoveDto.FromEntity).ToList(),
                Ability = p.Ability != null ? AbilityDto.FromEntity(p.Ability) : null,
                ItemName = p.HeldItem?.Name,
                BaseHp = p.Species.BaseHp,
                BaseAttack = p.Species.BaseAttack,
                BaseDefense = p.Species.BaseDefense,
                BaseSpecialAttack = p.Species.BaseSpecialAttack,
                BaseSpecialDefense = p.Species.BaseSpecialDefense,
                BaseSpeed = p.Species.BaseSpeed
            };
        }
    }
}