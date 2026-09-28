namespace Industrie.Models
{
    public class Machine
    {
        public required string Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PcName { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string Mdp { get; set; } = string.Empty; // Pensez à chiffrer ce champ en BDD

        public string AdresseIpExterne { get; set; } = string.Empty;
        public string AdresseIpInterne { get; set; } = string.Empty;
        public bool Bukup { get; set; }
        public DateOnly DateBukup { get; set; }

        // Clés étrangères et propriétés de navigation
        public int EcranId { get; set; }
        public Ecran TypeEcran { get; set; } = null!;

        public int OSId { get; set; }
        public OS TypeOS { get; set; } = null!;

        public int SiteId { get; set; }
        public Site Site { get; set; } = null!;

    }
}
