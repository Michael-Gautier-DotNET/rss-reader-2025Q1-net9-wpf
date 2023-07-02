using System.Diagnostics;

namespace gautier.rss.data;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Feed
{
    public string FeedName { get; set; } = string.Empty;
    public string FeedUrl { get; set; } = string.Empty;
    public string LastRetrieved { get; set; } = string.Empty;
    public string RetrieveLimitHrs { get; set; } = string.Empty;
    public string RetentionDays { get; set; } = string.Empty;

    private string GetDebuggerDisplay()
    {
        return $"{FeedName} {FeedUrl} {LastRetrieved} {RetrieveLimitHrs} {RetentionDays}";
    }
}