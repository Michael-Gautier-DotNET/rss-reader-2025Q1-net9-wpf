using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedArticleReader
    {
        private static readonly string _TableName = "feeds_articles";
        public static string TableName => _TableName;

        public static string[] TableColumnNames => new string[]
        {
            "feed_name",
            "headline_text",
            "article_summary",
            "article_text",
            "article_date",
            "article_url",
            "row_insert_date_time"
        };

        public static int CountAllRows(SQLiteConnection sqlConn)
        {
            int Count = 0;

            string CommandText = $"SELECT COUNT(*) FROM {_TableName};";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                Count = Convert.ToInt32(SQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static int CountRows(SQLiteConnection sqlConn, string feedName)
        {
            int Count = 0;

            string CommandText = $"SELECT COUNT(*) FROM {_TableName} WHERE feed_name = @FeedName;";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                SQLCmd.Parameters.AddWithValue("@FeedName", feedName);

                Count = Convert.ToInt32(SQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static int CountRows(SQLiteConnection sqlConn, string feedName, string articleUrl)
        {
            int Count = 0;

            string CommandText = $"SELECT COUNT(*) FROM {_TableName} WHERE feed_name = @FeedName AND article_url = @ArticleUrl;";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                SQLCmd.Parameters.AddWithValue("@FeedName", feedName);
                SQLCmd.Parameters.AddWithValue("@ArticleUrl", articleUrl);

                Count = Convert.ToInt32(SQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static bool Exists(SQLiteConnection sqlConn, string feedName)
        {
            int Count = CountRows(sqlConn, feedName);

            return Count > 0;
        }

        public static bool Exists(SQLiteConnection sqlConn, string feedName, string articleUrl)
        {
            int Count = CountRows(sqlConn, feedName, articleUrl);

            return Count > 0;
        }

        internal static void MapSQLParametersToAllTableColumns(SQLiteCommand cmd, FeedArticle article, string[] columnNames)
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

                cmd.Parameters.AddWithValue(ParamName, ParamValue);
            }

            return;
        }

        public static List<FeedArticle> GetAllRows(SQLiteConnection sqlConn)
        {
            List<FeedArticle> Rows = new();

            string CommandText = $"SELECT * FROM {_TableName};";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                using (SQLiteDataReader SQLRowReader = SQLCmd.ExecuteReader())
                {
                    CollectRows(SQLRowReader, Rows);
                }
            }

            return Rows;
        }

        public static List<FeedArticle> GetRows(SQLiteConnection sqlConn, string feedName)
        {
            List<FeedArticle> Rows = new();

            string CommandText = $"SELECT * FROM {_TableName} WHERE feed_name = @FeedName;";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                SQLCmd.Parameters.AddWithValue("@FeedName", feedName);

                using (SQLiteDataReader SQLRowReader = SQLCmd.ExecuteReader())
                {
                    CollectRows(SQLRowReader, Rows);
                }
            }

            return Rows;
        }

        private static void CollectRows(SQLiteDataReader reader, List<FeedArticle> rows)
        {
            int ColCount = reader.FieldCount;

            while (reader.Read())
            {
                var FeedName = string.Empty;
                var HeadlineText = string.Empty;
                var ArticleSummary = string.Empty;
                var ArticleText = string.Empty;
                var ArticleDate = string.Empty;
                var ArticleUrl = string.Empty;
                var RowInsertDateTime = string.Empty;

                for (var ColI = 0; ColI < ColCount; ColI++)
                {
                    object ColValue = reader.GetValue(ColI);

                    switch (ColI)
                    {
                        case 0: //feed_name
                            FeedName = $"{ColValue}";
                            break;
                        case 1://headline_text
                            HeadlineText = $"{ColValue}";
                            break;
                        case 2://article_summary
                            ArticleSummary = $"{ColValue}";
                            break;
                        case 3://article_text
                            ArticleText = $"{ColValue}";
                            break;
                        case 4://article_date
                            ArticleDate = $"{ColValue}";
                            break;
                        case 5://article_url
                            ArticleUrl = $"{ColValue}";
                            break;
                        case 6://row_insert_date_time
                            RowInsertDateTime = $"{ColValue}";
                            break;
                    }
                }

                FeedArticle FeedEntry = new()
                {
                    FeedName = FeedName,
                    HeadlineText = HeadlineText,
                    ArticleSummary = ArticleSummary,
                    ArticleText = ArticleText,
                    ArticleDate = ArticleDate,
                    ArticleUrl = ArticleUrl,
                    RowInsertDateTime = RowInsertDateTime
                };

                rows.Add(FeedEntry);
            }

            return;
        }
    }
}
