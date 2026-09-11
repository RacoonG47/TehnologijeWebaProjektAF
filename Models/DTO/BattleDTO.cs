using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class CalculateDamageDto
    {
        public int AttackerLevel { get; set; }
        public int AttackerAttackStat { get; set; }
        public int DefenderDefenseStat { get; set; }
        public int MovePower { get; set; }
        public PokemonType MoveType { get; set; }
        public MoveCategory MoveCategory { get; set; }
        public PokemonType AttackerPrimaryType { get; set; }
        public PokemonType? AttackerSecondaryType { get; set; }
        public PokemonType DefenderPrimaryType { get; set; }
        public PokemonType? DefenderSecondaryType { get; set; }

        public int AttackerStage { get; set; } = 0;
        public int DefenderStage { get; set; } = 0;
        public StatusCondition AttackerStatus { get; set; } = StatusCondition.None;
        public bool AttackerHasGuts { get; set; } = false;
    }

    public class DamageResultDto
    {
        public int Damage { get; set; }
        public float Effectiveness { get; set; }
    }
}