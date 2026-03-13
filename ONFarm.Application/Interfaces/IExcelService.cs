using ONFarm.Domain.Entities;

namespace ONFarm.Application.Interfaces;

public interface IExcelService
{
    /// <summary>
    /// Importe les patients depuis un fichier Excel.
    /// </summary>
    /// <param name="filePath">Chemin complet du fichier Excel.</param>
    /// <returns>Liste des patients importés.</returns>
    Task<IEnumerable<Patient>> ImporterPatientsAsync(string filePath);

    /// <summary>
    /// Exporte les patients vers un fichier Excel.
    /// </summary>
    /// <param name="patients">Liste des patients à exporter.</param>
    /// <param name="filePath">Chemin complet du fichier Excel de sortie.</param>
    Task ExporterPatientsAsync(IEnumerable<Patient> patients, string filePath);
}