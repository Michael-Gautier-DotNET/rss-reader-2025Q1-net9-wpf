using gautier.rss.data;
using System.ServiceModel.Syndication;
using System.Xml;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        private static readonly string _FeedSaveDirectoryPath = @"C:\RSSFeeds\";
        private static readonly Feed[] _FeedInfos = GetStaticFeedInfos();
        internal static void Main(string[] args)
        {
            SetupFeedDirectory();

            CreateStaticFeedFiles(_FeedInfos);

            TransformStaticFeedFiles(_FeedInfos);

            return;
        }


        /// <summary>
        /// Designed to generate static local files even if they are later accidentally deleted.
        /// </summary>
        private static void CreateStaticFeedFiles(Feed[] feedInfos)
        {
            foreach (var FeedInfo in feedInfos)
            {
                var FeedFilePath = GetFeedFilePath(FeedInfo);

                if (File.Exists(FeedFilePath) == false)
                {
                    CreateRSSFeedFile(FeedInfo.FeedUrl, FeedFilePath);
                }
            }

            return;
        }

        private static void TransformStaticFeedFiles(Feed[] feedInfos)
        {
            foreach (var FeedInfo in feedInfos)
            {
                var RSSFeedFilePath = GetFeedFilePath(FeedInfo);
                var NormalizedFeedFilePath = GetNormalizedFeedFilePath(FeedInfo);

                SyndicationFeed RSSFeed = new();

                if (File.Exists(RSSFeedFilePath) == true && File.Exists(NormalizedFeedFilePath) == false)
                {
                    using(var RSSXmlFile = XmlReader.Create(RSSFeedFilePath))
                    {
                        RSSFeed = SyndicationFeed.Load(RSSXmlFile);
                    }
                }

                if(RSSFeed.Items.Any())
                {
                    foreach(var RSSItem in RSSFeed.Items)
                    {
                        var FeedItem = new FeedArticle
                        {
                            FeedName = FeedInfo.FeedName,
                            HeadlineText = RSSItem.Title.Text,
                            ArticleSummary = RSSItem.Summary.Text,
                            ArticleText = $"{RSSItem.Content}",
                            ArticleDate = $"{RSSItem.PublishDate.LocalDateTime}",
                            ArticleUrl = $"{RSSItem.Links[0].GetAbsoluteUri()}",
                            RowInsertDateTime = DateTime.Now.ToString()
                        };
                    }
                }
            }

            return;
        }

        private static string GetFeedFilePath(Feed feedInfo)
        {
            return Path.Combine(_FeedSaveDirectoryPath, $"{feedInfo.FeedName}.xml");
        }

        private static string GetNormalizedFeedFilePath(Feed feedInfo)
        {
            return Path.Combine(_FeedSaveDirectoryPath, $"{feedInfo.FeedName}.txt");
        }

        private static Feed[] GetStaticFeedInfos()
        {
            Feed[] FeedInfos = new Feed[]
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

        private static void CreateRSSFeedFile(string feedUrl, string feedFilePath)
        {
            using (var feedXml = XmlReader.Create(feedUrl))
            {
                using (var feedXmlWriter = XmlWriter.Create(feedFilePath))
                {
                    feedXmlWriter.WriteNode(feedXml, true);
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