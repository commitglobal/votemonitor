using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Vote.Monitor.Core.FileGenerators;

public class ExcelFileGenerator
{
    private readonly IWorkbook _workbook;
    private readonly ICellStyle _headerStyle;
    private readonly ICellStyle _flaggedAnswerCellStyle;
    private readonly ICellStyle _flaggedValueCellStyle;

    private ExcelFileGenerator()
    {
        _workbook = new XSSFWorkbook();

        var headerFont = _workbook.CreateFont();
        headerFont.IsBold = true;

        _headerStyle = _workbook.CreateCellStyle();
        _headerStyle.SetFont(headerFont);

        _flaggedAnswerCellStyle = _workbook.CreateCellStyle();

        _flaggedAnswerCellStyle.FillForegroundColor = IndexedColors.Red.Index;
        _flaggedAnswerCellStyle.FillPattern = FillPattern.SolidForeground;
        _flaggedAnswerCellStyle.SetFont(headerFont);

        // answers that pick a flagged option: a light red fill keeps the text readable
        _flaggedValueCellStyle = _workbook.CreateCellStyle();
        _flaggedValueCellStyle.FillForegroundColor = IndexedColors.Red.Index;
        _flaggedValueCellStyle.FillPattern = FillPattern.SolidForeground;
    }

    public static ExcelFileGenerator New()
    {
        return new ExcelFileGenerator();
    }

    public ExcelFileGenerator WithSheet(string sheetName, List<string> header, List<List<object>> data)
    {
        var normalizedSheetName = WorkbookUtil.CreateSafeSheetName(sheetName);

        var sheet = _workbook.CreateSheet(normalizedSheetName);

        // Create header row
        IRow headerRow = sheet.CreateRow(0);

        for (var i = 0; i < header.Count; i++)
        {
            var cell = headerRow.CreateCell(i);
            if (header[i].Contains(ColorMarkers.Red))
            {
                cell.SetCellValue(header[i].Replace(ColorMarkers.Red, ""));
                cell.CellStyle = _flaggedAnswerCellStyle;
            }
            else
            {
                cell.SetCellValue(header[i]);
                cell.CellStyle = _headerStyle;
            }
        }

        // Create data rows
        for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
        {
            IRow dataRow = sheet.CreateRow(rowIndex + 1);
            for (int colIndex = 0; colIndex < data[rowIndex].Count; colIndex++)
            {
                var cellValue = data[rowIndex][colIndex]?.ToString();

                // an empty string cell still counts as a value in spreadsheet apps, so leave the cell out
                if (string.IsNullOrEmpty(cellValue))
                {
                    continue;
                }

                var isFlagged = cellValue.Contains(ColorMarkers.Red);
                if (isFlagged)
                {
                    cellValue = cellValue.Replace(ColorMarkers.Red, "");
                }

                var cell = dataRow.CreateCell(colIndex);
                cell.SetCellValue(cellValue);

                if (isFlagged)
                {
                    cell.CellStyle = _flaggedValueCellStyle;
                }

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
