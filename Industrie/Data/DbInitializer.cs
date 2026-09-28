using Industrie.Models;
using Microsoft.EntityFrameworkCore;

namespace Industrie.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync();

            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
            var providerName = context.Database.ProviderName ?? "Unknown";

            if (providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("[Database Health] Connexion active : Supabase Cloud (PostgreSQL via Npgsql)");
            }
            else if (providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("[Database Health] Mode actif : Stockage local InMemory (Environnement de développement)");
            }
            else
            {
                logger.LogInformation("[Database Health] Connexion active : {Provider}", providerName);
            }

            // S'assurer que le schéma de base de données existe
            try
            {
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[DbInitializer] EnsureCreated information");
            }

            // 1. Initialisation des Sites
            if (!await context.Sites.AnyAsync())
            {
                context.Sites.AddRange(
                    new Site { Name = "Site Alpha - Usine Nord" },
                    new Site { Name = "Site Bêta - Laboratoire Contrôle" },
                    new Site { Name = "Site Gamma - Zone Conditionnement" }
                );
                await context.SaveChangesAsync();
            }

            // 2. Initialisation des OS
            if (!await context.OSList.AnyAsync())
            {
                context.OSList.AddRange(
                    new OS { Name = "Windows 11 Pro" },
                    new OS { Name = "Windows 10 IoT Enterprise" },
                    new OS { Name = "Ubuntu 24.04 LTS" },
                    new OS { Name = "Debian 12" }
                );
                await context.SaveChangesAsync();
            }

            // 3. Initialisation des Écrans
            if (!await context.Ecrans.AnyAsync())
            {
                context.Ecrans.AddRange(
                    new Ecran { Name = "Écran Tactile Industriel 24\"" },
                    new Ecran { Name = "Double Écran 27\" 4K" },
                    new Ecran { Name = "Dalle Intégrée Siemens 15\"" }
                );
                await context.SaveChangesAsync();
            }

            // 4. Initialisation des Machines de démonstration
            if (!await context.Machines.AnyAsync())
            {
                var site = await context.Sites.FirstAsync();
                var os = await context.OSList.FirstAsync();
                var ecran = await context.Ecrans.FirstAsync();

                context.Machines.AddRange(
                    new Machine
                    {
                        Code = "MCH-001",
                        Name = "Fraiseuse CNC 5-Axes",
                        PcName = "PC-PROD-01",
                        Session = "op_fraiseuse",
                        Mdp = "CncPass2026!",
                        AdresseIpInterne = "192.168.1.101",
                        AdresseIpExterne = "10.0.1.101",
                        Bukup = true,
                        DateBukup = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                        SiteId = site.Id,
                        OSId = os.Id,
                        EcranId = ecran.Id
                    },
                    new Machine
                    {
                        Code = "MCH-002",
                        Name = "Robot d'Assemblage KUKA",
                        PcName = "PC-ROBOT-02",
                        Session = "op_kuka",
                        Mdp = "KukaSecure99",
                        AdresseIpInterne = "192.168.1.102",
                        AdresseIpExterne = "10.0.1.102",
                        Bukup = false, // Génère une alerte sur le dashboard
                        DateBukup = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                        SiteId = site.Id,
                        OSId = os.Id,
                        EcranId = ecran.Id
                    },
                    new Machine
                    {
                        Code = "MCH-003",
                        Name = "Banc d'Essai Qualité",
                        PcName = "PC-QUAL-03",
                        Session = "ctrl_qualite",
                        Mdp = "TestBench2026",
                        AdresseIpInterne = "192.168.1.103",
                        AdresseIpExterne = "10.0.1.103",
                        Bukup = true,
                        DateBukup = DateOnly.FromDateTime(DateTime.UtcNow),
                        SiteId = site.Id,
                        OSId = os.Id,
                        EcranId = ecran.Id
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
