using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AquariumDBContext : DbContext
{
    public AquariumDBContext(DbContextOptions<AquariumDBContext> options)
        : base(options)
    {
    }

    public DbSet<Aquarium> Aquariums => Set<Aquarium>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Coral> Corals => Set<Coral>();
    public DbSet<Fish> Fishes => Set<Fish>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("AquariumService");

        modelBuilder.Entity<Aquarium>(entity =>
        {
            entity.ToContainer("Aquariums");

            // Partition Key – extrem wichtig bei Cosmos
            entity.HasPartitionKey(a => a.OwnerId);

            // Discriminator für abstrakte Basisklasse
            entity.HasDiscriminator<string>("AquariumType")
                .HasValue<FreshWaterAquarium>("Freshwater")
                .HasValue<SeaWaterAquarium>("Seawater");
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToContainer("Animals");

            // Partition Key – wichtig bei Cosmos
            entity.HasPartitionKey(a => a.AquariumId);

            // Discriminator für abstrakte Basisklasse
            entity.HasDiscriminator<string>("AnimalType")
                .HasValue<Coral>("Coral")
                .HasValue<Fish>("Fish");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToContainer("Users");
            entity.HasPartitionKey(u => u.ID);
        });
    }
}
