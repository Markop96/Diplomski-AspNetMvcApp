using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using M_RENT_Aplikacija.Models;
using M_RENT_Aplikacija.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace M_RENT_Aplikacija.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Automobil> Automobili { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Automobil>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Automobil>()
                .Property(a => a.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Recenzija>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.FullPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Car)
                .WithMany()
                .HasForeignKey(r => r.IdCar)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Automobil>()
            .Property(a => a.IsActive) 
            .HasDefaultValue(true);
        }
    }
}
