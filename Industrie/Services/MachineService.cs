using Industrie.Data;
using Industrie.Models;
using Microsoft.EntityFrameworkCore;

namespace Industrie.Services
{
    public class MachineService : IMachineService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public MachineService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        #region --- MACHINE ---

        public async Task<List<Machine>> GetAllAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Machines
                .Include(m => m.TypeOS)
                .Include(m => m.TypeEcran)
                .Include(m => m.Site)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Machine?> GetByCodeAsync(string code)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Machines
                .Include(m => m.TypeOS)
                .Include(m => m.TypeEcran)
                .Include(m => m.Site)
                .FirstOrDefaultAsync(m => m.Code == code);
        }

        public async Task<List<Machine>> GetAllBySiteAsync(int siteId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Machines
                .Where(m => m.SiteId == siteId)
                .Include(m => m.TypeOS)
                .Include(m => m.TypeEcran)
                .Include(m => m.Site)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Machine>> GetAllByOSAsync(int osId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Machines
                .Where(m => m.OSId == osId)
                .Include(m => m.TypeOS)
                .Include(m => m.TypeEcran)
                .Include(m => m.Site)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Machine>> GetAllByEcranAsync(int ecranId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Machines
                .Where(m => m.EcranId == ecranId)
                .Include(m => m.TypeOS)
                .Include(m => m.TypeEcran)
                .Include(m => m.Site)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<string?> GetMdpAsync(string code)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var machine = await context.Machines.FindAsync(code);
            // EF Core appliqué au DbContext déchiffre automatiquement la valeur de Mdp
            return machine?.Mdp;
        }

        public async Task AddMachineAsync(Machine machine)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Machines.Add(machine);
            await context.SaveChangesAsync();
        }

        public async Task UpdateMachineAsync(Machine machine)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Machines.Update(machine);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMachineAsync(string code)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var machine = await context.Machines.FindAsync(code);
            if (machine != null)
            {
                context.Machines.Remove(machine);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region --- OS ---

        public async Task<List<OS>> GetAllOSAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.OSList.AsNoTracking().ToListAsync();
        }

        public async Task AddOSAsync(OS os)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.OSList.Add(os);
            await context.SaveChangesAsync();
        }

        public async Task UpdateOSAsync(OS os)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.OSList.Update(os);
            await context.SaveChangesAsync();
        }

        public async Task DeleteOSAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var os = await context.OSList.FindAsync(id);
            if (os != null)
            {
                context.OSList.Remove(os);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region --- SITE ---

        public async Task<List<Site>> GetAllSitesAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Sites.AsNoTracking().ToListAsync();
        }

        public async Task AddSiteAsync(Site site)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Sites.Add(site);
            await context.SaveChangesAsync();
        }

        public async Task UpdateSiteAsync(Site site)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Sites.Update(site);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSiteAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var site = await context.Sites.FindAsync(id);
            if (site != null)
            {
                context.Sites.Remove(site);
                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region --- ECRAN ---

        public async Task<List<Ecran>> GetAllEcransAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Ecrans.AsNoTracking().ToListAsync();
        }

        public async Task AddEcranAsync(Ecran ecran)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Ecrans.Add(ecran);
            await context.SaveChangesAsync();
        }

        public async Task UpdateEcranAsync(Ecran ecran)
        {
            await using var context = await _factory.CreateDbContextAsync();
            context.Ecrans.Update(ecran);
            await context.SaveChangesAsync();
        }

        public async Task DeleteEcranAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var ecran = await context.Ecrans.FindAsync(id);
            if (ecran != null)
            {
                context.Ecrans.Remove(ecran);
                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}