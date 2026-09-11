using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Enums;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private readonly IBattleService _battleService;
        private readonly ICatchService _catchService;
        private readonly INpcTrainerService _npcTrainerService;
        private readonly IRepository<Models.Entities.BattleLog> _battleLogRepo;
        private readonly ApplicationDbContext _context;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();

        public BattleController(
            IBattleService battleService,
            ICatchService catchService,
            INpcTrainerService npcTrainerService,
            IRepository<Models.Entities.BattleLog> battleLogRepo,
            ApplicationDbContext context)
        {
            _battleService = battleService;
            _catchService = catchService;
            _npcTrainerService = npcTrainerService;
            _battleLogRepo = battleLogRepo;
            _context = context;
        }

        [HttpGet("wild")]
        public async Task<IActionResult> GetWildEncounter([FromQuery] PokemonType type)
            => Ok(await _catchService.GenerateWildEncounterAsync(UserId, type));

        [HttpPost("catch")]
        public async Task<IActionResult> CatchPokemon([FromBody] CatchAttemptDto dto)
            => Ok(await _catchService.AttemptCatchAsync(UserId, dto.SpeciesId, dto.Level, dto.CurrentHp, dto.MaxHp, dto.Gender));

        [HttpGet("trainer")]
        public async Task<IActionResult> GenerateTrainer([FromQuery] string archetype)
            => Ok(await _npcTrainerService.GenerateNpcTrainerAsync(UserId, archetype));

        [HttpPost("victory")]
        public async Task<IActionResult> HandleVictory()
        {
            await _npcTrainerService.HandleVictoryAsync(UserId);
            return Ok(new { Message = "Team leveled up by 1!" });
        }

        [HttpPost("damage")]
        public IActionResult CalculateDamage([FromBody] CalculateDamageDto dto)
            => Ok(_battleService.CalculateDamage(dto));

        [HttpPost("log")]
        public async Task<IActionResult> LogBattle([FromBody] BattleLogRequestDto dto)
        {
            var log = new Models.Entities.BattleLog
            {
                UserId = UserId,
                TurnsJson = JsonSerializer.Serialize(dto.Turns),
                IsVictory = dto.IsVictory,
                BattleDate = DateTime.UtcNow
            };

            await _battleLogRepo.AddAsync(log);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Battle logged!" });
        }
    }
}