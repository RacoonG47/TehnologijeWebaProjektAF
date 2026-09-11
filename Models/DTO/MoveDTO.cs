using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class MoveDto
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

        public static MoveDto FromEntity(Move m) => new()
        {
            Id = m.Id,
            Name = m.Name,
            Type = m.Type,
            Category = m.Category,
            Power = m.Power,
            Accuracy = m.Accuracy,
            PP = m.PP,
            EffectType = m.EffectType,
            TargetStat = m.TargetStat,
            StageChange = m.StageChange,
            ApplyStatus = m.ApplyStatus,
            SecondaryEffectChance = m.SecondaryEffectChance
        };
    }
}