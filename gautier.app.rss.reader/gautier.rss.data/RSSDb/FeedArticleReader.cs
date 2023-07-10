using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedArticleReader
    {
        private static readonly string _TableName = "feeds_articles";
        public static string TableName => _TableName;

        public static int GetFeedArticleCount(SQLiteConnection sqlConn)
        {
            int Count = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM {_TableName};";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                Count = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static int GetFeedArticleCount(SQLiteConnection sqlConn, string feedName)
        {
            int Count = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM {_TableName} WHERE feed_name = @FeedName;";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                RowCheckSQLCmd.Parameters.AddWithValue("@FeedName", feedName);

                Count = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static int GetFeedArticleCount(SQLiteConnection sqlConn, string feedName, string articleUrl)
        {
            int Count = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM {_TableName} WHERE feed_name = @FeedName AND article_url = @ArticleUrl;";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                RowCheckSQLCmd.Parameters.AddWithValue("@FeedName", feedName);
                RowCheckSQLCmd.Parameters.AddWithValue("@ArticleUrl", articleUrl);

                Count = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return Count;
        }

        internal static void CreateFeedArticleParameters(SQLiteCommand SQLCmd, FeedArticle article, string[] columnNames)
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

        internal static string[] GetSQLFeedsArticlesColumnNames()
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
    }
}
