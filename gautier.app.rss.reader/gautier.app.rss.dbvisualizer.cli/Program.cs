using gautier.rss.data;
using gautier.rss.data.RSSDb;

namespace gautier.app.rss.dbvisualizer.cli
{
    internal static class Program
    {
        private static readonly string _FeedDbFilePath = @"C:\RSSFeeds\feeds_db\rss.db";

        internal static void Main(string[] args)
        {
            string SQLiteDbConnectionString = SQLUtil.GetSQLiteConnectionString(_FeedDbFilePath, 3);

            SortedList<string, Feed> RSSFeeds = FeedDataExchange.GetAllFeeds(SQLiteDbConnectionString);

            List<string> RSSFeedNames = new(RSSFeeds.Keys);

            foreach(string FeedName in RSSFeedNames)
            {
                Feed FeedEntry = RSSFeeds[FeedName];

                Console.WriteLine($"Feed:\t{FeedName}");
                Console.WriteLine($"\t\tLast Retrieved {FeedEntry.LastRetrieved} from {FeedEntry.FeedUrl}");
                Console.WriteLine($"\t\t\tConfiguration: Retrieve Limit Hrs {FeedEntry.RetrieveLimitHrs}, Retention Days {FeedEntry.RetentionDays}");
                /*
                 * Output article information.
                 */

                SortedList<string, FeedArticle> Articles = FeedDataExchange.GetFeedArticles(SQLiteDbConnectionString, FeedName);

                List<string> ArticleUrls = new(Articles.Keys);

                foreach(string ArticleUrl in ArticleUrls)
                {
                    FeedArticle Article = Articles[ArticleUrl];

                    Console.WriteLine($"URL: {ArticleUrl}");
                    Console.WriteLine($"\t\tHeadline: {Article.HeadlineText}");
                    Console.WriteLine($"\t\t\t\tText: {Article.ArticleSummary}");
                    Console.WriteLine($"\n\n\n");
                }
            }

            return;
        }
    }
}