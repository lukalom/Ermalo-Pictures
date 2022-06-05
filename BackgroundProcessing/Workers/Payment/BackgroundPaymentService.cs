namespace BackgroundProcessing.Workers.Payment
{
    public class BackgroundPaymentService : BackgroundService
    {

        private readonly IPaymentProcessor _paymentProcessor;

        public BackgroundPaymentService(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _paymentProcessor.ValidatePayment(stoppingToken);
        }
    }
}
