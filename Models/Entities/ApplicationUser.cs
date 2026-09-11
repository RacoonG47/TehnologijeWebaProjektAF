using Microsoft.AspNetCore.Identity;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<OwnedPokemon> OwnedPokemon { get; set; } = new List<OwnedPokemon>();

        public ICollection<BattleLog> BattleLogs { get; set; } = new List<BattleLog>();
    }
}
