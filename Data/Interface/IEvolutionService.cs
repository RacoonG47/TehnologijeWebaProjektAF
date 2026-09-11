using System.Threading.Tasks;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface IEvolutionService
    {
        Task<EvolutionCheckDto> CheckEvolutionAsync(string userId, int ownedPokemonId);
        Task EvolvePokemonAsync(string userId, int ownedPokemonId, int targetSpeciesId);
    }
}