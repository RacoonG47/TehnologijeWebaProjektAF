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
    public class EvolutionService : IEvolutionService
    {
        private readonly IRepository<OwnedPokemon> _ownedRepo;
        private readonly IRepository<PokemonSpecies> _speciesRepo;
        private readonly ApplicationDbContext _context;

        public EvolutionService(IRepository<OwnedPokemon> ownedRepo, IRepository<PokemonSpecies> speciesRepo, ApplicationDbContext context)
        {
            _ownedRepo = ownedRepo;
            _speciesRepo = speciesRepo;
            _context = context;
        }

        public async Task<EvolutionCheckDto> CheckEvolutionAsync(string userId, int ownedPokemonId)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId, q => q.Include(p => p.Species).ThenInclude(s => s.Evolutions).ThenInclude(e => e.ToSpecies));
            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            var availableEvolutions = pokemon.Species.Evolutions.Where(e =>
                e.RequiredLevel == 1 ||
                pokemon.Level >= e.RequiredLevel
            ).ToList();

            return new EvolutionCheckDto
            {
                CanEvolve = availableEvolutions.Any(),
                Options = availableEvolutions.Select(e => new EvolutionOptionDto
                {
                    TargetSpeciesId = e.ToSpeciesId,
                    TargetName = e.ToSpecies.Name,
                    RequiredLevel = e.RequiredLevel
                }).ToList()
            };
        }

        public async Task EvolvePokemonAsync(string userId, int ownedPokemonId, int targetSpeciesId)
        {
            var pokemon = await _ownedRepo.GetByIdAsync(ownedPokemonId, q => q.Include(p => p.Species).ThenInclude(s => s.Evolutions));
            if (pokemon == null || pokemon.UserId != userId) throw new Exception("Pokemon not found.");

            var evolution = pokemon.Species.Evolutions.FirstOrDefault(e => e.ToSpeciesId == targetSpeciesId);
            if (evolution == null) throw new Exception("Invalid evolution path for this Pokemon.");

            if (evolution.RequiredLevel != 1 && pokemon.Level < evolution.RequiredLevel)
                throw new Exception("Pokemon is not high enough level to evolve this way.");

            // Get the new species name
            var newSpecies = await _speciesRepo.GetByIdAsync(targetSpeciesId);
            if (newSpecies == null) throw new Exception("Target species not found.");

            pokemon.SpeciesId = targetSpeciesId;
            pokemon.Nickname = newSpecies.Name; // Update nickname to new evolution name

            _ownedRepo.Update(pokemon);
            await _context.SaveChangesAsync();
        }
    }
}