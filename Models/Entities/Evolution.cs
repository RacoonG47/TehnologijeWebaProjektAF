using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class Evolution
    {
        public int Id { get; set; }
        public int FromSpeciesId { get; set; }
        public PokemonSpecies FromSpecies { get; set; } = null!;

        public int ToSpeciesId { get; set; }
        public PokemonSpecies ToSpecies { get; set; } = null!;
        public int RequiredLevel { get; set; }
    }
}
