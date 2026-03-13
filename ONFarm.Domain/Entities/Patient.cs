using ONFarm.Domain.Enums;

namespace ONFarm.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public DateOnly? DateNaissance { get; private set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    public ICollection<Medicament> Medicaments { get; set; } = new List<Medicament>();
    public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    public ICollection<Rappel> Rappels { get; set; } = new List<Rappel>();

    public string NomComplet => $"{Prenom} {Nom}".Trim();

    public string DateNaissanceAffichage =>
        DateNaissance.HasValue
            ? DateNaissance.Value.ToString("dd/MM/yyyy")
            : "—";

    public void SetDateNaissance(DateOnly? date)
    {
        DateNaissance = date;
    }


    public DateTime? DerniereFacture =>
        Ordonnances.OrderByDescending(o => o.DateFacture)
            .FirstOrDefault()?.DateFacture;

    public DateTime? ProchaineFacture =>
        Ordonnances.Where(o => o.ProchainRenouvellement >= DateTime.UtcNow.Date)
            .OrderBy(o => o.ProchainRenouvellement)
            .FirstOrDefault()?.ProchainRenouvellement;
}