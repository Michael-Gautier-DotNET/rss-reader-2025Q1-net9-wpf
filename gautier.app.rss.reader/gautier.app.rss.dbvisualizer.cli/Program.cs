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
                Console.WriteLine($"Feed:\t{FeedName}");

                /*
                 * Output article information.
                 */
            }

            return;
        }
    }
}