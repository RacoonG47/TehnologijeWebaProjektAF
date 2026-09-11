using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class WildEncounterDto
    {
        public int SpeciesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonType PrimaryType { get; set; }
        public PokemonType? SecondaryType { get; set; }
        public int Level { get; set; }
        public Gender Gender { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public Rarity Rarity { get; set; }

        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }

        public List<MoveDto> Moves { get; set; } = new();
        public AbilityDto? Ability { get; set; }
    }

    public class CatchResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? OwnedPokemonId { get; set; }
    }

    public class AvailableOptionsDto
    {
        public List<MoveDto> AvailableMoves { get; set; } = new();
        public List<AbilityDto> AvailableAbilities { get; set; } = new();
        public List<HeldItemDto> AllItems { get; set; } = new();
    }

    public class HeldItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateSetupDto
    {
        public List<int> SelectedMoveIds { get; set; } = new();
        public int? SelectedAbilityId { get; set; }
        public int? SelectedItemId { get; set; }
    }

    public class EvolutionOptionDto
    {
        public int TargetSpeciesId { get; set; }
        public string TargetName { get; set; } = string.Empty;
        public int RequiredLevel { get; set; }
    }

    public class EvolutionCheckDto
    {
        public bool CanEvolve { get; set; }
        public List<EvolutionOptionDto> Options { get; set; } = new();
    }

    public class NpcTrainerDto
    {
        public string ArchetypeName { get; set; } = string.Empty;
        public List<NpcPokemonDto> Team { get; set; } = new();
    }

    public class NpcPokemonDto
    {
        public int SpeciesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public Gender Gender { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public PokemonType PrimaryType { get; set; }
        public PokemonType? SecondaryType { get; set; }
        public List<MoveDto> Moves { get; set; } = new();
        public AbilityDto? Ability { get; set; }

        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpecialAttack { get; set; }
        public int BaseSpecialDefense { get; set; }
        public int BaseSpeed { get; set; }
    }
}