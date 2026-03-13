using ClosedXML.Excel;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Globalization;

namespace ONFarm.Application.Services;

public class ExcelService : IExcelService
{
    public async Task<IEnumerable<Patient>> ImporterPatientsAsync(string filePath)
    {
        var patients = new List<Patient>();

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            var usedRange = worksheet.RangeUsed();
            if (usedRange == null)
                return patients;

            var rows = usedRange.RowsUsed().Skip(1); 
            int rowIndex = 1;

            foreach (var row in rows)
            {
                rowIndex++;

                try
                {
                    string nom = row.Cell(1).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(nom))
                        continue;

                    string prenom = row.Cell(2).GetString().Trim();
                    string telephone = NullIfEmpty(row.Cell(3).GetString());
                    string notes = NullIfEmpty(row.Cell(4).GetString());
                    string adresse = NullIfEmpty(row.Cell(5).GetString());

                    DateOnly? dateNaissance = ParseDate(row.Cell(6));

                    var patient = new Patient
                    {
                        Nom = nom,
                        Prenom = prenom,
                        Telephone = telephone,
                        Notes = notes,
                        Adresse = adresse,
                        IsActive = true
                    };

                    patient.SetDateNaissance(dateNaissance);

                    patients.Add(patient);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erreur lors du traitement de la ligne {rowIndex}", ex);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erreur lors de l'importation du fichier Excel", ex);
        }

        return await Task.FromResult(patients);
    }

    public async Task ExporterPatientsAsync(IEnumerable<Patient> patients, string filePath)
    {
        await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Patients");

            var headers = new[]
            {
                "Nom",
                "Prénom",
                "Téléphone",
                "Notes",
                "Adresse",
                "Date Naissance",
                "Dernière facture",
                "Prochaine facture"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E7D32");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            ws.Row(1).Height = 22;

            int row = 2;

            foreach (var p in patients)
            {
                ws.Cell(row, 1).Value = p.Nom;
                ws.Cell(row, 2).Value = p.Prenom;
                ws.Cell(row, 3).Value = p.Telephone ?? "";
                ws.Cell(row, 4).Value = p.Notes ?? "";
                ws.Cell(row, 5).Value = p.Adresse ?? "";

                if (p.DateNaissance.HasValue)
                {
                    ws.Cell(row, 6).Value = p.DateNaissance.Value.ToDateTime(TimeOnly.MinValue);
                    ws.Cell(row, 6).Style.DateFormat.Format = "dd/MM/yyyy";
                }

                if (p.DerniereFacture.HasValue)
                {
                    ws.Cell(row, 7).Value = p.DerniereFacture.Value;
                    ws.Cell(row, 7).Style.DateFormat.Format = "dd/MM/yyyy";
                }
                else
                {
                    ws.Cell(row, 7).Value = "-";
                }

                if (p.ProchaineFacture.HasValue)
                {
                    ws.Cell(row, 8).Value = p.ProchaineFacture.Value;
                    ws.Cell(row, 8).Style.DateFormat.Format = "dd/MM/yyyy";
                }
                else
                {
                    ws.Cell(row, 8).Value = "-";
                }

                if (row % 2 == 0)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F8E9");

                ws.Row(row).Height = 18;

                row++;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            workbook.SaveAs(filePath);
        });
    }


    private static string? NullIfEmpty(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static DateOnly? ParseDate(IXLCell cell)
    {
        if (cell.IsEmpty())
            return null;

        if (cell.DataType == XLDataType.DateTime)
        {
            var d = cell.GetDateTime();
            return DateOnly.FromDateTime(d);
        }

        var raw = cell.GetString().Trim();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        if (DateTime.TryParseExact(
            raw,
            new[]
            {
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "d/M/yyyy",
                "yyyy/MM/dd"
            },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsed))
        {
            return DateOnly.FromDateTime(parsed);
        }

        if (DateTime.TryParse(raw, out var fallback))
        {
            return DateOnly.FromDateTime(fallback);
        }

        return null;
    }
}