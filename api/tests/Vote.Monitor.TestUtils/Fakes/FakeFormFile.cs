using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Vote.Monitor.TestUtils.Fakes;

public class FakeFormFile
{
    private string _filename = "file.fake";
    private long _fileSizeInBits = 999;

    public static FakeFormFile New() => new();

    public FakeFormFile HavingFileName(string filename)
    {
        _filename = filename;
        return this;
    }

    public FakeFormFile HavingLength(long fileSizeInBits)
    {
        _fileSizeInBits = fileSizeInBits;
        return this;
    }

    public IFormFile Please()
    {
        var formFile = Substitute.For<IFormFile>();

        formFile.FileName.Returns(_filename);
        formFile.Length.Returns(_fileSizeInBits);

        return formFile;
    }

    private FakeFormFile()
    {
    }
}
