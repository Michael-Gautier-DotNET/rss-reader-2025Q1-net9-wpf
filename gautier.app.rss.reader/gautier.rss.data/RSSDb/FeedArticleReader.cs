using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedArticleReader
    {
        private static readonly string _TableName = "feeds_articles";

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
    }
}
