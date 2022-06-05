namespace EP.Infrastructure.Services.Stripe.DTO
{
    public class StripeSessionDto
    {
        public string Movie { get; set; }
        public string Hall { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public decimal Price { get; set; }
        public string TransactionUniqueCode { get; set; }
    }
}
