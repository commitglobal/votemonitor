namespace Vote.Monitor.Domain.Constants;

public static class CustomDBFunctions
{
    /// <summary>
    /// Gets a set of unique values from given array
    /// </summary>
    public const string ArrayUnique = "array_unique";

    public const string CreateArrayUnique = @$"
            CREATE OR REPLACE FUNCTION ""{ArrayUnique}"" (a text[]) RETURNS text[] AS $$
              SELECT ARRAY (
                SELECT DISTINCT v FROM unnest(a) AS b(v)
              )
            $$ LANGUAGE SQL;
              ";

    /// <summary>
    /// Gets a set difference between two arrays
    /// </summary>
    public const string ArrayDiff = "array_diff";

    public const string CreateArrayDiff = @$"
            CREATE OR REPLACE FUNCTION ""{ArrayDiff}""(minuend anyarray, subtrahend anyarray, out difference anyarray)
            RETURNS anyarray AS
            $$
            BEGIN
                EXECUTE 'SELECT array(select unnest($1) EXCEPT SELECT unnest($2))'
                USING minuend, subtrahend
                INTO difference;
            END;
            $$ LANGUAGE PLPGSQL RETURNS NULL ON NULL INPUT;
              ";
}
