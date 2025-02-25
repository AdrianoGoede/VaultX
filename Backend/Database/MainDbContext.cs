using Microsoft.EntityFrameworkCore;
using VaultX.Database.Models;

namespace VaultX.Database
{
    public partial class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) {}

        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Transference> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity => {
                entity.ToTable("customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").IsRequired(true).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired(true).HasMaxLength(100);
                entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired(true).HasMaxLength(100);
                entity.Property(e => e.BirthDate).HasColumnName("birth_date").IsRequired(true);
                entity.Property(e => e.JoinedAt).HasColumnName("joined_at").IsRequired(true);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired(true).HasMaxLength(64);
                entity.Property(e => e.PasswordSalt).HasColumnName("password_salt").IsRequired(true).HasMaxLength(64);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(true).HasDefaultValue(true);
            });

            modelBuilder.Entity<AccessToken>(entity => {
                entity.ToTable("access_tokens");
                entity.HasKey(e => e.Token);
                entity.Property(e => e.Token).HasColumnName("token").IsRequired(true).HasMaxLength(64);
                entity.Property(e => e.CustomerId).HasColumnName("customer").IsRequired(true);
                entity.Property(e => e.IssuedAt).HasColumnName("issued_at").IsRequired(true);
                entity.Property(e => e.ValidUntil).HasColumnName("valid_until").IsRequired(true);
                entity.HasOne(e => e.Customer).WithMany(c => c.AccessTokens).HasForeignKey(e => e.CustomerId);
            });

            modelBuilder.Entity<Currency>(entity => {
                entity.ToTable("currencies");
                entity.HasKey(e => e.Code);
                entity.Property(e => e.Code).HasColumnName("code").IsRequired(true).HasMaxLength(4);
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsCrypto).HasColumnName("is_crypto").IsRequired(true).HasDefaultValue(false);
                entity.Property(e => e.IconUrl).HasColumnName("icon_url").IsRequired(false).HasMaxLength(2048);
            });

            modelBuilder.Entity<Account>(entity => {
                entity.ToTable("accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").IsRequired(true).ValueGeneratedOnAdd();
                entity.Property(e => e.CustomerId).HasColumnName("customer").IsRequired(true);
                entity.Property(e => e.CurrencyCode).HasColumnName("currency").IsRequired(true).HasMaxLength(4);
                entity.Property(e => e.Balance).HasColumnName("balance").IsRequired(true).HasPrecision(25,2).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(true).HasDefaultValue(true);
                entity.Property(e => e.IsBlocked).HasColumnName("is_blocked").IsRequired(true).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired(true);
                entity.HasOne(e => e.Customer).WithMany(p => p.Accounts).HasForeignKey(e => e.CustomerId);
                entity.HasOne(e => e.Currency).WithMany(p => p.Accounts).HasForeignKey(e => e.CurrencyCode);
            });

            modelBuilder.Entity<Deposit>(entity => {
                entity.ToTable("deposits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").IsRequired(true).ValueGeneratedOnAdd();
                entity.Property(e => e.DestinationAccountId).HasColumnName("destination_account").IsRequired(true);
                entity.Property(e => e.Depositant).HasColumnName("depositant").IsRequired(true).HasMaxLength(200);
                entity.Property(e => e.CurrencyCode).HasColumnName("currency").IsRequired(true).HasMaxLength(4);
                entity.Property(e => e.ConversionRate).HasColumnName("conversion_rate").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.OriginalAmount).HasColumnName("original_amount").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.ConvertedAmount).HasColumnName("converted_amount").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.Timestamp).HasColumnName("timestamp").IsRequired(true);
                entity.HasOne(e => e.DestinationAccount).WithMany(p => p.IncomingDeposits).HasForeignKey(e => e.DestinationAccountId);
            });

            modelBuilder.Entity<Transference>(entity => {
                entity.ToTable("transferences");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").IsRequired(true).ValueGeneratedOnAdd();
                entity.Property(e => e.OriginAccountId).HasColumnName("origin_account").IsRequired(true);
                entity.Property(e => e.DestinationAccountId).HasColumnName("destination_account").IsRequired(true);
                entity.Property(e => e.ConversionRate).HasColumnName("conversion_rate").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.AmountInOriginCurrency).HasColumnName("amount_origin_currency").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.AmountInDestinationCurrency).HasColumnName("amount_destination_currency").IsRequired(true).HasPrecision(25,2);
                entity.Property(e => e.Timestamp).HasColumnName("timestamp").IsRequired(true);
                entity.HasOne(e => e.OriginAccount).WithMany(p => p.OutgoingTransferences).HasForeignKey(e => e.OriginAccountId);
                entity.HasOne(e => e.DestinationAccount).WithMany(p => p.IncomingTransferences).HasForeignKey(e => e.DestinationAccountId);
            });
        }
    }
}