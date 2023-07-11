using System.Data;
using System.Data.SQLite;

using gautier.rss.data.RSSDb;

namespace gautier.rss.data;

public static class FeedDataExchange
{
    private const char _Tab = '\t';

    public static SortedList<string, Feed> GetAllFeeds(string sqlConnectionString)
    {
        var Feeds = new SortedList<string, Feed>();

        using (SQLiteConnection SQLConn = SQLUtil.OpenSQLiteConnection(sqlConnectionString))
        {
            var FeedEntries = FeedReader.GetAllRows(SQLConn);

            foreach (var FeedEntry in FeedEntries)
            {
                Feeds[FeedEntry.FeedName] = FeedEntry;
            }
        }

        return Feeds;
    }

    public static SortedList<string, SortedList<string, FeedArticle>> GetAllFeedArticles(string sqlConnectionString)
    {
        var feeds_articles = new SortedList<string, SortedList<string, FeedArticle>>(100);

        using (var sql_conn = new SQLiteConnection(sqlConnectionString))
        {
            sql_conn.Open();

            using (var sql_com = sql_conn.CreateCommand())
            {
                sql_com.CommandType = CommandType.StoredProcedure;
                sql_com.CommandText = "get_all_feeds_articles";

                using (var col_reader = sql_com.ExecuteReader())
                {
                    var col_count = col_reader.FieldCount;

                    while (col_reader.Read())
                    {
                        var FeedName = string.Empty;
                        var HeadlineText = string.Empty;
                        var ArticleSummary = string.Empty;
                        var ArticleText = string.Empty;
                        var ArticleDate = string.Empty;
                        var ArticleUrl = string.Empty;
                        var RowInsertDateTime = string.Empty;

                        for (var col_index = 0; col_index < col_count; col_index++)
                        {
                            var col_value = $"{col_reader.GetValue(col_index)}";
                            //Assume all is text/string.
                            switch (col_index)
                            {
                                case 0: //feed_name
                                    FeedName = col_value;
                                    break;
                                case 1://headline_text
                                    HeadlineText = col_value;
                                    break;
                                case 2://article_summary
                                    ArticleSummary = col_value;
                                    break;
                                case 3://article_text
                                    ArticleText = col_value;
                                    break;
                                case 4://article_date
                                    ArticleDate = col_value;
                                    break;
                                case 5://article_url
                                    ArticleUrl = col_value;
                                    break;
                                case 6://row_insert_date_time
                                    RowInsertDateTime = col_value;
                                    break;
                            }
                        }

                        if (!feeds_articles.ContainsKey(FeedName))
                        {
                            feeds_articles[FeedName] = new SortedList<string, FeedArticle>(1000);
                        }

                        if (feeds_articles.ContainsKey(FeedName))
                        {
                            var articles = feeds_articles[FeedName];

                            if (!articles.ContainsKey(ArticleUrl))
                            {
                                FeedArticle feed_article_item = new FeedArticle
                                {
                                    FeedName = FeedName,
                                    HeadlineText = HeadlineText,
                                    ArticleSummary = ArticleSummary,
                                    ArticleText = ArticleText,
                                    ArticleDate = ArticleDate,
                                    ArticleUrl = ArticleUrl,
                                    RowInsertDateTime = RowInsertDateTime
                                };

                                articles.Add(ArticleUrl, feed_article_item);
                            }
                        }
                    }
                }
            }
        }

        return feeds_articles;
    }

    public static SortedList<string, FeedArticle> GetFeedArticles(string sqlConnectionString, string feedName)
    {
        var feeds_articles = new SortedList<string, FeedArticle>(100);

        using (var sql_conn = new SQLiteConnection(sqlConnectionString))
        {
            sql_conn.Open();

            using (var sql_com = sql_conn.CreateCommand())
            {
                sql_com.CommandType = CommandType.StoredProcedure;
                sql_com.CommandText = "get_feed_articles";

                sql_com.Parameters.AddWithValue("@feed_name", feedName);

                using (var col_reader = sql_com.ExecuteReader())
                {
                    var col_count = col_reader.FieldCount;

                    while (col_reader.Read())
                    {
                        var FeedName = string.Empty;
                        var HeadlineText = string.Empty;
                        var ArticleSummary = string.Empty;
                        var ArticleText = string.Empty;
                        var ArticleDate = string.Empty;
                        var ArticleUrl = string.Empty;
                        var RowInsertDateTime = string.Empty;

                        for (var col_index = 0; col_index < col_count; col_index++)
                        {
                            var col_value = $"{col_reader.GetValue(col_index)}";
                            //Assume all is text/string.
                            switch (col_index)
                            {
                                case 0: //feed_name
                                    FeedName = col_value;
                                    break;
                                case 1://headline_text
                                    HeadlineText = col_value;
                                    break;
                                case 2://article_summary
                                    ArticleSummary = col_value;
                                    break;
                                case 3://article_text
                                    ArticleText = col_value;
                                    break;
                                case 4://article_date
                                    ArticleDate = col_value;
                                    break;
                                case 5://article_url
                                    ArticleUrl = col_value;
                                    break;
                                case 6://row_insert_date_time
                                    RowInsertDateTime = col_value;
                                    break;
                            }
                        }

                        if (!feeds_articles.ContainsKey(ArticleUrl))
                        {
                            FeedArticle feed_article_item = new FeedArticle
                            {
                                FeedName = FeedName,
                                HeadlineText = HeadlineText,
                                ArticleSummary = ArticleSummary,
                                ArticleText = ArticleText,
                                ArticleDate = ArticleDate,
                                ArticleUrl = ArticleUrl,
                                RowInsertDateTime = RowInsertDateTime
                            };

                            feeds_articles.Add(ArticleUrl, feed_article_item);
                        }
                    }
                }
            }
        }

        return feeds_articles;
    }

    public static void ImportStaticFeedFilesToDatabase(string feedSaveDirectoryPath, string feedDbFilePath, Feed[] feedInfos)
    {
        SortedList<string, List<FeedArticleUnion>> FeedsArticles = new();

        DateTime ModificationDateTime = DateTime.Now;
        string ModificationDateTimeText = ModificationDateTime.ToString();

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
                    else if (Col1 == "SUM")
                    {
                        FeedArticlePair.ArticleDetail.ArticleSummary += Col2;
                    }

                    if (Col1 == "URL" && PreviousURL != Col2)
                    {
                        FeedArticlePair = new FeedArticleUnion
                        {
                            FeedHeader = FeedHeader,
                            ArticleDetail = new FeedArticle()
                        };

                        FeedArticlePair.ArticleDetail.RowInsertDateTime = ModificationDateTimeText;

                        if (!FeedsArticles.ContainsKey(FeedHeader.FeedName))
                        {
                            FeedsArticles.Add(FeedHeader.FeedName, new List<FeedArticleUnion>());
                        }

                        FeedsArticles[FeedHeader.FeedName].Add(FeedArticlePair);

                        PreviousURL = Col2;
                    }

                    CreateFeedArticle(FeedArticlePair, Col1, Col2);
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

    private static FeedArticleUnion CreateFeedArticle(FeedArticleUnion feedArticle, string firstColumn, string secondColumn)
    {
        switch (firstColumn)
        {
            case "URL":
                feedArticle.ArticleDetail.ArticleUrl = secondColumn;
                break;
            case "DATE":
                feedArticle.ArticleDetail.ArticleDate = secondColumn;
                break;
            case "HEAD":
                feedArticle.ArticleDetail.HeadlineText = secondColumn;
                break;
            case "TEXT":
                feedArticle.ArticleDetail.ArticleText = secondColumn;
                break;
            case "SUM":
                feedArticle.ArticleDetail.ArticleSummary = secondColumn;
                break;
            default:
                // Handle unknown firstColumn value
                break;
        }

        return feedArticle;
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

        if (Exists)
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

        if (Exists)
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

        /*Feed entries from the database.*/
        MergeValidateFeedEntries(FeedEntries, StaticFeedEntries, FeedUrls);

        /*Feed entries from the file.*/
        MergeValidateFeedEntries(StaticFeedEntries, FeedEntries, FeedUrls);

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
