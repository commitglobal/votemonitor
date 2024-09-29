namespace SubmissionsFaker;

public static class StringExtensions
{
    public static string OfLength(this string input, int length)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // If the input string length is less than the desired length, return the full string
        return input.Length <= length ? input : input.Substring(0, length);
    }
}