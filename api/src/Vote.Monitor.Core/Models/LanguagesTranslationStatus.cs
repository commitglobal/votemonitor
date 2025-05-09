namespace Vote.Monitor.Core.Models;

public class LanguagesTranslationStatus : Dictionary<string, TranslationStatus>
{
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
        if (obj == null || !(obj is LanguagesTranslationStatus))
            return false;

        var other = (LanguagesTranslationStatus)obj;

        if (this.Count != other.Count)
            return false;

        foreach (var pair in this)
        {
            if (!other.ContainsKey(pair.Key) || other[pair.Key] != pair.Value)
                return false;
        }

        return true;
    }

    public void AddOrUpdateTranslationStatus(string languageCode, TranslationStatus status)
    {
        if (ContainsKey(languageCode))
        {
            this[languageCode] = status;
        }

        Add(languageCode, status);
    }
}