using System.IO.Compression;
using System.Text;
using ReproIVF.Shared.Entities;

namespace ReproIVF.WebAPI.Export;

public static class XlsxExportBuilder
{
    public static byte[] BuildImplantsWorkbook(List<Implant> implants)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            WriteEntry(archive, "[Content_Types].xml", ContentTypesXml());
            WriteEntry(archive, "_rels/.rels", RootRelsXml());
            WriteEntry(archive, "xl/workbook.xml", WorkbookXml());
            WriteEntry(archive, "xl/_rels/workbook.xml.rels", WorkbookRelsXml());
            WriteEntry(archive, "xl/worksheets/sheet1.xml", SheetXml(implants));
        }

        return stream.ToArray();
    }

    private static void WriteEntry(ZipArchive archive, string path, string content)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Fastest);
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        writer.Write(content);
    }

    private static string ContentTypesXml() =>
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
          <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
          <Default Extension="xml" ContentType="application/xml"/>
          <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
          <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
        </Types>
        """;

    private static string RootRelsXml() =>
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
        </Relationships>
        """;

    private static string WorkbookXml() =>
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
          <sheets>
            <sheet name="Implants" sheetId="1" r:id="rId1"/>
          </sheets>
        </workbook>
        """;

    private static string WorkbookRelsXml() =>
        """
        <?xml version="1.0" encoding="UTF-8"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
        </Relationships>
        """;

    private static string SheetXml(List<Implant> implants)
    {
        var headers = new[]
        {
            "ID", "Program", "Owner", "OPU", "Fert", "Freezing", "Straw", "Donor", "Bull",
            "Semen", "Fresh/DT/Vit", "Implant", "Recip", "Technician", "In Calf"
        };

        var sb = new StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

        AppendRow(sb, 1, headers.Select(h => CellString(h)).ToArray());

        var row = 2;
        foreach (var item in implants)
        {
            var bull = $"{item.Bull?.Name} {(string.IsNullOrWhiteSpace(item.Bull?.Code) ? "" : $"({item.Bull?.Code})")}".Trim();
            var values = new[]
            {
                CellNumber(item.Id),
                CellString(item.Program?.Name ?? ""),
                CellString(item.Owner?.Name ?? ""),
                CellString(item.OpuDate?.ToString("dd/MM/yyyy") ?? ""),
                CellString(item.FertDate?.ToString("dd/MM/yyyy") ?? ""),
                CellString(item.FreezingDate?.ToString("dd/MM/yyyy") ?? ""),
                item.StrawId.HasValue ? CellNumber(item.StrawId.Value) : CellString(""),
                CellString(item.Donor?.Code ?? ""),
                CellString(bull),
                CellString(item.SemenType?.Name ?? ""),
                CellString(item.PreservationMethod?.Name ?? ""),
                CellString(item.ImplantDate?.ToString("dd/MM/yyyy") ?? ""),
                CellString(item.RecipId ?? ""),
                CellString(item.Technician?.Name ?? ""),
                CellString(item.InCalf.HasValue ? (item.InCalf.Value ? "Yes" : "No") : "")
            };

            AppendRow(sb, row, values);
            row++;
        }

        sb.Append("</sheetData></worksheet>");
        return sb.ToString();
    }

    private static void AppendRow(StringBuilder sb, int rowIndex, string[] cells)
    {
        sb.Append($"<row r=\"{rowIndex}\">");
        for (var i = 0; i < cells.Length; i++)
        {
            var cellRef = $"{ColumnName(i + 1)}{rowIndex}";
            sb.Append(cells[i].Replace("{CELL_REF}", cellRef));
        }
        sb.Append("</row>");
    }

    private static string CellString(string value) =>
        $"<c r=\"{{CELL_REF}}\" t=\"inlineStr\"><is><t>{EscapeXml(value)}</t></is></c>";

    private static string CellNumber(int value) =>
        $"<c r=\"{{CELL_REF}}\"><v>{value}</v></c>";

    private static string EscapeXml(string value) =>
        value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");

    private static string ColumnName(int index)
    {
        var name = string.Empty;
        while (index > 0)
        {
            index--;
            name = (char)('A' + (index % 26)) + name;
            index /= 26;
        }

        return name;
    }
}
