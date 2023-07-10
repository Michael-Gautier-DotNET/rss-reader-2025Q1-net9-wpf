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
            Feed[] FeedEntries = FeedFileConverter.GetStaticFeedInfos(_FeedNamesFilePath);

            FeedFileConverter.CreateStaticFeedFiles(_FeedSaveDirectoryPath, _FeedDbFilePath, FeedEntries);

            FeedFileConverter.TransformStaticFeedFiles(_FeedSaveDirectoryPath, FeedEntries);

            FeedDataExchange.ImportStaticFeedFilesToDatabase(_FeedSaveDirectoryPath, _FeedDbFilePath, FeedEntries);

            return;
        }
    }
}