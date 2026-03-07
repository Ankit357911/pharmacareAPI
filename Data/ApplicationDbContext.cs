
using Microsoft.EntityFrameworkCore;
using pharmacareAPI.Models;
namespace pharmacareAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MedicineCategory> MedicineCategories { get; set; }

        public DbSet<Medicine> Medicines { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionItem> TransactionItems { get; set; }

        public DbSet<Earning> Earnings { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.MobileNumber)
                .IsUnique();

            modelBuilder.Entity<MedicineCategory>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Medicines)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionCode)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionItem>()
                .HasOne(ti => ti.Transaction)
                .WithMany(t => t.TransactionItems)
                .HasForeignKey(ti => ti.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransactionItem>()
                .HasOne(ti => ti.Medicine)
                .WithMany(m => m.TransactionItems)
                .HasForeignKey(ti => ti.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Earning>()
                .HasIndex(e => new { e.PeriodType, e.PeriodStart, e.PeriodEnd })
                .IsUnique();
        }
    }
}
