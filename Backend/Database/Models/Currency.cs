using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VaultX.Database.Models
{
    public record Currency
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsCrypto { get; set; }
        public string IconUrl { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}