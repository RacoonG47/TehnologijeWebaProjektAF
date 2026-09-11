using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class Move
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonType Type { get; set; }
        public MoveCategory Category { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public int PP { get; set; }
        public EffectType EffectType { get; set; }
        public BattleStat? TargetStat { get; set; }
        public int? StageChange { get; set; }
        public StatusCondition? ApplyStatus { get; set; }
        public int? SecondaryEffectChance { get; set; }
    }
}