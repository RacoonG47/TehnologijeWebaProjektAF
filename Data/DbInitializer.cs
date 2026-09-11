using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data
{
    internal record SeedDataDto(List<SeedAbilityDto> Abilities, List<SeedMoveDto> Moves, List<SeedSpeciesDto> Species, List<SeedEvolutionDto> Evolutions);
    internal record SeedAbilityDto(int Id, string Name, AbilityTrigger Trigger, EffectType? EffectType, BattleStat? TargetStat, int? StageChange, StatusCondition? StatusInteraction);
    internal record SeedMoveDto(int Id, string Name, PokemonType Type, MoveCategory Category, int Power, int Accuracy, int PP, EffectType EffectType, BattleStat? TargetStat, int? StageChange, StatusCondition? ApplyStatus, int? SecondaryEffectChance);
    internal record SeedSpeciesDto(int Id, string Name, PokemonType PrimaryType, PokemonType? SecondaryType, Rarity Rarity, int BaseHp, int BaseAttack, int BaseDefense, int BaseSpecialAttack, int BaseSpecialDefense, int BaseSpeed, Gender BaseGender, List<int> AbilityIds, List<int> MoveIds);
    internal record SeedEvolutionDto(int FromSpeciesId, int ToSpeciesId, int RequiredLevel);

    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (await context.PokemonSpecies.AnyAsync()) return;

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "seed-data.json");
            var json = await File.ReadAllTextAsync(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var data = JsonSerializer.Deserialize<SeedDataDto>(json, options)!;

            var abilities = data.Abilities.Select(a => new Ability
            {
                Id = a.Id,
                Name = a.Name,
                Trigger = a.Trigger,
                EffectType = a.EffectType,
                TargetStat = a.TargetStat,
                StageChange = a.StageChange,
                StatusInteraction = a.StatusInteraction
            }).ToList();

            var moves = data.Moves.Select(m => new Move
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
            }).ToList();

            var species = data.Species.Select(s => new PokemonSpecies
            {
                Id = s.Id,
                Name = s.Name,
                PrimaryType = s.PrimaryType,
                SecondaryType = s.SecondaryType,
                Rarity = s.Rarity,
                BaseHp = s.BaseHp,
                BaseAttack = s.BaseAttack,
                BaseDefense = s.BaseDefense,
                BaseSpecialAttack = s.BaseSpecialAttack,
                BaseSpecialDefense = s.BaseSpecialDefense,
                BaseSpeed = s.BaseSpeed,
                BaseGender = s.BaseGender
            }).ToList();

            context.Abilities.AddRange(abilities);
            context.Moves.AddRange(moves);
            context.PokemonSpecies.AddRange(species);
            await context.SaveChangesAsync();

            foreach (var seedSpec in data.Species)
            {
                var dbSpec = species.First(s => s.Id == seedSpec.Id);
                foreach (var abId in seedSpec.AbilityIds)
                    dbSpec.Abilities.Add(abilities.First(a => a.Id == abId));

                foreach (var mvId in seedSpec.MoveIds)
                    dbSpec.Moves.Add(moves.First(m => m.Id == mvId));
            }
            await context.SaveChangesAsync();

            var evolutions = data.Evolutions.Select(e => new Evolution
            {
                FromSpeciesId = e.FromSpeciesId,
                ToSpeciesId = e.ToSpeciesId,
                RequiredLevel = e.RequiredLevel
            }).ToList();

            context.Evolutions.AddRange(evolutions);
            await context.SaveChangesAsync();
        }
    }
}