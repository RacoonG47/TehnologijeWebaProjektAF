using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;


namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class PokemonSpecies
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonType PrimaryType { get; set; }
        public PokemonType? SecondaryType { get; set; }
        public Rarity Rarity { get; set; }

        public int BaseHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }

        public Gender BaseGender { get; set; } 
        public ICollection<Move> Moves { get; set; } = new List<Move>();
        public ICollection<Ability> Abilities { get; set; } = new List<Ability>();
        public ICollection<Evolution> Evolutions { get; set; } = new List<Evolution>();
        public ICollection<OwnedPokemon> OwnedPokemon { get; set; } = new List<OwnedPokemon>();
    }
}
