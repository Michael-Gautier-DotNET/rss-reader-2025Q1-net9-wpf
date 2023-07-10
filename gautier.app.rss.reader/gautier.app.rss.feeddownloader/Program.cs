using gautier.rss.data;
using gautier.rss.data.RSSDb;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        private static readonly string _FeedSaveDirectoryPath = @"C:\RSSFeeds\";
        private static readonly Feed[] _FeedInfos = GetStaticFeedInfos();
        private static readonly string _FeedDbFilePath = @"rss.db";

        internal static void Main(string[] args)
        {
            SetupFeedDirectory();

            FeedFileConverter.CreateStaticFeedFiles(_FeedSaveDirectoryPath, _FeedInfos);

            FeedFileConverter.TransformStaticFeedFiles(_FeedSaveDirectoryPath, _FeedInfos);

            FeedDataExchange.ImportStaticFeedFilesToDatabase(_FeedSaveDirectoryPath, _FeedInfos, _FeedDbFilePath);

            return;
        }

        private static void SetupFeedDirectory()
        {
            if (Directory.Exists(_FeedSaveDirectoryPath) == false)
            {
                Directory.CreateDirectory(_FeedSaveDirectoryPath);
            }

            return;
        }

        public static Feed[] GetStaticFeedInfos()
        {
            var FeedInfos = new Feed[]
            {
                new Feed{
                    FeedName = "Ars Technica",
                    FeedUrl = "https://feeds.arstechnica.com/arstechnica/index"
                },
                new Feed{
                    FeedName = "Slashdot",//no more than once per 30 minutes
                    FeedUrl = "http://rss.slashdot.org/Slashdot/slashdotMainatom"
                },
                new Feed{
                    FeedName = "Phoronix",
                    FeedUrl = "https://www.phoronix.com/phoronix-rss.php"
                },
                new Feed{
                    FeedName = "DistroWatch",
                    FeedUrl = "https://distrowatch.com/news/dw.xml"
                }
            };

            return FeedInfos;
        }
    }
}