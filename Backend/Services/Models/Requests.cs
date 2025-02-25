#nullable disable
namespace VaultX.Services.Models
{
    public record AuthenticationRequest
    {
        public uint CustomerId { get; set; }
        public string Password { get; set; }
    }

    public record CurrencyOperationRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsCrypto { get; set; }
        public string IconUrl { get; set; }
    }
    
    public record CustomerOperationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string Password { get; set; }
    }

    public record AccountOperationRequest
    {
        public string CurrencyCode { get; set; }
    }

    public record DepositOperationRequest
    {
        public uint DestinationAccountId { get; set; }
        public string Depositant { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
    }
    
    public record TransferenceOperationRequest
    {
        public uint OriginAccountId { get; set; }
        public uint DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}