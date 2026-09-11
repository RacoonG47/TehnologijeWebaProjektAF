using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class AbilityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AbilityTrigger Trigger { get; set; }
        public EffectType? EffectType { get; set; }
        public BattleStat? TargetStat { get; set; }
        public int? StageChange { get; set; }
        public StatusCondition? StatusInteraction { get; set; }

        public static AbilityDto FromEntity(Ability a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            Trigger = a.Trigger,
            EffectType = a.EffectType,
            TargetStat = a.TargetStat,
            StageChange = a.StageChange,
            StatusInteraction = a.StatusInteraction
        };
    }
}