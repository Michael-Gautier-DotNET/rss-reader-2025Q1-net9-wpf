using System.Data.SQLite;
using System.Globalization;

using gautier.rss.data.RSSDb;

namespace gautier.rss.data;

public static class FeedDataExchange
{
    private const char _Tab = '\t';

    private static readonly DateTimeFormatInfo _InvariantFormat = DateTimeFormatInfo.InvariantInfo;

    public static SortedList<string, Feed> GetAllFeeds(string sqlConnectionString)
    {
        SortedList<string, Feed> Feeds = new();

        using (SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(sqlConnectionString))
        {
            List<Feed> FeedEntries = FeedReader.GetAllRows(SQLConn);

            foreach (Feed FeedEntry in FeedEntries)
            {
                Feeds[FeedEntry.FeedName] = FeedEntry;
            }
        }

        return Feeds;
    }

    public static SortedList<string, SortedList<string, FeedArticle>> GetAllFeedArticles(string sqlConnectionString)
    {
        SortedList<string, SortedList<string, FeedArticle>> Articles = new(100);

        using (SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(sqlConnectionString))
        {
            List<FeedArticle> FeedArticles = FeedArticleReader.GetAllRows(SQLConn);

            foreach (FeedArticle ArticleEntry in FeedArticles)
            {
                string FeedName = ArticleEntry.FeedName;

                if (Articles.ContainsKey(FeedName) == false)
                {
                    Articles[FeedName] = new(1000);
                }

                if (Articles.ContainsKey(FeedName))
                {
                    string ArticleUrl = ArticleEntry.ArticleUrl;

                    SortedList<string, FeedArticle> ArticlesByUrl = Articles[FeedName];

                    if (ArticlesByUrl.ContainsKey(ArticleUrl) == false)
                    {
                        ArticlesByUrl.Add(ArticleUrl, ArticleEntry);
                    }
                }
            }
        }

        return Articles;
    }

    public static SortedList<string, FeedArticle> GetFeedArticles(string sqlConnectionString, string feedName)
    {
        SortedList<string, FeedArticle> Articles = new(100);

        using (SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(sqlConnectionString))
        {
            List<FeedArticle> FeedArticles = FeedArticleReader.GetRows(SQLConn, feedName);

            foreach (FeedArticle ArticleEntry in FeedArticles)
            {
                string ArticleUrl = ArticleEntry.ArticleUrl;

                if (Articles.ContainsKey(ArticleUrl) == false)
                {
                    Articles.Add(ArticleUrl, ArticleEntry);
                }
            }
        }

        return Articles;
    }

    public static void ImportStaticFeedFilesToDatabase(string feedSaveDirectoryPath, string feedDbFilePath, Feed[] feedInfos)
    {
        SortedList<string, List<FeedArticleUnion>> FeedsArticles = new();

        DateTime ModificationDateTime = DateTime.Now;
        string ModificationDateTimeText = ModificationDateTime.ToString(_InvariantFormat.UniversalSortableDateTimePattern);

        foreach (Feed FeedHeader in feedInfos)
        {
            FeedHeader.LastRetrieved = ModificationDateTimeText;
            FeedHeader.RetrieveLimitHrs = "1";
            FeedHeader.RetentionDays = "45";

            string filePath = GetNormalizedFeedFilePath(feedSaveDirectoryPath, FeedHeader);

            if (File.Exists(filePath) == false)
            {
                continue;
            }

            using (StreamReader LineReader = new(filePath))
            {
                string FileLine = string.Empty;
                string PreviousURL = string.Empty;

                FeedArticleUnion FeedArticlePair = new();

                bool InText = false;
                List<string> LineHeaders = new()
                {
                    "URL",
                    "DATE",
                    "HEAD",
                    "TEXT",
                    "SUM",
                };

                while (LineReader.EndOfStream == false && (FileLine = LineReader.ReadLine() ?? string.Empty) is not null)
                {
                    if (string.IsNullOrWhiteSpace(FileLine))
                    {
                        continue;
                    }

                    string Col1 = string.Empty;
                    string Col2 = string.Empty;

                    if (FileLine.Contains(_Tab))
                    {
                        Col1 = FileLine.Substring(0, FileLine.IndexOf(_Tab));
                        Col2 = FileLine.Substring(FileLine.IndexOf(_Tab) + 1);
                    }

                    if (LineHeaders.Contains(Col1))
                    {
                        InText = false;
                    }

                    if (Col1 == "SUM")
                    {
                        FeedArticlePair.ArticleDetail.ArticleSummary = Col2;
                    }

                    if (Col1 == "TEXT")
                    {
                        InText = true;

                        FeedArticlePair.ArticleDetail.ArticleText = Col2;
                    }

                    if (LineHeaders.Contains(Col1) == false && InText)
                    {
                        FeedArticlePair.ArticleDetail.ArticleText += FileLine;
                    }

                    if (Col1 == "URL" && PreviousURL != Col2)
                    {
                        FeedArticlePair = new FeedArticleUnion
                        {
                            FeedHeader = FeedHeader,
                            ArticleDetail = new FeedArticle
                            {
                                FeedName = FeedHeader.FeedName
                            }
                        };

                        FeedArticlePair.ArticleDetail.RowInsertDateTime = ModificationDateTimeText;

                        if (!FeedsArticles.ContainsKey(FeedHeader.FeedName))
                        {
                            FeedsArticles.Add(FeedHeader.FeedName, new List<FeedArticleUnion>());
                        }

                        FeedsArticles[FeedHeader.FeedName].Add(FeedArticlePair);

                        PreviousURL = Col2;
                    }

                    if (Col1 == "URL")
                    {
                        FeedArticlePair.ArticleDetail.ArticleUrl = Col2;
                    }

                    if (Col1 == "DATE")
                    {
                        FeedArticlePair.ArticleDetail.ArticleDate = Col2;
                    }

                    if (Col1 == "HEAD")
                    {
                        FeedArticlePair.ArticleDetail.HeadlineText = Col2;
                    }
                }
            }
        }

        WriteRSSArticlesToDatabase(feedDbFilePath, FeedsArticles);

        return;
    }

    private static string GetNormalizedFeedFilePath(string feedSaveDirectoryPath, Feed feedInfo)
    {
        return Path.Combine(feedSaveDirectoryPath, $"{feedInfo.FeedName}.txt");
    }

    public static void WriteRSSArticlesToDatabase(string feedDbFilePath, SortedList<string, List<FeedArticleUnion>> feedsArticles)
    {
        string ConnectionString = SQLUtil.GetSQLiteConnectionString(feedDbFilePath, 3);

        using SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(ConnectionString);

        var FeedNames = feedsArticles.Keys;

        SortedList<string, Feed> Feeds = CollectFeeds(feedsArticles, FeedNames);

        /*Insert or Update feeds and feeds_articles tables*/
        UpdateRSSTables(feedsArticles, SQLConn, FeedNames, Feeds);

        return;
    }

    private static SortedList<string, Feed> CollectFeeds(SortedList<string, List<FeedArticleUnion>> feedsArticles, IList<string> feedNames)
    {
        SortedList<string, Feed> Feeds = new();

        foreach (var FeedName in feedNames)
        {
            if (Feeds.ContainsKey(FeedName) == false)
            {
                var FUL = feedsArticles[FeedName];

                if (FUL.Count > 0)
                {
                    var FU = FUL[0];

                    Feeds[FeedName] = FU.FeedHeader;
                }
            }
        }

        return Feeds;
    }

    private static void UpdateRSSTables(SortedList<string, List<FeedArticleUnion>> feedsArticles, SQLiteConnection SQLConn, IList<string> FeedNames, SortedList<string, Feed> Feeds)
    {
        foreach (var FeedName in FeedNames)
        {
            var FeedHeader = Feeds[FeedName];

            List<FeedArticleUnion> FeedArticles = feedsArticles[FeedName];

            /*Insert or Update feeds table*/
            ModifyFeed(SQLConn, FeedHeader);

            foreach (var article in FeedArticles)
            {
                /*Insert or Update feeds_articles table*/
                ModifyFeedArticle(SQLConn, FeedHeader, article);
            }
        }

        return;
    }

    private static void ModifyFeedArticle(SQLiteConnection sqlConn, Feed feedHeader, FeedArticleUnion article)
    {
        bool Exists = FeedArticleReader.Exists(sqlConn, feedHeader.FeedName, article.ArticleDetail.ArticleUrl);

        if (Exists == false)
        {
            FeedArticleWriter.AddFeedArticle(sqlConn, article);
        }
        else
        {
            FeedArticleWriter.ModifyFeedArticle(sqlConn, article);
        }

        return;
    }

    private static void ModifyFeed(SQLiteConnection sqlConn, Feed feedHeader)
    {
        bool Exists = FeedReader.Exists(sqlConn, feedHeader.FeedName);

        if (Exists == false)
        {
            FeedWriter.AddFeed(sqlConn, feedHeader);
        }
        else
        {
            FeedWriter.ModifyFeed(sqlConn, feedHeader);
        }

        return;
    }

    public static Feed[] MergeFeedEntries(string feedDbFilePath, Feed[] feedEntries)
    {
        string ConnectionString = SQLUtil.GetSQLiteConnectionString(feedDbFilePath, 3);

        using SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(ConnectionString);

        List<string> FeedUrls = new();

        List<Feed> StaticFeedEntries = new(feedEntries);
        List<Feed> FeedEntries = FeedReader.GetAllRows(SQLConn);

        if (FeedEntries.Count > 0)
        {
            /*Feed entries from the database.*/
            MergeValidateFeedEntries(FeedEntries, StaticFeedEntries, FeedUrls);

            /*Feed entries from the file.*/
            MergeValidateFeedEntries(StaticFeedEntries, FeedEntries, FeedUrls);
        }
        else
        {
            FeedEntries = StaticFeedEntries;
        }

        return FeedEntries.ToArray();
    }

    private static void MergeValidateFeedEntries(List<Feed> leftSideValues, List<Feed> rightSideValues, List<string> secondKeys)
    {
        foreach (Feed LeftEntry in leftSideValues)
        {
            string SecondKey = LeftEntry.FeedUrl.ToLower();

            if (secondKeys.Contains(SecondKey))
            {
                continue;
            }
            else
            {
                secondKeys.Add(SecondKey);
            }

            /*Feed entries from the file.*/
            foreach (Feed RightSideEntry in rightSideValues)
            {
                if (LeftEntry.FeedName == RightSideEntry.FeedName)
                {
                    string RightSideSecondKey = RightSideEntry.FeedUrl.ToLower();

                    if (SecondKey != RightSideSecondKey && secondKeys.Contains(RightSideSecondKey) == false)
                    {
                        /*
                         * Give another feed the opportunity to use the url under a different feed name.
                         * I do not really support this functionality but I am annotating the concept regardless.
                         */
                        secondKeys.Remove(RightSideSecondKey);

                        /*
                         * For now, assume the url from the secondary source is more current.
                         *      In the future, 
                         *          may want to compare the 
                         *              file's last modifed time to the data record's last retrieved time.
                         */
                        LeftEntry.FeedUrl = RightSideSecondKey;
                        secondKeys.Add(RightSideSecondKey);
                    }
                }
            }
        }

        return;
    }
}
