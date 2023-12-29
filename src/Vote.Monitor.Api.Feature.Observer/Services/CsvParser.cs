using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace Vote.Monitor.Api.Feature.Observer.Services;



public class CsvParser<T, TMapper> : ICsvParser<T> where T : class
                                                            where TMapper : ClassMap<T>

{
    private readonly Validator<T> _modelValidator;
    private readonly ILogger _logger;

    public CsvParser(
        ILogger<CsvParser<T, TMapper>> logger,
        Validator<T> modelValidator
        )
    {
        _logger = logger;
        _modelValidator = modelValidator;

    }

    public ParsingResult<T> Parse(Stream stream)
    {
        List<CsvRowParsed<T>> rowsRead = new List<CsvRowParsed<T>>();

        bool anyError = false;

        using var reader = new StreamReader(stream);

        var readerConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ExceptionMessagesContainRawData = false,
            MissingFieldFound = null
        };

        using var csv = new CsvReader(reader, readerConfiguration);
        csv.Context.RegisterClassMap<TMapper>();

        if (CsvIsEmpty(csv)) return new ParsingResult<T>.Fail("Cannot parse the file or file empty.");


        if (HeaderIsValid(csv, out CsvRowParsed<T> header, out ParsingResult<T> result))
        {
            rowsRead.Add(header);
        }
        else
            return result;


        while (csv.Read())
        {

            CsvRowParsed<T> row = ToCsvRowParsed(csv.GetRecord<T>()!, _modelValidator, csv.Parser.RawRecord);
            anyError = anyError || !row.IsSuccess;
            rowsRead.Add(row);
        }

        if (anyError || rowsRead.CheckAndSetDuplicatesLines())
        {
            return new ParsingResult<T>.Fail(rowsRead);
        }
        return new ParsingResult<T>.Success(rowsRead.Where(x => x.Value != null).Select(x => x.Value!));
    }


    private CsvRowParsed<T> HeaderToCsvRow(CsvReader csv) => new CsvRowParsed<T>()
    {
        IsSuccess = true,
        OriginalRow = string.Join(",", csv.HeaderRecord!),
        Value = null
    };

    private bool HeaderIsValid(CsvReader csv, out CsvRowParsed<T> header, out ParsingResult<T> result)
    {
        csv.ReadHeader();
        header = HeaderToCsvRow(csv);

        try
        {

            csv.ValidateHeader<T>();
            result = new ParsingResult<T>.Success(new List<T>());
            return true;
        }
        catch (HeaderValidationException ex)
        {
            _logger.LogError(ex, "Cannot parse the header!");
            header.IsSuccess = false;
            header.ErrorMessage = "Cannot parse the header!";
            result = new ParsingResult<T>.Fail(header);
            return false;
        }
    }

    private bool CsvIsEmpty(CsvReader? csv)
    {
        if (csv == null || !csv.Read())
        {
            return true;
        }
        return false;
    }


    private static CsvRowParsed<T> ToCsvRowParsed(T item, Validator<T> validator, string originalString)
    {
        var validationResult = validator.Validate(item);

        return new CsvRowParsed<T>()
        {
            IsSuccess = validationResult.IsValid,
            OriginalRow = originalString,
            Value = item,
            ErrorMessage = validationResult.ToString(",")
        };
    }





}




