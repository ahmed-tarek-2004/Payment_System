using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Payment_System_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_System_Project
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Payment.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
            // optionsBuilder.UseSqlServer("Data Source=A7MEDTAREK;Initial Catalog=Payment;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(b => b.UserId);
                entity.Property(b => b.UserName).IsRequired();
                entity.Property(b => b.Email).IsRequired();
                entity.Property(b => b.Password).IsRequired();
                entity.Property(b => b.CreatedAt).IsRequired();
                //entity.HasIndex(b => new { b.UserName, b.Email }).IsUnique();
                entity.HasIndex(b => b.UserName).IsUnique();
                entity.HasIndex(b => b.Email).IsUnique();
                entity.Property(b => b.CreatedAt).HasDefaultValueSql("getdate()");
                
                entity.
                HasMany(b => b.method)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

                entity.
                HasMany(b => b.auditLog)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserID);

                entity
                .HasMany(b => b.transaction)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Restrict);
                entity
                .Ignore(p => p.methos);

            });
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodId);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Details).IsRequired();
                entity.Property(e => e.IsDefault).HasDefaultValueSql("0");
                entity.HasMany(b => b.Transactions)
                .WithOne(b => b.PaymentMethod)
                .HasForeignKey(b => b.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
               // entity.Ignore(b => b.methos);
            });
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GetDate()");
            }); 
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.Action).IsRequired();
                //entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GetDate()");
            });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PaymentMethod> paymentMethods { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
