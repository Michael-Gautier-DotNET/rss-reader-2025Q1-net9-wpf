using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedReader
    {
        private static readonly string _TableName = "feeds";

        public static int GetFeedCount(SQLiteConnection sqlConn)
        {
            int Count = 0;

            string RowCheckCommandText = $"SELECT COUNT(*) FROM {_TableName};";

            using (SQLiteCommand RowCheckSQLCmd = new(RowCheckCommandText, sqlConn))
            {
                Count = Convert.ToInt32(RowCheckSQLCmd.ExecuteScalar());
            }

            return Count;
        }

        public static int GetFeedCount(SQLiteConnection sqlConn, string feedName)
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
    }
}
