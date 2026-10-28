using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Vote.Monitor.Core.FileGenerators;

namespace Vote.Monitor.Core.UnitTests;

public class ExcelFileGeneratorTests
{
    [Fact]
    public void ShouldLeaveOutCells_WhenValueIsEmpty()
    {
        var file = ExcelFileGenerator.New()
            .WithSheet("Sheet", ["A", "B", "C", "D"], [["value", "", null!, 0]])
            .Please();

        using var workbook = new XSSFWorkbook(new MemoryStream(Convert.FromBase64String(file)));
        var row = workbook.GetSheetAt(0).GetRow(1);

        row.GetCell(0).StringCellValue.Should().Be("value");
        row.GetCell(1).Should().BeNull();
        row.GetCell(2).Should().BeNull();
        row.GetCell(3).StringCellValue.Should().Be("0");
    }

    [Fact]
    public void ShouldFillCell_WhenValueIsMarkedFlagged()
    {
        var file = ExcelFileGenerator.New()
            .WithSheet("Sheet", ["A", "B"], [[ColorMarkers.Red + "Yes", "No"]])
            .Please();

        using var workbook = new XSSFWorkbook(new MemoryStream(Convert.FromBase64String(file)));
        var row = workbook.GetSheetAt(0).GetRow(1);

        row.GetCell(0).StringCellValue.Should().Be("Yes");
        row.GetCell(0).CellStyle.FillPattern.Should().Be(FillPattern.SolidForeground);
        row.GetCell(0).CellStyle.FillForegroundColor.Should().Be(IndexedColors.Rose.Index);
        row.GetCell(1).CellStyle.FillPattern.Should().Be(FillPattern.NoFill);
    }
}
