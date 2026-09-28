using Industrie.Models;
using Industrie.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Industrie.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IEncryptionService _encryptionService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionService encryptionService)
            : base(options)
        {
            _encryptionService = encryptionService;
        }

        public DbSet<Machine> Machines { get; set; }
        public DbSet<OS> OSList { get; set; }
        public DbSet<Ecran> Ecrans { get; set; }
        public DbSet<Site> Sites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convertisseur EF Core pour chiffrer/déchiffrer automatiquement Mdp
            var passwordConverter = new ValueConverter<string, string>(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            );

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(m => m.Code);

                // Application du chiffrement automatique sur la colonne Mdp
                entity.Property(m => m.Mdp)
                      .HasConversion(passwordConverter)
                      .IsRequired();

                // Configuration des relations avec protection contre la suppression en cascade
                entity.HasOne(m => m.TypeOS)
                      .WithMany()
                      .HasForeignKey(m => m.OSId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.TypeEcran)
                      .WithMany()
                      .HasForeignKey(m => m.EcranId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Site)
                      .WithMany()
                      .HasForeignKey(m => m.SiteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}