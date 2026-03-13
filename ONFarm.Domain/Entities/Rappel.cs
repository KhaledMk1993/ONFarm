namespace ONFarm.Domain.Entities;
public class Rappel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public DateTime DateRappel { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool EstVu { get; set; } = false;
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public Patient? Patient { get; set; }
}
