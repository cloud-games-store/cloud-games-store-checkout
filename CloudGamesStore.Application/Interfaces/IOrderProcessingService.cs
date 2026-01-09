namespace CloudGamesStore.Application.Interfaces
{
    public interface IOrderProcessingService
    {
        Task ProcessPendingOrdersAsync(object message, string queueName);
    }
}
