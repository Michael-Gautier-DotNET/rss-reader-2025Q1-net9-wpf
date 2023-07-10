using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedReader
    {
        public static int GetFeedCount(SQLiteConnection sqlConn)
        {
            int FeedCount = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM feed;";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                FeedCount = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return FeedCount;
        }

        public static int GetFeedCount(SQLiteConnection sqlConn, string feedName)
        {
            int FeedCount = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM feed WHERE feed_name = @FeedName;";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                RowCheckSQLCmd.Parameters.AddWithValue("@FeedName", feedName);

                FeedCount = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return FeedCount;
        }
    }
}
