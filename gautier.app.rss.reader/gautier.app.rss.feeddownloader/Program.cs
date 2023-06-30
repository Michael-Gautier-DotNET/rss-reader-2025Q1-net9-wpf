using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

using gautier.rss.data;

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
            SortedList<string, List<FeedArticle>> Feeds = TransformMSSyndicationToFeedArticles(feedInfos);

            foreach (var FeedInfo in feedInfos)
            {
                if (Feeds.ContainsKey(FeedInfo.FeedName) == false)
                {
                    continue;
                }

                WriteRSSArticlesToFile(FeedInfo, Feeds[FeedInfo.FeedName]);
            }

            return;
        }

        private static void WriteRSSArticlesToFile(Feed feedInfo, List<FeedArticle> feedArticles)
        {
            var RSSFeedFileOutput = new StringBuilder();
            var NormalizedFeedFilePath = GetNormalizedFeedFilePath(feedInfo);

            using (var RSSFeedFile = new StreamWriter(NormalizedFeedFilePath, false))
            {
                foreach (var Article in feedArticles)
                {
                    var HeadlineText = Article.HeadlineText;
                    var ArticleSummary = Article.ArticleSummary;
                    var ArticleText = Article.ArticleText;
                    var ArticleDate = Article.ArticleDate;
                    var ArticleUrl = Article.ArticleUrl;

                    RSSFeedFileOutput.AppendLine($"URL  {ArticleUrl}");
                    RSSFeedFileOutput.AppendLine($"DATE {ArticleDate}");
                    RSSFeedFileOutput.AppendLine($"HEAD {HeadlineText}");
                    RSSFeedFileOutput.AppendLine($"TEXT {ArticleText}");
                    RSSFeedFileOutput.AppendLine($"SUM  {ArticleSummary}");
                }

                RSSFeedFile.Write(RSSFeedFileOutput.ToString());

                RSSFeedFile.Flush();
                RSSFeedFile.Close();
            }

            RSSFeedFileOutput.Clear();

            return;
        }

        private static SortedList<string, List<FeedArticle>> TransformMSSyndicationToFeedArticles(Feed[] feedInfos)
        {
            var FeedArticles = new SortedList<string, List<FeedArticle>>();

            foreach (var FeedInfo in feedInfos)
            {
                SyndicationFeed RSSFeed = GetSyndicationFeed(FeedInfo);

                if (RSSFeed.Items.Any())
                {
                    if (FeedArticles.ContainsKey(FeedInfo.FeedName) == false)
                    {
                        FeedArticles[FeedInfo.FeedName] = new List<FeedArticle>();
                    }

                    var Articles = FeedArticles[FeedInfo.FeedName];

                    foreach (var RSSItem in RSSFeed.Items)
                    {
                        FeedArticle FeedItem = CreateRSSFeedArticle(FeedInfo, RSSItem);

                        Articles.Add(FeedItem);
                    }
                }
            }

            return FeedArticles;
        }

        private static FeedArticle CreateRSSFeedArticle(Feed feed, SyndicationItem syndicate)
        {
            string ArticleText = string.Empty;
            string ContentType = $"{syndicate.Content?.Type}";

            switch (ContentType)
            {
                case "TextSyndicationContent":
                    {
                        var Content = syndicate.Content as TextSyndicationContent;

                        ArticleText = Content?.Text ?? string.Empty;
                    }
                    break;
                case "UrlSyndicationContent":
                    {
                        var Content = syndicate.Content as UrlSyndicationContent;

                        ArticleText = Content?.Url.AbsoluteUri ?? string.Empty;
                    }
                    break;
                case "XmlSyndicationContent":
                    {
                        var Content = syndicate.Content as XmlSyndicationContent;

                        ArticleText = Content?.ToString() ?? string.Empty;
                    }
                    break;
            }

            string ArticleUrl = string.Empty;

            foreach (var Url in syndicate.Links)
            {
                ArticleUrl = $"{Url.GetAbsoluteUri()}";

                break;
            }

            DateTime ArticleDate = syndicate.PublishDate.LocalDateTime;

            if(ArticleDate.Year < 0002)
            {
                ArticleDate = syndicate.LastUpdatedTime.LocalDateTime;
            }

            string ArticleDateText = ArticleDate.ToString("MM/dd/yyyy hh:mm tt");

            return new FeedArticle
            {
                FeedName = feed.FeedName,
                HeadlineText = syndicate.Title.Text,
                ArticleSummary = syndicate.Summary.Text,
                ArticleText = ArticleText,
                ArticleDate = ArticleDateText,
                ArticleUrl = ArticleUrl,
                RowInsertDateTime = DateTime.Now.ToString()
            };
        }

        private static SyndicationFeed GetSyndicationFeed(Feed FeedInfo)
        {
            var RSSFeedFilePath = GetFeedFilePath(FeedInfo);

            SyndicationFeed RSSFeed = new();

            if (File.Exists(RSSFeedFilePath) == true)
            {
                using (var RSSXmlFile = XmlReader.Create(RSSFeedFilePath))
                {
                    RSSFeed = SyndicationFeed.Load(RSSXmlFile);
                }
            }

            return RSSFeed;
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
                }/*,
                  * 6/29/2023
                  * File format for this feed is not readable through the Syndication API
                  *    at the moment.
                  *    Generates the following result:
                  *    System.Xml.XmlException
                          HResult=0x80131940
                          Message=The element with name 'RDF' and namespace 'http://www.w3.org/1999/02/22-rdf-syntax-ns#' is not an allowed feed format.
                          Source=System.ServiceModel.Syndication
                  *     ------------------------------------------
                  *     Will come back to this file later. Commenting it out so as to move forward with 
                  *     the overall API and implementation build out.

                new Feed{
                    FeedName = "DistroWatch",
                    FeedUrl = "https://distrowatch.com/news/dw.xml"
                }*/
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