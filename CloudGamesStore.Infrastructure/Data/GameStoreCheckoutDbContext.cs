using CloudGamesStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudGamesStore.Infrastructure.Data
{
    public class GameStoreCheckoutDbContext : DbContext
    {
        public GameStoreCheckoutDbContext(DbContextOptions<GameStoreCheckoutDbContext> options)
            : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionRule> PromotionRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameStoreCheckoutDbContext).Assembly);
        }
    }
}
