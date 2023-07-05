using System.Data;

using System.Data.SQLite;

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
        SortedList<string, List<FeedArticleUnion>> feedsArticles = new SortedList<string, List<FeedArticleUnion>>();

        foreach (Feed feedInfo in feedInfos)
        {
            string filePath = GetNormalizedFeedFilePath(feedSaveDirectoryPath, feedInfo);

            if (!File.Exists(filePath))
            {
                continue;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                FeedArticleUnion feedArticleUnion = new();
                string previousURL = string.Empty;

                Console.WriteLine($"Reading Feed {feedInfo.FeedName}");

                int counter = 0;

                while (reader.EndOfStream == false && (line = reader.ReadLine() ?? string.Empty) is not null)
                {
                    counter++;

                    Console.WriteLine($"Reading line {counter}");

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        Console.WriteLine("skip empty line");

                        continue;
                    }

                    string firstColumn = string.Empty;
                    string secondColumn = string.Empty;

                    if (line.Contains(_Tab))
                    {
                        firstColumn = line.Substring(0, line.IndexOf(_Tab));
                        secondColumn = line.Substring(line.IndexOf(_Tab) + 1);
                    }
                    else if (firstColumn == "SUM")
                    {
                        feedArticleUnion.ArticleDetail.ArticleSummary += secondColumn;
                    }

                    if (firstColumn == "URL" && previousURL != secondColumn)
                    {
                        feedArticleUnion = new FeedArticleUnion
                        {
                            FeedHeader = feedInfo,
                            ArticleDetail = new FeedArticle()
                        };

                        if (!feedsArticles.ContainsKey(feedInfo.FeedName))
                        {
                            feedsArticles.Add(feedInfo.FeedName, new List<FeedArticleUnion>());
                        }

                        feedsArticles[feedInfo.FeedName].Add(feedArticleUnion);

                        previousURL = secondColumn;
                    }

                    CreateFeedArticle(feedArticleUnion, firstColumn, secondColumn);
                }
            }
        }

        WriteRSSArticlesToDatabase(connectionString, feedsArticles);

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
        Console.WriteLine("Write RSS Articles");

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            Console.WriteLine("Open Connection");

            connection.Open();

            Console.WriteLine("Write to tables");

            foreach (var feedArticlesPair in feedsArticles)
            {
                string feedName = feedArticlesPair.Key;
                List<FeedArticleUnion> articles = feedArticlesPair.Value;

                // Check if feed exists in the feeds table
                string checkFeedCommandText = "SELECT COUNT(*) FROM feeds WHERE feed_name = @FeedName";
                using (SQLiteCommand checkFeedCommand = new SQLiteCommand(checkFeedCommandText, connection))
                {
                    checkFeedCommand.Parameters.AddWithValue("@FeedName", feedName);
                    int existingFeedCount = Convert.ToInt32(checkFeedCommand.ExecuteScalar());

                    if (existingFeedCount == 0)
                    {
                        // Insert new feed into feeds table
                        string insertFeedCommandText = "INSERT INTO feeds (feed_name, feed_url) VALUES (@FeedName, @FeedUrl)";
                        using (SQLiteCommand insertFeedCommand = new SQLiteCommand(insertFeedCommandText, connection))
                        {
                            insertFeedCommand.Parameters.AddWithValue("@FeedName", feedName);
                            insertFeedCommand.Parameters.AddWithValue("@FeedUrl", articles[0].FeedHeader.FeedUrl);
                            insertFeedCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Update last_retrieved column in feeds table
                        string updateLastRetrievedCommandText = "UPDATE feeds SET last_retrieved = @LastRetrieved WHERE feed_name = @FeedName";
                        using (SQLiteCommand updateLastRetrievedCommand = new SQLiteCommand(updateLastRetrievedCommandText, connection))
                        {
                            updateLastRetrievedCommand.Parameters.AddWithValue("@LastRetrieved", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            updateLastRetrievedCommand.Parameters.AddWithValue("@FeedName", feedName);
                            updateLastRetrievedCommand.ExecuteNonQuery();
                        }
                    }
                }

                // Insert or update articles in feeds_articles table
                foreach (var article in articles)
                {
                    string checkArticleCommandText = "SELECT COUNT(*) FROM feeds_articles WHERE feed_name = @FeedName AND article_url = @ArticleUrl";
                    using (SQLiteCommand checkArticleCommand = new SQLiteCommand(checkArticleCommandText, connection))
                    {
                        checkArticleCommand.Parameters.AddWithValue("@FeedName", feedName);
                        checkArticleCommand.Parameters.AddWithValue("@ArticleUrl", article.ArticleDetail.ArticleUrl);
                        int existingArticleCount = Convert.ToInt32(checkArticleCommand.ExecuteScalar());

                        if (existingArticleCount == 0)
                        {
                            // Insert new article into feeds_articles table
                            string insertArticleCommandText = "INSERT INTO feeds_articles (feed_name, headline_text, article_summary, article_text, article_date, article_url) " +
                                "VALUES (@FeedName, @HeadlineText, @ArticleSummary, @ArticleText, @ArticleDate, @ArticleUrl)";
                            using (SQLiteCommand insertArticleCommand = new SQLiteCommand(insertArticleCommandText, connection))
                            {
                                insertArticleCommand.Parameters.AddWithValue("@FeedName", feedName);
                                insertArticleCommand.Parameters.AddWithValue("@HeadlineText", article.ArticleDetail.HeadlineText);
                                insertArticleCommand.Parameters.AddWithValue("@ArticleSummary", article.ArticleDetail.ArticleSummary);
                                insertArticleCommand.Parameters.AddWithValue("@ArticleText", article.ArticleDetail.ArticleText);
                                insertArticleCommand.Parameters.AddWithValue("@ArticleDate", article.ArticleDetail.ArticleDate);
                                insertArticleCommand.Parameters.AddWithValue("@ArticleUrl", article.ArticleDetail.ArticleUrl);
                                insertArticleCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Update existing article in feeds_articles table
                            string updateArticleCommandText = "UPDATE feeds_articles SET headline_text = @HeadlineText, article_summary = @ArticleSummary, " +
                                "article_text = @ArticleText, article_date = @ArticleDate WHERE feed_name = @FeedName AND article_url = @ArticleUrl";
                            using (SQLiteCommand updateArticleCommand = new SQLiteCommand(updateArticleCommandText, connection))
                            {
                                updateArticleCommand.Parameters.AddWithValue("@HeadlineText", article.ArticleDetail.HeadlineText);
                                updateArticleCommand.Parameters.AddWithValue("@ArticleSummary", article.ArticleDetail.ArticleSummary);
                                updateArticleCommand.Parameters.AddWithValue("@ArticleText", article.ArticleDetail.ArticleText);
                                updateArticleCommand.Parameters.AddWithValue("@ArticleDate", article.ArticleDetail.ArticleDate);
                                updateArticleCommand.Parameters.AddWithValue("@FeedName", feedName);
                                updateArticleCommand.Parameters.AddWithValue("@ArticleUrl", article.ArticleDetail.ArticleUrl);
                                updateArticleCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        return;
    }

}
