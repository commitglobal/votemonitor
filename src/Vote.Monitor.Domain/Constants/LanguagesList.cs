using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Domain.Constants;

public record LanguageDetails
{
    public LanguageDetails(string name, string iso1, string iso3, char languageType)
    {
        Id = iso3.ToGuid();
        Name = name;
        Iso1 = iso1;
        Iso3 = iso3;
        LanguageType = languageType;
    }

    public Guid Id { get; }

    /// <summary>
    /// English language name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public string Iso1 { get; }

    /// <summary>
    /// Three-letter language code (ISO 639-3)
    /// </summary>
    public string Iso3 { get; }

    /// <summary>
    /// Language type: Living, Extinct, Ancient, Historical, Constructed (only the initial character)
    /// </summary>
    public char LanguageType { get; }

    public Language ToEntity()
    {
        return new Language(Name, Iso1, Iso3, LanguageType);
    }
}

public static class LanguagesList
{
    private static List<LanguageDetails>? _languageDetailsList;

    /// <summary>
    /// Cached list of languages from https://iso639-3.sil.org/
    /// </summary>
    private static IEnumerable<LanguageDetails> GetAll()
    {
        if (_languageDetailsList != null)
        {
            return _languageDetailsList;
        }

        _languageDetailsList = new List<LanguageDetails>();

        using (StreamReader reader = new StreamReader($"./Resources/languages-iso-639-3_20230123.tab"))
        {
            string? header = reader.ReadLine();
            Dictionary<string, int> headerMap = new Dictionary<string, int>();
            if (header != null)
            {
                string[] headerColumns = header.Split('\t');
                for (int i = 0; i < headerColumns.Length; i++)
                {
                    headerMap[headerColumns[i]] = i;
                }
            }

            string? line;
            while ((line = reader.ReadLine()) != null && line != "")
            {
                string[] columns = line.Split('\t');

                char languageType = columns[headerMap["Language_Type"]][0];
                if (new List<char> { 'E', 'A', 'H' }.Contains(languageType)) continue; // Skip Extinct, Ancient, Historical
                
                LanguageDetails languageData = new LanguageDetails(
                    name: columns[headerMap["Ref_Name"]],
                    iso1: columns[headerMap["Part1"]],
                    iso3: columns[headerMap["Id"]],
                    languageType
                );

                _languageDetailsList.Add(languageData);
            }
        }

        return _languageDetailsList;
    }

    public static IEnumerable<LanguageDetails> GetAllIso1() {
        return GetAll().Where(x => x.Iso1 != "");
    }

    public static IEnumerable<LanguageDetails> GetAllIso3() {
        return GetAll();
    }
}
