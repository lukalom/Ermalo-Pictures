namespace BackgroundProcessing.Jobs.OrderDetails
{
    public interface IOrderDetailsProcessorService
    {
        Task CancelInvalidOrders();
    }
}
