#nullable disable
namespace VaultX.Database.Models
{
    public record Deposit
    {
        public uint Id { get; set; }
        public uint DestinationAccountId { get; set; }
        public string Depositant { get; set; }
        public string CurrencyCode { get; set; }
        public double ConversionRate { get; set; }
        public double OriginalAmount { get; set; }
        public double ConvertedAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public Account DestinationAccount { get; set; }
    }
    
    public record Transference
    {
        public uint Id { get; set; }
        public uint OriginAccountId { get; set; }
        public uint DestinationAccountId { get; set; }
        public double ConversionRate { get; set; }
        public double AmountInOriginCurrency { get; set; }
        public double AmountInDestinationCurrency { get; set; }
        public DateTime Timestamp { get; set; }
        public Account OriginAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }
}