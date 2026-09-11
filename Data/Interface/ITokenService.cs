namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data.Interface
{
    public interface ITokenService
    {
        string CreateToken(string username, string userId, string role);
    }
}
