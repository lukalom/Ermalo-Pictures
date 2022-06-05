namespace BackgroundProcessing.Workers.OrderDetails;

public class BackgroundOrderDetailsService : BackgroundService
{

    private readonly IOrderDetailsProcessor _orderDetailsProcessor;

    public BackgroundOrderDetailsService(IOrderDetailsProcessor orderDetailsProcessor)
    {
        _orderDetailsProcessor = orderDetailsProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _orderDetailsProcessor.ValidateOrders(stoppingToken);
    }
}