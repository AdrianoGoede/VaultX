#nullable disable
namespace VaultX.Services.Models
{
    public record AuthenticationResponse
    {
        public string Token { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Message { get; set; }
    }

    public record StandardOperationResponse
    {
        public uint Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    
    public record CurrencyOperationResponse
    {
        public string Code { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}