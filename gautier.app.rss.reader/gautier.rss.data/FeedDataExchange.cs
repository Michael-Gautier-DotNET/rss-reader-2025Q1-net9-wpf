using System.Data;

using System.Data.SQLite;

namespace gautier.rss.data;

public static class FeedDataExchange
{
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
                            Feed feed_item = new(FeedName, FeedUrl, LastRetrieved, RetrieveLimitHrs, RetentionDays);

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
                                FeedArticle feed_article_item = new(FeedName, HeadlineText, ArticleSummary, ArticleText, ArticleDate, ArticleUrl, RowInsertDateTime);

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
                            FeedArticle feed_article_item = new(FeedName, HeadlineText, ArticleSummary, ArticleText, ArticleDate, ArticleUrl, RowInsertDateTime);

                            feeds_articles.Add(ArticleUrl, feed_article_item);
                        }
                    }
                }
            }
        }

        return feeds_articles;
    }
}
