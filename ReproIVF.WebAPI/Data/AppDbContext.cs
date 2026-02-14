using Microsoft.EntityFrameworkCore;
using ReproIVF.Shared.Entities;
using ProgramEntity = ReproIVF.Shared.Entities.Program;

namespace ReproIVF.WebAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Implant> Implants => Set<Implant>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<Bull> Bulls => Set<Bull>();
    public DbSet<ProgramEntity> Programs => Set<ProgramEntity>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<SemenType> SemenTypes => Set<SemenType>();
    public DbSet<PreservationMethod> PreservationMethods => Set<PreservationMethod>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProgramEntity>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Bull>()
            .HasIndex(x => x.Name);

        modelBuilder.Entity<Donor>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<SemenType>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<PreservationMethod>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Technician>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<SemenType>().HasData(
            new SemenType { Id = 1, Name = "conv" },
            new SemenType { Id = 2, Name = "sex" },
            new SemenType { Id = 3, Name = "Empty" }
        );

        modelBuilder.Entity<PreservationMethod>().HasData(
            new PreservationMethod { Id = 1, Name = "Fresh" },
            new PreservationMethod { Id = 2, Name = "DT" },
            new PreservationMethod { Id = 3, Name = "Vit" }
        );
    }
}
