namespace BackgroundProcessing.Workers.OrderDetails;

public interface IOrderDetailsProcessor
{
    Task ValidateOrders(CancellationToken cancellationToken);
}