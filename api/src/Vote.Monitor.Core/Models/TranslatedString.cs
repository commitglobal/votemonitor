namespace Vote.Monitor.Core.Models;

public class TranslatedString : Dictionary<string, string>
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
}