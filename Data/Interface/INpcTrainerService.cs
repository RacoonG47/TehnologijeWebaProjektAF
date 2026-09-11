using System.Threading.Tasks;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface INpcTrainerService
    {
        Task<NpcTrainerDto> GenerateNpcTrainerAsync(string userId, string archetype);
        Task HandleVictoryAsync(string userId);
    }
}