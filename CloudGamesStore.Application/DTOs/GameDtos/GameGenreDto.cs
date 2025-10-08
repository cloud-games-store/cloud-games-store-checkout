using CloudGamesStore.Application.DTOs.GenreDtos;

namespace CloudGamesStore.Application.DTOs.GameDtos
{
    public class GameGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int GenreId { get; set; }
        public GenreDto Genre { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
