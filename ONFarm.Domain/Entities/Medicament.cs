namespace ONFarm.Domain.Entities;
public class Medicament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequence { get; set; }
    public string? Notes { get; set; }
    public bool IsActif { get; set; } = true;

    public Patient? Patient { get; set; }
}
