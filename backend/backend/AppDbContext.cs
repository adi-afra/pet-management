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
        public DbSet<AdoptionApplication> AdoptionApplications { get; set; }
        public DbSet<SavedPet> SavedPets { get; set; } 

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

            // --- Configure User Constraints ---

            //email is required for each user
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            //emails must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

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
        }
    }
}