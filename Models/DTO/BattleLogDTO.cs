namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO
{
    public class BattleLogRequestDto
    {
        public bool IsVictory { get; set; }
        public List<string> Turns { get; set; } = new();
    }
}