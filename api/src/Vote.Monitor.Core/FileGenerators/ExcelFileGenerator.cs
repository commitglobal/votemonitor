using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Vote.Monitor.Core.FileGenerators;

public class ExcelFileGenerator
{
    private readonly IWorkbook _workbook;
    private readonly ICellStyle _headerStyle;

    private ExcelFileGenerator()
    {
        _workbook = new XSSFWorkbook();

        _headerStyle = _workbook.CreateCellStyle();
        var headerFont = _workbook.CreateFont();
        headerFont.IsBold = true;
        _headerStyle.SetFont(headerFont);
    }

    public static ExcelFileGenerator New()
    {
        return new ExcelFileGenerator();
    }

    public ExcelFileGenerator WithSheet(string sheetName, List<string> header, List<List<object>> data)
    {
        var sheet = _workbook.CreateSheet(sheetName);

        // Create header row
        IRow headerRow = sheet.CreateRow(0);

        for (var i = 0; i < header.Count; i++)
        {
            var cell = headerRow.CreateCell(i);
            cell.SetCellValue(header[i]);
            cell.CellStyle = _headerStyle;
        }

        // Create data rows
        for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
        {
            IRow dataRow = sheet.CreateRow(rowIndex + 1);
            for (int colIndex = 0; colIndex < data[rowIndex].Count; colIndex++)
            {
                var cell = dataRow.CreateCell(colIndex);
                var cellValue = data[rowIndex][colIndex]?.ToString();
                cell.SetCellValue(cellValue);

                if ((cellValue?.StartsWith("https://") ?? false) || (cellValue?.StartsWith("http://") ?? false))
                {
                    var targetLink = new XSSFHyperlink(HyperlinkType.Url)
                    {
                        Address = cellValue
                    };

                    cell.Hyperlink = targetLink;
                }
            }
        }

        return this;
    }

    public string Please()
    {
        using (var memoryStream = new MemoryStream())
        {
            _workbook.Write(memoryStream);

            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}
