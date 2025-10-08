using CloudGamesStore.Application.DTOs.GameDtos;
using CloudGamesStore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Infrastructure.Client
{
    public class GameServiceClient : IGameServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GameServiceClient> _logger;

        public GameServiceClient(HttpClient httpClient, ILogger<GameServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GameGenreDto> GetGamesByIdsAsync(string gameId)
        {
            var response = await _httpClient.GetAsync("games/" + gameId);
            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get game data: {Status}", response.StatusCode);
                    throw new HttpRequestException("Failed to fetch game data");
                }

                var content = await response.Content.ReadFromJsonAsync<GameGenreDto>();
                return content!;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to convert game dat", ex.Message);
                throw new HttpRequestException("Failed to convert game data");
            }
        }
    }
}
