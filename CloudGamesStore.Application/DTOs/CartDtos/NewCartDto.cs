using CloudGamesStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.DTOs.CartDtos
{
    public class NewCartDto
    {
        public Guid UserId { get; set; }
        public List<NewCartItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
