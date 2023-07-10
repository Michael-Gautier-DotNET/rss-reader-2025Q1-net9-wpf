using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedReader
    {
        private static readonly string _TableName = "feeds";
        public static string TableName => _TableName;

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

        internal static void CreateFeedParameters(SQLiteCommand SQLCmd, Feed FeedHeader, string[] columnNames)
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

        internal static string[] GetSQLFeedsColumnNames()
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
    }
}
