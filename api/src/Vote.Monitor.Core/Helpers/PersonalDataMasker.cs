namespace Vote.Monitor.Core.Helpers;

/// <summary>
/// Masks observer personal data for platform admins: keeps the first and last character of each part.
/// </summary>
public static class PersonalDataMasker
{
    private const char MaskChar = '*';

    public static string MaskName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        return string.Join(' ', name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(MaskWord));
    }

    public static string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        var atIndex = email.LastIndexOf('@');
        if (atIndex <= 0)
        {
            return MaskWord(email);
        }

        return MaskWord(email[..atIndex]) + email[atIndex..];
    }

    public static string MaskPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return string.Empty;
        }

        const int visibleDigits = 2;
        if (phoneNumber.Length <= visibleDigits)
        {
            return new string(MaskChar, phoneNumber.Length);
        }

        return new string(MaskChar, phoneNumber.Length - visibleDigits) + phoneNumber[^visibleDigits..];
    }

    private static string MaskWord(string word) => word.Length switch
    {
        0 => string.Empty,
        1 => MaskChar.ToString(),
        2 => $"{word[0]}{MaskChar}",
        _ => $"{word[0]}{new string(MaskChar, word.Length - 2)}{word[^1]}"
    };
}
