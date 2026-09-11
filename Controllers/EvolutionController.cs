using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EvolutionController : ControllerBase
    {
        private readonly IEvolutionService _evolutionService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();

        public EvolutionController(IEvolutionService evolutionService)
        {
            _evolutionService = evolutionService;
        }

        [HttpGet("check/{ownedPokemonId}")]
        public async Task<IActionResult> CheckEvolution(int ownedPokemonId)
        {
            return Ok(await _evolutionService.CheckEvolutionAsync(UserId, ownedPokemonId));
        }

        [HttpPost("execute/{ownedPokemonId}")]
        public async Task<IActionResult> EvolvePokemon(int ownedPokemonId, [FromQuery] int targetSpeciesId)
        {
            await _evolutionService.EvolvePokemonAsync(UserId, ownedPokemonId, targetSpeciesId);
            return Ok(new { Message = "Pokemon evolved successfully!" });
        }
    }
}