using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class OwnedPokemon
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int SpeciesId { get; set; }
        public PokemonSpecies Species { get; set; } = null!;

        public string Nickname { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public Gender Gender { get; set; }
        public bool IsInTeam { get; set; } = false;

       public ICollection<Move> Moves { get; set; } = new List<Move>();
        public int? AbilityId { get; set; }
        public Ability? Ability { get; set; }

        public int? HeldItemId { get; set; }
        public HeldItem? HeldItem { get; set; }
    }
}
