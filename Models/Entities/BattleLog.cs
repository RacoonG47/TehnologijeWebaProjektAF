namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities
{
    public class BattleLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string TurnsJson { get; set; } = "[]";

        public bool IsVictory { get; set; }
        public DateTime BattleDate { get; set; } = DateTime.UtcNow;
    }
}
