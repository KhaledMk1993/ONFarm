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
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public Patient? Patient { get; set; }
    public bool EstImminente =>
        ProchainRenouvellement >= DateTime.UtcNow.Date &&
        ProchainRenouvellement <= DateTime.UtcNow.Date.AddDays(2);

    public bool EstEnRetard =>
        ProchainRenouvellement < DateTime.UtcNow.Date &&
        Statut == StatutOrdonnance.EnAttente;
}