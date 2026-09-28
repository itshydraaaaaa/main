using Industrie.Models;

namespace Industrie.Services
{
    public interface IMachineService
    {
        // --- MACHINE ---
        Task<List<Machine>> GetAllAsync();
        Task<Machine?> GetByCodeAsync(string code);
        Task<List<Machine>> GetAllBySiteAsync(int siteId);
        Task<List<Machine>> GetAllByOSAsync(int osId);
        Task<List<Machine>> GetAllByEcranAsync(int ecranId);
        Task<string?> GetMdpAsync(string code);
        Task AddMachineAsync(Machine machine);
        Task UpdateMachineAsync(Machine machine);
        Task DeleteMachineAsync(string code);

        // --- OS ---
        Task<List<OS>> GetAllOSAsync();
        Task AddOSAsync(OS os);
        Task UpdateOSAsync(OS os);
        Task DeleteOSAsync(int id);

        // --- SITE ---
        Task<List<Site>> GetAllSitesAsync();
        Task AddSiteAsync(Site site);
        Task UpdateSiteAsync(Site site);
        Task DeleteSiteAsync(int id);

        // --- ECRAN ---
        Task<List<Ecran>> GetAllEcransAsync(); 
        Task AddEcranAsync(Ecran ecran);
        Task UpdateEcranAsync(Ecran ecran);
        Task DeleteEcranAsync(int id);
    }
}