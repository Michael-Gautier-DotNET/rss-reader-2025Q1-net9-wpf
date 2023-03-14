using System.Diagnostics;

namespace gautier.rss.data;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct Feed(string FeedName, string FeedUrl, string LastRetrieved, string RetrieveLimitHrs, string RetentionDays)
{
    private string GetDebuggerDisplay()
    {
        return $"{FeedName} {FeedUrl} {LastRetrieved} {RetrieveLimitHrs} {RetentionDays}";
    }
}