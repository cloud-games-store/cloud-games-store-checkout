using CloudGamesStore.Application.DTOs.CartDtos;
using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ILogger<CartService> _logger;
        private readonly ICartRepository _cartRepository;

        public CartService(ILogger<CartService> logger,
            ICartRepository cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
        }

        public async Task AddItemCartByUserIdAsync(int userId, int gameId, int quantity = 1)
        {
            await _cartRepository.AddItemToCartAsync(userId, gameId, quantity);
        }

        public async Task RemoveItemFromCartAsync(int userId, int gameId)
        {
            await _cartRepository.RemoveItemFromCartAsync(userId, gameId);
        }

        public async Task UpdateItemQuantityAsync(int userId, int gameId, int newQuantity)
        {
            await _cartRepository.UpdateItemQuantityAsync(userId, gameId, newQuantity);
        }

        public Task DeleteCartByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<CartDto> GetCartByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
