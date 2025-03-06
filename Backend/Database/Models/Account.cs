using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VaultX.Database.Models
{
    public record Account
    {
        [Key]
        public uint Id { get; set; }
        public uint CustomerId { get; set; }
        public string CurrencyCode { get; set; }
        public double Balance { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public Customer Customer { get; set; }
        public Currency Currency { get; set; }
        public ICollection<Deposit> IncomingDeposits { get; set; }
        public ICollection<Transference> IncomingTransferences { get; set; }
        public ICollection<Transference> OutgoingTransferences { get; set; }
    }
}