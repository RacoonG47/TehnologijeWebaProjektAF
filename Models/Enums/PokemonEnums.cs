namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums
{
    public enum Rarity { Common, Uncommon, Rare, PseudoLegendary, Mythical, Legendary }
    public enum Gender { Genderless = 0, Male = 1, Female = 2 }
    public enum PokemonType { Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy }
    public enum MoveCategory { Physical, Special, Status }

    public enum EffectType { None, StatChange, ApplyStatus, Heal }
    public enum BattleStat { Attack, Defense, SpecialAttack, SpecialDefense, Speed }
    public enum StatusCondition { None, Burn, Paralyze, Poison, Sleep, Freeze }
    public enum AbilityTrigger { None, OnSwitchIn, WhileActive, OnStatusApplied }
}