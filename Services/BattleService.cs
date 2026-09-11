using System;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Services
{
    public class BattleService : IBattleService
    {
        public DamageResultDto CalculateDamage(CalculateDamageDto dto)
        {
            double atkMultiplier = dto.AttackerStage > 0 ? (2.0 + dto.AttackerStage) / 2.0 : dto.AttackerStage < 0 ? 2.0 / (2.0 - dto.AttackerStage) : 1.0;
            double defMultiplier = dto.DefenderStage > 0 ? (2.0 + dto.DefenderStage) / 2.0 : dto.DefenderStage < 0 ? 2.0 / (2.0 - dto.DefenderStage) : 1.0;

            int finalAtk = (int)(((2 * dto.AttackerAttackStat * dto.AttackerLevel) / 100 + 5) * atkMultiplier);
            int finalDef = (int)(((2 * dto.DefenderDefenseStat * dto.AttackerLevel) / 100 + 5) * defMultiplier);

            if (dto.AttackerStatus == StatusCondition.Burn && dto.MoveCategory == MoveCategory.Physical)
            {
                finalAtk = dto.AttackerHasGuts ? (int)(finalAtk * 1.5) : (int)(finalAtk * 0.5);
            }

            float baseDamage = ((2f * dto.AttackerLevel / 5f + 2) * dto.MovePower * finalAtk / (float)finalDef) / 50f + 2f;

            float stab = (dto.MoveType == dto.AttackerPrimaryType || dto.MoveType == dto.AttackerSecondaryType) ? 1.5f : 1f;
            float effectiveness = TypeChart.GetEffectiveness(dto.MoveType, dto.DefenderPrimaryType, dto.DefenderSecondaryType);

            System.Diagnostics.Debug.WriteLine($"[DAMAGE] Move:{dto.MoveType} vs {dto.DefenderPrimaryType}/{dto.DefenderSecondaryType} = Eff:{effectiveness}");

            int finalDamage = (int)(baseDamage * stab * effectiveness);
            if (finalDamage < 1 && effectiveness > 0f) finalDamage = 1;

            return new DamageResultDto { Damage = finalDamage, Effectiveness = effectiveness };
        }
    }
}