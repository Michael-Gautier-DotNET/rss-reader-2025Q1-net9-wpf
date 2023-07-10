using System.Data;
using System.Data.SQLite;
using System.Text;

using gautier.rss.data.RSSDb;

namespace gautier.rss.data;

public static class FeedDataExchange
{
    private const char _Tab = '\t';

    public static SortedList<string, Feed> GetAllFeeds(string sqlConnectionString)
    {
        var feeds = new SortedList<string, Feed>();

        using (var sql_conn = new SQLiteConnection(sqlConnectionString))
        {
            sql_conn.Open();

            using (var sql_com = sql_conn.CreateCommand())
            {
                sql_com.CommandType = CommandType.StoredProcedure;
                sql_com.CommandText = "get_all_feeds";

                using (var col_reader = sql_com.ExecuteReader())
                {
                    var col_count = col_reader.FieldCount;

                    while (col_reader.Read())
                    {
                        var FeedName = string.Empty;
                        var FeedUrl = string.Empty;
                        var LastRetrieved = string.Empty;
                        var RetrieveLimitHrs = string.Empty;
                        var RetentionDays = string.Empty;

                        for (var col_index = 0; col_index < col_count; col_index++)
                        {
                            var col_value = col_reader.GetString(col_index);
                            //Assume all is text/string.
                            switch (col_index)
                            {
                                case 0: //feed_name
                                    FeedName = col_value;
                                    break;
                                case 1://feed_url
                                    FeedUrl = col_value;
                                    break;
                                case 2://last_retrieved
                                    LastRetrieved = col_value;
                                    break;
                                case 3://retrieve_limit_hrs
                                    RetrieveLimitHrs = col_value;
                                    break;
                                case 4://retention_days
                                    RetentionDays = col_value;
                                    break;
                            }
                        }

                        if (!feeds.ContainsKey(FeedName))
                        {
                            Feed feed_item = new Feed
                            {
                                FeedName = FeedName,
                                FeedUrl = FeedUrl,
                                LastRetrieved = LastRetrieved,
                                RetrieveLimitHrs = RetrieveLimitHrs,
                                RetentionDays = RetentionDays
                            };

                            feeds.Add(FeedName, feed_item);
                        }
                    }
                }
            }
        }

        return feeds;
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

    public static void ImportStaticFeedFilesToDatabase(string feedSaveDirectoryPath, Feed[] feedInfos, string connectionString)
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

        WriteRSSArticlesToDatabase(connectionString, FeedsArticles);

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

    public static void WriteRSSArticlesToDatabase(string connectionString, SortedList<string, List<FeedArticleUnion>> feedsArticles)
    {
        string[] FeedColumnNames = GetSQLFeedsColumnNames();

        string[] FeedArticleColumnNames = GetSQLFeedsArticlesColumnNames();

        using (SQLiteConnection SQLConn = new(connectionString))
        {
            SQLConn.Open();

            var FeedNames = feedsArticles.Keys;

            SortedList<string, Feed> Feeds = new();

            foreach (var FeedName in FeedNames)
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

            foreach (var FeedName in FeedNames)
            {
                var FeedHeader = Feeds[FeedName];

                List<FeedArticleUnion> FeedArticles = feedsArticles[FeedName];

                ModifyFeed(SQLConn, FeedHeader, FeedColumnNames);

                // Insert or update articles in feeds_articles table
                foreach (var article in FeedArticles)
                {
                    ModifyFeedArticle(SQLConn, FeedHeader, article, FeedArticleColumnNames);
                }
            }
        }

        return;
    }

    private static string[] GetSQLFeedsArticlesColumnNames()
    {
        return new string[]
        {
            "feed_name",
            "headline_text",
            "article_summary",
            "article_text",
            "article_date",
            "article_url",
            "row_insert_date_time"
        };
    }

    private static string[] GetSQLFeedsColumnNames()
    {
        return new string[]
        {
            "feed_name",
            "feed_url",
            "last_retrieved",
            "retrieve_limit_hrs",
            "retention_days"
        };
    }

    private static void ModifyFeedArticle(SQLiteConnection sqlConn, Feed feedHeader, FeedArticleUnion article, string[] columnNames)
    {
        const string TableName = "feeds_articles";

        int RowCount = FeedArticleReader.GetFeedArticleCount(sqlConn, feedHeader.FeedName, article.ArticleDetail.ArticleUrl);

        if (RowCount == 0)
        {
            // Insert new article into feeds_articles table
            StringBuilder CommandText = CreateSQLInsertCMDText(TableName, columnNames);

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                CreateFeedArticleParameters(SQLCmd, article.ArticleDetail, columnNames);

                SQLCmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Update existing article in feeds_articles table
            StringBuilder CommandText = CreateSQLUpdateCMDText(TableName, columnNames);
            CommandText.Append("feed_name = @feed_name AND article_url = @article_url;");

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                CreateFeedArticleParameters(SQLCmd, article.ArticleDetail, columnNames);

                SQLCmd.ExecuteNonQuery();
            }
        }

        return;
    }

    private static void ModifyFeed(SQLiteConnection sqlConn, Feed feedHeader, string[] columnNames)
    {
        const string TableName = "feeds";

        int RowCount = FeedReader.GetFeedCount(sqlConn, feedHeader.FeedName);

        if (RowCount == 0)
        {
            StringBuilder CommandText = CreateSQLInsertCMDText(TableName, columnNames);

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                CreateFeedParameters(SQLCmd, feedHeader, columnNames);

                SQLCmd.ExecuteNonQuery();
            }
        }
        else
        {
            // Update last_retrieved column in feeds table
            StringBuilder CommandText = CreateSQLUpdateCMDText(TableName, columnNames);
            CommandText.Append("feed_name = @feed_name;");

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                CreateFeedParameters(SQLCmd, feedHeader, columnNames);

                SQLCmd.ExecuteNonQuery();
            }
        }

        return;
    }

    private static void CreateFeedParameters(SQLiteCommand SQLCmd, Feed FeedHeader, string[] columnNames)
    {
        foreach (var ColumnName in columnNames)
        {
            string ParamName = $"@{ColumnName}";
            string ParamValue = string.Empty;

            switch (ColumnName)
            {
                case "feed_name":
                    ParamValue = $"{FeedHeader.FeedName}";
                    break;
                case "feed_url":
                    ParamValue = $"{FeedHeader.FeedUrl}";
                    break;
                case "last_retrieved":
                    ParamValue = $"{FeedHeader.LastRetrieved}";
                    break;
                case "retrieve_limit_hrs":
                    ParamValue = $"{FeedHeader.RetrieveLimitHrs}";
                    break;
                case "retention_days":
                    ParamValue = $"{FeedHeader.RetentionDays}";
                    break;
            }

            SQLCmd.Parameters.AddWithValue(ParamName, ParamValue);
        }

        return;
    }

    private static void CreateFeedArticleParameters(SQLiteCommand SQLCmd, FeedArticle article, string[] columnNames)
    {
        foreach (var ColumnName in columnNames)
        {
            string ParamName = $"@{ColumnName}";
            string ParamValue = string.Empty;

            switch (ColumnName)
            {
                case "feed_name":
                    ParamValue = $"{article.FeedName}";
                    break;
                case "headline_text":
                    ParamValue = $"{article.HeadlineText}";
                    break;
                case "article_summary":
                    ParamValue = $"{article.ArticleSummary}";
                    break;
                case "article_text":
                    ParamValue = $"{article.ArticleText}";
                    break;
                case "article_date":
                    ParamValue = $"{article.ArticleDate}";
                    break;
                case "article_url":
                    ParamValue = $"{article.ArticleUrl}";
                    break;
                case "row_insert_date_time":
                    ParamValue = $"{article.RowInsertDateTime}";
                    break;
            }

            SQLCmd.Parameters.AddWithValue(ParamName, ParamValue);
        }

        return;
    }

    private static StringBuilder CreateSQLInsertCMDText(string tableName, string[] columnNames)
    {
        StringBuilder ColumnNameSB = new();
        ColumnNameSB.AppendLine($"INSERT INTO {tableName} (");

        StringBuilder ColumnValuesSB = new();

        for (int ColI = 0; ColI < columnNames.Length; ColI++)
        {
            var ColumnName = columnNames[ColI];
            string ParamName = $"@{ColumnName}";

            string Sep = ",";

            if (ColI + 1 == columnNames.Length)
            {
                Sep = string.Empty;
            }

            ColumnNameSB.AppendLine($"{ColumnName}{Sep}");
            ColumnValuesSB.AppendLine($"{ParamName}{Sep}");
        }

        ColumnNameSB.AppendLine(") VALUES (");
        ColumnValuesSB.AppendLine(");");

        StringBuilder CommandText = new();

        CommandText.Append($"{ColumnNameSB}{ColumnValuesSB};");

        return CommandText;
    }

    private static StringBuilder CreateSQLUpdateCMDText(string tableName, string[] columnNames)
    {
        StringBuilder ColumnNameSB = new();
        ColumnNameSB.AppendLine($"UPDATE {tableName} SET ");

        for (int ColI = 0; ColI < columnNames.Length; ColI++)
        {
            var ColumnName = columnNames[ColI];
            string ParamName = $"@{ColumnName}";

            string Sep = ",";

            if (ColI + 1 == columnNames.Length)
            {
                Sep = string.Empty;
            }

            ColumnNameSB.AppendLine($"{ColumnName} = {ParamName}{Sep}");
        }

        ColumnNameSB.AppendLine(" WHERE ");

        StringBuilder CommandText = new();

        CommandText.Append($"{ColumnNameSB}");

        return CommandText;
    }
}
