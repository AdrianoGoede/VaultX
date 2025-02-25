#nullable disable
namespace VaultX.Database.Models
{
    public record Deposit
    {
        public uint Id { get; set; }
        public uint DestinationAccountId { get; set; }
        public string Depositant { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public Account DestinationAccount { get; set; }
    }
    
    public record Transference
    {
        public uint Id { get; set; }
        public uint OriginAccountId { get; set; }
        public uint DestinationAccountId { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AmountInOriginCurrency { get; set; }
        public decimal AmountInDestinationCurrency { get; set; }
        public DateTime Timestamp { get; set; }
        public Account OriginAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }
}