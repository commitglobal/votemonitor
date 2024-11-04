namespace Vote.Monitor.Core.Models;

public class TranslatedString : Dictionary<string, string>
{
    public TranslatedString()
    {
        
    }
    
    public TranslatedString(TranslatedString? source)
    {
        if (source == null)
        {
            return;
        }

        foreach (var translation in source)
        {
            this[translation.Key] = translation.Value;
        }
    }
    
    
    // Override GetHashCode method
    public override int GetHashCode()
    {
        int hashCode = 17;
        foreach (var pair in this)
        {
            hashCode = hashCode * 23 + pair.Key.GetHashCode();
            hashCode = hashCode * 23 + pair.Value.GetHashCode();
        }

        return hashCode;
    }

    // Override Equals method
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is TranslatedString))
            return false;

        var other = (TranslatedString)obj;

        if (this.Count != other.Count)
            return false;

        foreach (var pair in this)
        {
            if (!other.ContainsKey(pair.Key) || other[pair.Key] != pair.Value)
                return false;
        }

        return true;
    }

    public void AddTranslation(string languageCode)
    {
        if (ContainsKey(languageCode))
        {
            return;
        }

        Add(languageCode, string.Empty);
    }

    public void RemoveTranslation(string languageCode)
    {
        if (ContainsKey(languageCode))
        {
            Remove(languageCode);
        }
    }

    public static TranslatedString New(IEnumerable<string> languages, string value)
    {
        var languagesArray = languages.ToArray();

        var translatedString = new TranslatedString();
        foreach (var language in languagesArray)
        {
            translatedString.Add(language, value);
        }

        return translatedString;
    }

    public TranslatedString TrimTranslations(IEnumerable<string> languages)
    {
        var translationsToRemove = Keys.Where(x => !languages.Contains(x));

        foreach (var code in translationsToRemove)
        {
            RemoveTranslation(code);
        }

        return this;
    }
}
