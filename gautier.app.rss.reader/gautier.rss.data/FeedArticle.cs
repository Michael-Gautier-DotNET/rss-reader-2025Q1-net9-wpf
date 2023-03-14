using System.Diagnostics;

namespace gautier.rss.data;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct FeedArticle(string FeedName, string HeadlineText, string ArticleSummary, string ArticleText, string ArticleDate, string ArticleUrl, string RowInsertDateTime)
{
    private string GetDebuggerDisplay()
    {
        return $"{FeedName} {HeadlineText} {ArticleUrl}";
    }
}