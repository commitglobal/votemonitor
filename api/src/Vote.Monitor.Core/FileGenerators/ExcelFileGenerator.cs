﻿using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Vote.Monitor.Core.FileGenerators;

public class ExcelFileGenerator
{
    private readonly IWorkbook _workbook;
    private readonly ICellStyle _headerStyle;
    private readonly ICellStyle _flaggedAnswerCellStyle;

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
    }

    public static ExcelFileGenerator New()
    {
        return new ExcelFileGenerator();
    }

    public ExcelFileGenerator WithSheet(string sheetName, List<string> header, List<List<object>> data)
    {
        var normalizedSheetName = sheetName.Length > 31 ? sheetName.Substring(0, 31 - 3) + "..." : sheetName;

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
