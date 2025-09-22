namespace CloudGamesStore.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IGameRepository Games { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }
        ICouponRepository Coupons { get; }
        IPromotionRepository Promotions { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
