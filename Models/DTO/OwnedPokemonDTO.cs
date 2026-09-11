using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class OwnedPokemonDTO
    {
        public int Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string SpeciesName { get; set; } = string.Empty;
        public PokemonType PrimaryType { get; set; }
        public PokemonType? SecondaryType { get; set; }
        public int Level { get; set; }
        public Gender Gender { get; set; }
        public bool IsInTeam { get; set; }
        public List<MoveDto> Moves { get; set; } = new();
        public AbilityDto? Ability { get; set; }
        public string? ItemName { get; set; }

        public int BaseHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
    }
}