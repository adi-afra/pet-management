using backend.classes;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // --- DbSets for all entities ---
        public DbSet<User> Users { get; set; }           // Base class for Admin + Client
        public DbSet<Client> Clients { get; set; }       // Specific type for convenience
        public DbSet<Admin> Admins { get; set; }         // Specific type for convenience

        public DbSet<Pet> Pets { get; set; }             // Base class for Dog + Cat
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Cat> Cats { get; set; }

        public DbSet<Meeting> Meetings { get; set; }
        
        public DbSet<SavedPets> SavedPets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configure ID generation ---
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd(); // tells EF: let the database generate the ID

            modelBuilder.Entity<Pet>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Meeting>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd();
            

            // --- Configure inheritance ---

            // Users: Client & Admin
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Client>("Client")
                .HasValue<Admin>("Admin");

            // Pets: Dog & Cat
            modelBuilder.Entity<Pet>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Dog>("Dog")
                .HasValue<Cat>("Cat");

            // --- Configure relationships ---

            // Client 1 -> * Meetings
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Meetings)
                .WithOne()
                .HasForeignKey(m => m.UserId)
                .IsRequired();

            // Pet 1 -> * Meetings
            modelBuilder.Entity<Pet>()
                .HasMany(p => p.Meetings)
                .WithOne(m => m.Pet)
                .HasForeignKey(m => m.PetId)
                .IsRequired();
            
            //prevents duplicate saves from the same user
            modelBuilder.Entity<SavedPets>()
                .HasIndex(sp => new { sp.UserId, sp.PetId })
                .IsUnique();

            // 1 User -> * Saved Pets
            modelBuilder.Entity<SavedPets>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedPets)
                .HasForeignKey(sp => sp.UserId);

            // 1 Pet -> * Saved Pets
            modelBuilder.Entity<SavedPets>()
                .HasOne(sp => sp.Pet)
                .WithMany(p => p.SavedPets)
                .HasForeignKey(sp => sp.PetId);

            modelBuilder.Entity<Pet>()
                .HasIndex(p => new { p.UserId, p.Name, p.Age, p.Breed })
                .IsUnique();

            modelBuilder.Entity<Meeting>()
                .HasIndex(m => new { m.PetId, m.Type })
                .IsUnique();
        }
    }
}