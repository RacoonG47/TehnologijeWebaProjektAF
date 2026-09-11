using System.Collections.Generic;
using System.Threading.Tasks;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface ITeamService
    {
        Task<List<OwnedPokemonDTO>> GetUserTeamAsync(string userId);
        Task<List<OwnedPokemonDTO>> GetUserStorageAsync(string userId);
        Task SwapPokemonAsync(string userId, int ownedPokemonId, bool addToTeam);
        Task ReleasePokemonAsync(string userId, int ownedPokemonId);
        Task<AvailableOptionsDto> GetAvailableOptionsAsync(string userId, int ownedPokemonId);
        Task UpdatePokemonSetupAsync(string userId, int ownedPokemonId, UpdateSetupDto dto);
    }
}