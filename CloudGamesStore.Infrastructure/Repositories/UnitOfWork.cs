using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Interfaces;
using CloudGamesStore.Infrastructure.Client;
using CloudGamesStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameStoreCheckoutDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;

        //private IGameRepository? _games;
        private IGameServiceClient _gameServiceClient;
        private ICartRepository? _carts;
        private IOrderRepository? _orders;
        private ICouponRepository? _coupons;
        private IPromotionRepository? _promotions;

        public UnitOfWork(
            GameStoreCheckoutDbContext context,
            ILogger<UnitOfWork> logger,
            //ILogger<GameRepository> gameLogger,
            ILogger<GameServiceClient> gameLogger,
            ILogger<CartRepository> cartLogger,
            ILogger<OrderRepository> orderLogger,
            ILogger<CouponRepository> couponLogger,
            ILogger<PromotionRepository> promotionLogger)
        {
            _context = context;
            _logger = logger;

            // Initialize repositories with their specific loggers
            _carts = new CartRepository(_context, cartLogger, _gameServiceClient);
            _orders = new OrderRepository(_context, orderLogger);
            _coupons = new CouponRepository(_context, couponLogger);
            _promotions = new PromotionRepository(_context, promotionLogger);
        }

        //public IGameRepository Games => _games ??= new GameRepository(_context,
        //    _context.GetService<ILogger<GameRepository>>());

        public ICartRepository Carts => _carts ??= new CartRepository(_context,
            _context.GetService<ILogger<CartRepository>>(), _gameServiceClient);

        public IOrderRepository Orders => _orders ??= new OrderRepository(_context,
            _context.GetService<ILogger<OrderRepository>>());

        public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context,
            _context.GetService<ILogger<CouponRepository>>());

        public IPromotionRepository Promotions => _promotions ??= new PromotionRepository(_context,
            _context.GetService<ILogger<PromotionRepository>>());

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes in Unit of Work");
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            try
            {
                _transaction = await _context.Database.BeginTransactionAsync();
                _logger.LogInformation("Transaction started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning transaction");
                throw;
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    _logger.LogInformation("Transaction committed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                    _logger.LogInformation("Transaction rolled back");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction");
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
