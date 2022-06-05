namespace EP.Application.Services.Payment.DTO.Response
{
    public class CreatePaymentResponseDto
    {
        public string PaymentUrl { get; set; }
        public string Movie { get; set; }
        public string Hall { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public decimal Price { get; set; }
        public string StartTime { get; set; }
        public string TransactionUniqueCode { get; set; }
        public string QrCodeUrl { get; set; }
    }
}
