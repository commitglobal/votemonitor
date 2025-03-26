using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MatrixQuestionTests
{
    private readonly string _defaultLanguageCode = LanguagesList.EN.Iso1;
    private readonly string _languageCode = LanguagesList.RO.Iso1;
    private readonly MatrixOption _translatedOption;
    private readonly MatrixRow _translatedRow;
    
    public MatrixQuestionTests()
    {
        _translatedOption = MatrixOption.Create(Guid.NewGuid(), new TranslatedString
        {
            [_defaultLanguageCode] = "some option",
            [_languageCode] = "some option translated"
        },
            false);
        
        _translatedRow = MatrixRow.Create(Guid.NewGuid(), new TranslatedString
            {
                [_defaultLanguageCode] = "some option",
                [_languageCode] = "some option translated"
            });
    }

    private TranslatedString CreateTranslatedString(string value)
    {
        return new TranslatedString
        {
            [_defaultLanguageCode] = "some text for default language",
            [_languageCode] = value
        };
    }
    
    [Fact]
    public void ComparingToAMatrixQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
        };

        var helptext = new TranslatedString
        {
            {"EN", "other text"}
        };

        MatrixOption[] options = [.. new MatrixOptionFaker().Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker().Generate(3)];

        var id = Guid.NewGuid();
        var matrixQuestion1 = MatrixQuestion.Create(
            id, "C!", text, helptext,null,options,rows);
        var matrixQuestion2 = matrixQuestion1.DeepClone();

        // Act
        var result = matrixQuestion1 == matrixQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToAMatrixQuestion_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var text1 = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
        };

        var text2 = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
        };

        var helptext1 = new TranslatedString
        {
            {_defaultLanguageCode, "other text"}
        };

        var helptext2 = new TranslatedString
        {
            {_defaultLanguageCode, "other different"}
        };

        MatrixOption[] options1 = [.. new MatrixOptionFaker().Generate(3)];
        MatrixOption[] options2 = [.. new MatrixOptionFaker().Generate(3)];
        
        MatrixRow[] rows1 = [.. new MatrixRowFaker().Generate(3)];
        MatrixRow[] rows2 = [.. new MatrixRowFaker().Generate(3)];


        var id = Guid.NewGuid();

        var textQuestion1 = MatrixQuestion.Create(id, "C!", text1,null,null, options1,rows1);
        var textQuestion2 = MatrixQuestion.Create(id, "C!", text2, null,null, options2, rows2);

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
