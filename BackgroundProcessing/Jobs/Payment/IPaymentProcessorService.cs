namespace BackgroundProcessing.Jobs.Payment
{
    public interface IPaymentProcessorService
    {
        Task CancelInvalidPayment();
    }
}
