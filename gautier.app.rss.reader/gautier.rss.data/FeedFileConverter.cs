using System.ServiceModel.Syndication;
using System.Text;

namespace gautier.rss.data
{
    public static class FeedFileConverter
    {
        private const char _Tab = '\t';

        /// <summary>
        /// Designed to generate static local files even if they are later accidentally deleted.
        /// </summary>
        public static void CreateStaticFeedFiles(string feedSaveDirectoryPath, string sqlDbConnectionString, Feed[] feedInfos)
        {
            foreach (var FeedInfo in feedInfos)
            {
                string FeedFilePath = GetFeedFilePath(feedSaveDirectoryPath, FeedInfo);

                if (File.Exists(FeedFilePath) == false)
                {
                    RSSNetClient.CreateRSSFeedFile(FeedInfo.FeedUrl, FeedFilePath);
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
            StringBuilder RSSFeedFileOutput = new();
            string NormalizedFeedFilePath = GetNormalizedFeedFilePath(feedSaveDirectoryPath, feedInfo);

            using (StreamWriter RSSFeedFile = new(NormalizedFeedFilePath, false))
            {
                foreach (var Article in feedArticles)
                {
                    string HeadlineText = Article.HeadlineText;
                    string ArticleSummary = Article.ArticleSummary;
                    string ArticleText = Article.ArticleText;
                    string ArticleDate = Article.ArticleDate;
                    string ArticleUrl = Article.ArticleUrl;

                    RSSFeedFileOutput.AppendLine($"URL{_Tab}{ArticleUrl}");
                    RSSFeedFileOutput.AppendLine($"DATE{_Tab}{ArticleDate}");
                    RSSFeedFileOutput.AppendLine($"HEAD{_Tab}{HeadlineText}");
                    RSSFeedFileOutput.AppendLine($"TEXT{_Tab}{ArticleText}");
                    RSSFeedFileOutput.AppendLine($"SUM{_Tab}{ArticleSummary}");
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
            SortedList<string, List<FeedArticle>> FeedArticles = new();

            foreach (var FeedInfo in feedInfos)
            {
                string RSSFeedFilePath = GetFeedFilePath(feedSaveDirectoryPath, FeedInfo);

                SyndicationFeed RSSFeed = RSSNetClient.CreateRSSSyndicationFeed(RSSFeedFilePath);

                if (RSSFeed.Items.Any())
                {
                    if (FeedArticles.ContainsKey(FeedInfo.FeedName) == false)
                    {
                        FeedArticles[FeedInfo.FeedName] = new();
                    }

                    List<FeedArticle> Articles = FeedArticles[FeedInfo.FeedName];

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
                        TextSyndicationContent? Content = syndicate.Content as TextSyndicationContent;

                        ArticleText = Content?.Text ?? string.Empty;
                    }
                    break;
                case "UrlSyndicationContent":
                    {
                        UrlSyndicationContent? Content = syndicate.Content as UrlSyndicationContent;

                        ArticleText = Content?.Url.AbsoluteUri ?? string.Empty;
                    }
                    break;
                case "XmlSyndicationContent":
                    {
                        XmlSyndicationContent? Content = syndicate.Content as XmlSyndicationContent;

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
