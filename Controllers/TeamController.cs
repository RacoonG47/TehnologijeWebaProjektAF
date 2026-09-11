using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeam()
        {
            return Ok(await _teamService.GetUserTeamAsync(UserId));
        }

        [HttpGet("storage")]
        public async Task<IActionResult> GetStorage()
        {
            return Ok(await _teamService.GetUserStorageAsync(UserId));
        }

        [HttpPut("swap/{ownedPokemonId}")]
        public async Task<IActionResult> SwapPokemon(int ownedPokemonId, [FromQuery] bool addToTeam)
        {
            await _teamService.SwapPokemonAsync(UserId, ownedPokemonId, addToTeam);
            return Ok(new { Message = addToTeam ? "Added to team!" : "Moved to storage!" });
        }

        [HttpDelete("release/{ownedPokemonId}")]
        public async Task<IActionResult> ReleasePokemon(int ownedPokemonId)
        {
            await _teamService.ReleasePokemonAsync(UserId, ownedPokemonId);
            return Ok(new { Message = "Pokemon released!" });
        }

        [HttpGet("options/{ownedPokemonId}")]
        public async Task<IActionResult> GetOptions(int ownedPokemonId)
        {
            return Ok(await _teamService.GetAvailableOptionsAsync(UserId, ownedPokemonId));
        }

        [HttpPut("setup/{ownedPokemonId}")]
        public async Task<IActionResult> UpdateSetup(int ownedPokemonId, [FromBody] UpdateSetupDto dto)
        {
            await _teamService.UpdatePokemonSetupAsync(UserId, ownedPokemonId, dto);
            return Ok(new { Message = "Pokemon setup updated!" });
        }
    }
}