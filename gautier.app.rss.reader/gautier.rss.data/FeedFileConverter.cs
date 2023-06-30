using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

using gautier.rss.data.RDFConversion;

namespace gautier.rss.data
{
    public static class FeedFileConverter
    {
        /// <summary>
        /// Designed to generate static local files even if they are later accidentally deleted.
        /// </summary>
        public static void CreateStaticFeedFiles(string feedSaveDirectoryPath, Feed[] feedInfos)
        {
            foreach (var FeedInfo in feedInfos)
            {
                var FeedFilePath = GetFeedFilePath(feedSaveDirectoryPath, FeedInfo);

                if (File.Exists(FeedFilePath) == false)
                {
                    CreateRSSFeedFile(FeedInfo.FeedUrl, FeedFilePath);
                }
            }

            return;
        }

        public static void TransformStaticFeedFiles(string feedSaveDirectoryPath, Feed[] feedInfos)
        {
            SortedList<string, List<FeedArticle>> Feeds = TransformMSSyndicationToFeedArticles(feedSaveDirectoryPath, feedInfos);

            foreach (var FeedInfo in feedInfos)
            {
                if (Feeds.ContainsKey(FeedInfo.FeedName) == false)
                {
                    continue;
                }

                WriteRSSArticlesToFile(feedSaveDirectoryPath, FeedInfo, Feeds[FeedInfo.FeedName]);
            }

            return;
        }

        private static void WriteRSSArticlesToFile(string feedSaveDirectoryPath, Feed feedInfo, List<FeedArticle> feedArticles)
        {
            var RSSFeedFileOutput = new StringBuilder();
            var NormalizedFeedFilePath = GetNormalizedFeedFilePath(feedSaveDirectoryPath, feedInfo);

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

        private static SortedList<string, List<FeedArticle>> TransformMSSyndicationToFeedArticles(string feedSaveDirectoryPath, Feed[] feedInfos)
        {
            var FeedArticles = new SortedList<string, List<FeedArticle>>();

            foreach (var FeedInfo in feedInfos)
            {
                SyndicationFeed RSSFeed = GetSyndicationFeed(feedSaveDirectoryPath, FeedInfo);

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

            if (ArticleDate.Year < 0002)
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

        private static SyndicationFeed GetSyndicationFeed(string feedSaveDirectoryPath, Feed FeedInfo)
        {
            var RSSFeedFilePath = GetFeedFilePath(feedSaveDirectoryPath, FeedInfo);

            SyndicationFeed RSSFeed = new();

            if (File.Exists(RSSFeedFilePath) == true)
            {
                try
                {
                    using (var RSSXmlFile = XmlReader.Create(RSSFeedFilePath))
                    {
                        RSSFeed = SyndicationFeed.Load(RSSXmlFile);
                    }
                }
                catch (XmlException xmlE)
                {
                    bool ExceptionContainsRDF = xmlE.Message.Contains("'RDF'");
                    bool ExceptionContainsInvalidFormat = xmlE.Message.Contains("not an allowed feed format");

                    if (ExceptionContainsRDF && ExceptionContainsInvalidFormat)
                    {
                        /*
                        var RDFDoc = RDFConverter.CreateRDF(RSSFeedFilePath);

                        RSSFeed = SyndicationConverter.ConvertToSyndicationFeed(RDFDoc);
                        */

                        RSSFeed = SyndicationConverter.ConvertToSyndicationFeedUsingArgotic(RSSFeedFilePath);
                    }
                }

            }

            return RSSFeed;
        }

        private static void CreateRSSFeedFile(string feedUrl, string feedFilePath)
        {
            using (var feedXml = XmlReader.Create(feedUrl))
            {
                using (var feedXmlWriter = XmlWriter.Create(feedFilePath))
                {
                    feedXmlWriter.WriteNode(feedXml, false);
                }
            }

            return;
        }

        private static string GetFeedFilePath(string feedSaveDirectoryPath, Feed feedInfo)
        {
            return Path.Combine(feedSaveDirectoryPath, $"{feedInfo.FeedName}.xml");
        }

        private static string GetNormalizedFeedFilePath(string feedSaveDirectoryPath, Feed feedInfo)
        {
            return Path.Combine(feedSaveDirectoryPath, $"{feedInfo.FeedName}.txt");
        }
    }
}
