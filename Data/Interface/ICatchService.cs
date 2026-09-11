using System.Threading.Tasks;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface ICatchService
    {
        Task<WildEncounterDto> GenerateWildEncounterAsync(string userId, PokemonType type);
        Task<CatchResultDto> AttemptCatchAsync(string userId, int speciesId, int level, int currentHp, int maxHp, Gender gender);
    }
}