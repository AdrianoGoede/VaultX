using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VaultX.Database.Models
{
    public record Customer
    {
        [Key]
        public uint Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateOnly JoinedAt { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<AccessToken> AccessTokens { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }

    public record AccessToken
    {
        [Key]
        public string Token { get; set; }
        public uint CustomerId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public Customer Customer { get; set; }
    }
}