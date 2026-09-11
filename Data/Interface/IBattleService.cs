using System.Threading.Tasks;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface IBattleService
    {
        DamageResultDto CalculateDamage(CalculateDamageDto dto);
    }
}