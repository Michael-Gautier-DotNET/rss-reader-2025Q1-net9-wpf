using gautier.rss.data;
using System.Xml;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        private static readonly string _FeedSaveDirectoryPath = @"C:\RSSFeeds\";
        internal static void Main(string[] args)
        {
            SetupFeedDirectory();

            CreateStaticFeedFiles();

            return;
        }

        /// <summary>
        /// Designed to generate static local files even if they are later accidentally deleted.
        /// </summary>
        private static void CreateStaticFeedFiles()
        {
            List<Feed> FeedInfos = new List<Feed>
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

            foreach (var FeedInfo in FeedInfos)
            {
                var FeedFilePath = Path.Combine(_FeedSaveDirectoryPath, $"{FeedInfo.FeedName}.xml");

                if (File.Exists(FeedFilePath) == false)
                {
                    using (var feedXml = XmlReader.Create(FeedInfo.FeedUrl))
                    {
                        using (var feedXmlWriter = XmlWriter.Create(FeedFilePath))
                        {
                            feedXmlWriter.WriteNode(feedXml, true);
                        }
                    }
                }
            }

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
    }
}