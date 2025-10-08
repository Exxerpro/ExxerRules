namespace IndTrace.Aggregation.BoundedTests.Mapper;

public static class LevenshteinMetrics
{
    public static int Levenshtein(string s, string t)
    {
        var dp = new int[s.Length + 1, t.Length + 1];
        for (var i = 0; i <= s.Length; i++) dp[i, 0] = i;
        for (var j = 0; j <= t.Length; j++) dp[0, j] = j;

        for (var i = 1; i <= s.Length; i++)
            for (var j = 1; j <= t.Length; j++)
            {
                var cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                dp[i, j] = Math.Min(Math.Min(
                        dp[i - 1, j] + 1,
                        dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost);
            }

        return dp[s.Length, t.Length];
    }
}
