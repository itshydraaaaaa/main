using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Industrie.Services
{
    public class ActiveDirectoryService
    {
        private const string GroupeResponsable = "QRQCRESPONSABLE";
        private readonly ILogger<ActiveDirectoryService> _logger;

        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger)
        {
            _logger = logger;
        }

        // ── UTILITAIRES ──────────────────────────────────────────────

        public string GetUsernameOnly(string domainUser)
        {
            if (string.IsNullOrWhiteSpace(domainUser))
                return string.Empty;

            // Optimisation .NET 9 : Utilisation des Slices pour éviter l'allocation de tableaux en mémoire
            int index = domainUser.LastIndexOf('\\');
            return index >= 0 ? domainUser[(index + 1)..] : domainUser;
        }

        // ── LOOKUP INDIVIDUELS ───────────────────────────────────────

        public string? GetEmailByUsername(string username)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain);
                using var user = UserPrincipal.FindByIdentity(context, username);
                return user?.EmailAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'email pour {Username}", username);
                return null;
            }
        }

        public string GetDisplayNameByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return username;

            try
            {
                using var context = new PrincipalContext(ContextType.Domain);
                using var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                if (user == null)
                    return username;

                var display = user.DisplayName;
                if (!string.IsNullOrWhiteSpace(display))
                    return display;

                var name = user.Name;
                return !string.IsNullOrWhiteSpace(name) ? name : username;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Impossible de récupérer le DisplayName pour {Username}", username);
                return username;
            }
        }

        // ── LISTES DE GROUPES ────────────────────────────────────────

        public List<AdUser> GetResponsable(string[]? groupes = null)
        {
            var groups = groupes ?? new[] { GroupeResponsable };
            var result = new List<AdUser>();

            try
            {
                using var context = new PrincipalContext(ContextType.Domain);

                foreach (var groupName in groups)
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group == null) continue;

                    foreach (var member in group.GetMembers(recursive: true))
                    {
                        if (member is not UserPrincipal user) continue;

                        var username = user.SamAccountName ?? user.Name ?? string.Empty;

                        // Éviter les doublons si un utilisateur est dans plusieurs groupes
                        if (result.Any(r => r.Username == username)) continue;

                        result.Add(new AdUser(
                            Username: username,
                            DisplayName: !string.IsNullOrWhiteSpace(user.DisplayName)
                                            ? user.DisplayName
                                            : user.Name ?? username,
                            Email: user.EmailAddress,
                            Group: groupName
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                // Journaliser si un ILogger est injecté — silencieux pour l'instant
                _ = ex;
            }

            return result.OrderBy(u => u.DisplayName).ToList();
        }
    }

    // ── DTO ─────────────────────────────────────────────────────────
    public record AdUser(
        string Username,
        string DisplayName,
        string? Email,
        string Group
    );
}