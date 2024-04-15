namespace Vote.Monitor.Domain;

public class Postgres
{
    // DB Functions
    public static class Functions
    {
        public static string ObjectKeys(JsonDocument @object) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");

        public static string Unnest(string[] values) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");

        /// <summary>
        /// Gets a set of unique values from given array
        /// </summary>
        /// <param name="values">Input array</param>
        /// <returns>Array without element.</returns>
        public static string[] ArrayUnique(IEnumerable<string> values) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");

        /// <summary>
        /// Removes element if exists from given array. Executes array_remove function from postgres
        /// </summary>
        /// <remarks>Search is case-sensitive</remarks>
        /// <param name="values">Input array</param>
        /// <param name="value">Element to remove</param>
        /// <returns>Array without element.</returns>
        public static string[] ArrayRemove(string[] values, string value) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");

        /// <summary>
        /// Produces result of A\B set operation using custom written postgres function called array_diff
        /// </summary>
        /// <remarks>Search is case-sensitive</remarks>
        /// <param name="arrayA">array A</param>
        /// <param name="arrayB">array B</param>
        /// <returns>All the elements that are in array A but not in array B.</returns>
        public static string[] ArrayDiff(string[] arrayA, string[] arrayB) =>
            throw new InvalidOperationException("This method is not meant to be called directly.");
    }
}
