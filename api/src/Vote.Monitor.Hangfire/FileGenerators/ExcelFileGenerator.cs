using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Data;

namespace Vote.Monitor.Hangfire.FileGenerators;

public class ExcelFile
{
    private readonly IWorkbook _workbook;
    private readonly ICellStyle _headerStyle;

    private ExcelFile()
    {
        _workbook = new XSSFWorkbook();

        _headerStyle = _workbook.CreateCellStyle();
        var headerFont = _workbook.CreateFont();
        headerFont.IsBold = true;
        _headerStyle.SetFont(headerFont);
    }

    public static ExcelFile New()
    {
        return new ExcelFile();
    }

    public ExcelFile WithSheet(string sheetName, DataTable dataTable)
    {
        var sheet = _workbook.CreateSheet(sheetName);
        Dictionary<int, string> headers = new Dictionary<int, string>();

        #region Generating SheetRow based on datatype

        // Create header row
        IRow headerRow = sheet.CreateRow(0);
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            headers.TryAdd(i, dataTable.Columns[i].ColumnName);
            headerRow.CreateCell(i).SetCellValue(dataTable.Columns[i].ColumnName);
        }

        // Create data rows
        for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
        {
            IRow dataRow = sheet.CreateRow(rowIndex + 1);
            for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
            {
                var cell = dataRow.CreateCell(colIndex);
                var cellValue = dataTable.Rows[rowIndex][colIndex].ToString();
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
        #endregion

        #region Generating Header Cells
        var header = sheet.CreateRow(0);
        for (var i = 0; i < headers.Count; i++)
        {
            var cell = header.CreateCell(i);
            cell.SetCellValue(headers[i]);
            cell.CellStyle = _headerStyle;
        }
        #endregion

        return this;
    }
    public ExcelFile WithSheet<T>(string sheetName, List<T> exportData) where T : class
    {
        var sheet = _workbook.CreateSheet(sheetName);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

        #region Generating SheetRow based on datatype

        Dictionary<int, string> headers = new Dictionary<int, string>();

        for (int i = 0; i < exportData.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            int columnIndex = 0;
            T item = exportData[i];
        }
        #endregion

        #region Generating Header Cells
        var header = sheet.CreateRow(0);
        for (var i = 0; i < headers.Count; i++)
        {
            var cell = header.CreateCell(i);
            cell.SetCellValue(headers[i]);
            cell.CellStyle = _headerStyle;
        }
        #endregion

        return this;
    }

    public byte[] Write()
    {
        using (var memoryStream = new MemoryStream())
        {
            _workbook.Write(memoryStream);

            return memoryStream.ToArray();
        }
    }
}
