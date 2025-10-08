using CloudGamesStore.Application.DTOs.GameDtos;

namespace CloudGamesStore.Application.Interfaces
{
    public interface IGameServiceClient
    {
        Task<GameGenreDto> GetGamesByIdsAsync(string gameId);
    }
}
