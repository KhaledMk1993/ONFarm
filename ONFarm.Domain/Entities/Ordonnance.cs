using ONFarm.Domain.Enums;

namespace ONFarm.Domain.Entities;
public class Ordonnance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public DateTime DateFacture { get; set; }
    public DateTime ProchainRenouvellement { get; set; }
    public StatutOrdonnance Statut { get; set; } = StatutOrdonnance.EnAttente;
    public string? Notes { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public Patient? Patient { get; set; }

    public bool EstImminente =>
        ProchainRenouvellement >= DateTime.Today &&
        ProchainRenouvellement <= DateTime.Today.AddDays(2);

    public bool EstEnRetard =>
        ProchainRenouvellement < DateTime.Today &&
        Statut == StatutOrdonnance.EnAttente;
}
