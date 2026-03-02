using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

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
        modelBuilder.Entity<Aquarium>(entity =>
        {
            entity.ToCollection("Aquariums");

            entity.HasDiscriminator<string>("AquariumType")
                .HasValue<FreshWaterAquarium>("Freshwater")
                .HasValue<SeaWaterAquarium>("Seawater");
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToCollection("Animals");

            entity.HasDiscriminator<string>("AnimalType")
                .HasValue<Coral>("Coral")
                .HasValue<Fish>("Fish");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToCollection("Users");
        });
    }
}
