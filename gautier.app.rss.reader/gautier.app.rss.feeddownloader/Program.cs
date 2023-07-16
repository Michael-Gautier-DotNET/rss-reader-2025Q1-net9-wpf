using gautier.rss.data;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        private static readonly string _FeedSaveDirectoryPath = @"C:\RSSFeeds\";
        private static readonly string _FeedNamesFilePath = @"rss_feed_sources.txt";
        private static readonly string _FeedDbFilePath = @"rss.db";

        internal static void Main(string[] args)
        {
            Console.WriteLine($"Gautier RSS Feed Downloader {DateTime.Now}");
            Console.WriteLine($"Started {DateTime.Now}");
            Console.WriteLine($"Feeds Saved to {_FeedSaveDirectoryPath}");
            Console.WriteLine($"SQLite database: {_FeedDbFilePath}");
            Console.WriteLine($"RSS Feed Configuration Path: {_FeedNamesFilePath}");

            Feed[] FeedEntries = FeedFileConverter.GetStaticFeedInfos(_FeedNamesFilePath);

            FeedEntries = FeedDataExchange.MergeFeedEntries(_FeedDbFilePath, FeedEntries);

            Console.WriteLine($"Processing {FeedEntries.Length} Feeds");

            foreach (Feed FeedEntry in FeedEntries)
            {
                Console.WriteLine($"Feed:\t{FeedEntry.FeedName}");
                Console.WriteLine($"\tLast Retrieved: {FeedEntry.LastRetrieved}");
                Console.WriteLine($"\t\t {FeedEntry.FeedUrl}");
                Console.WriteLine($"\t\t\t DbId: {FeedEntry.DbId}");
                Console.WriteLine($"\t\t\t Updated: {FeedEntry.RowInsertDateTime}");
                Console.WriteLine($"\t\t\t Retrieve Limit Hrs: {FeedEntry.RetrieveLimitHrs}");
                Console.WriteLine($"\t\t\t Retention Days: {FeedEntry.RetentionDays}");
            }

            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine("1]\tIMPLICIT DOWNLOAD + CREATE STATIC FEED FILES");

            FeedFileConverter.CreateStaticFeedFiles(_FeedSaveDirectoryPath, _FeedDbFilePath, FeedEntries);

            Console.WriteLine("2]\tCONVERT FROM XML TO TAB DELIMITED FORMAT");

            FeedFileConverter.TransformStaticFeedFiles(_FeedSaveDirectoryPath, FeedEntries);

            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine("3]\tDATABASE IMPORT | TAB DELIMITED INTEGRATION TO SQLite");

            FeedDataExchange.ImportStaticFeedFilesToDatabase(_FeedSaveDirectoryPath, _FeedDbFilePath, FeedEntries);

            Console.WriteLine($"Ended {DateTime.Now}");

            return;
        }
    }
}