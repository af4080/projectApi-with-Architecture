using Microsoft.EntityFrameworkCore;
using projectApiAngular.Models;

namespace projectApiAngular.Data
{
    public class Chinese_SalesDbContext : DbContext
    {

        public Chinese_SalesDbContext(DbContextOptions<Chinese_SalesDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Donner> Doners { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Basket> Baskets { get; set; }

        //  public DbSet<Maneger> manegers { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   //configure User entity
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(70);
            modelBuilder.Entity<User>().Property(u => u.Phone).HasMaxLength(15);
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();


            //configure Donner entity
            modelBuilder.Entity<Donner>().HasKey(d => d.Id);
            modelBuilder.Entity<Donner>().Property(d => d.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Donner>().Property(d => d.Email).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Donner>().Property(d => d.Phone).HasMaxLength(15);
  
            //configure Category entity
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Category>().HasIndex(c=> c.Name).IsUnique();
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Gifts)
                .WithOne(g => g.Category)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            //configure Gift entity
            modelBuilder.Entity<Gift>().HasKey(g => g.Id);
            modelBuilder.Entity<Gift>().Property(g => g.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Gift>().Property(g => g.Description).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<Gift>().Property(g => g.Price).IsRequired();
            modelBuilder.Entity<Gift>().Property(g => g.ImagePath).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Gift>().Property(g => g.DonerId).IsRequired();
            modelBuilder.Entity<Gift>().Property(g => g.CategoryId).IsRequired();
            modelBuilder.Entity<Gift>().Property(g => g.WinnerId).IsRequired(false);

            modelBuilder.Entity<Gift>()
              .HasOne(g => g.Doner)
              .WithMany(d => d.Gifts)
              .HasForeignKey(g => g.DonerId)
              .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Winner)
                .WithMany(c=>c.WonGifts)
                .HasForeignKey(g => g.WinnerId)
                .OnDelete(DeleteBehavior.SetNull);


            //configure Purchase entity
            modelBuilder.Entity<Purchase>().HasKey(p => p.Id);
            modelBuilder.Entity<Purchase>().Property(p => p.PurchaseDate).IsRequired();
            modelBuilder.Entity<Purchase>().Property(p => p.CustomerId).IsRequired();
            modelBuilder.Entity<Purchase>().Property(p => p.GiftId).IsRequired();
            modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Gift)
            .WithMany(g=>g.Purchases)
            .HasForeignKey(p => p.GiftId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Castomer)
            .WithMany(u => u.Purchases)
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

            //configure Basket entity
            modelBuilder.Entity<Basket>().HasKey(b => b.Id);
            modelBuilder.Entity<Basket>().Property(b => b.UserId).IsRequired();
            modelBuilder.Entity<Basket>().Property(b => b.GiftId).IsRequired();
            modelBuilder.Entity<Basket>()
             .HasIndex(b => new { b.UserId, b.GiftId })
             .IsUnique();



            //configure Maneger entity
            //modelBuilder.Entity <Maneger>().HasKey(m => m.Id);
            //modelBuilder.Entity <Maneger>().Property(m => m.Name).IsRequired().HasMaxLength(50);
            //modelBuilder.Entity<Maneger>().Property(m => m.Email).IsRequired().HasMaxLength(50);
            //modelBuilder.Entity <Maneger>().Property(m => m.Password).IsRequired().HasMaxLength(70);

            //email is unique
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Donner>().HasIndex(u=> u.Email).IsUnique();
            //modelBuilder.Entity<Maneger>().HasIndex(m => m.Email).IsUnique();
            //
            modelBuilder.Entity<Gift>().HasIndex(n => n.Name).IsUnique();
        }
    }
}
