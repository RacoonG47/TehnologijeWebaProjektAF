using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class Ability
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AbilityTrigger Trigger { get; set; }
        public EffectType? EffectType { get; set; }
        public BattleStat? TargetStat { get; set; }
        public int? StageChange { get; set; }
        public StatusCondition? StatusInteraction { get; set; }
    }
}