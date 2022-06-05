namespace BackgroundProcessing.Workers.Payment
{
    public interface IPaymentProcessor
    {
        Task ValidatePayment(CancellationToken cancellationToken);
    }
}
