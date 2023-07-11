using System.Data;
using System.Data.SQLite;

namespace gautier.rss.data.RSSDb
{
    public class FeedReader
    {
        private static readonly string _TableName = "feeds";
        public static string TableName => _TableName;

        public static string[] TableColumnNames => new string[]
        {
            "feed_name",
            "feed_url",
            "last_retrieved",
            "retrieve_limit_hrs",
            "retention_days"
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

        public static bool Exists(SQLiteConnection sqlConn, string feedName)
        {
            int Count = CountRows(sqlConn, feedName);

            return Count > 0;
        }

        internal static void MapSQLParametersToAllTableColumns(SQLiteCommand SQLCmd, Feed FeedHeader, string[] columnNames)
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

        public static List<Feed> GetAllRows(SQLiteConnection sqlConn)
        {
            List<Feed> Rows = new();

            string CommandText = $"SELECT * FROM {_TableName};";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                using (SQLiteDataReader SQLRowReader = SQLCmd.ExecuteReader())
                {
                    int ColCount = SQLRowReader.FieldCount;

                    while (SQLRowReader.Read())
                    {
                        string FeedName = string.Empty;
                        string FeedUrl = string.Empty;
                        string LastRetrieved = string.Empty;
                        string RetrieveLimitHrs = string.Empty;
                        string RetentionDays = string.Empty;

                        for (int ColI = 0; ColI < ColCount; ColI++)
                        {
                            object ColValue = SQLRowReader.GetValue(ColI);

                            switch (ColI)
                            {
                                case 0: //feed_name
                                    FeedName = $"{ColValue}";
                                    break;
                                case 1://feed_url
                                    FeedUrl = $"{ColValue}";
                                    break;
                                case 2://last_retrieved
                                    LastRetrieved = $"{ColValue}";
                                    break;
                                case 3://retrieve_limit_hrs
                                    RetrieveLimitHrs = $"{ColValue}";
                                    break;
                                case 4://retention_days
                                    RetentionDays = $"{ColValue}";
                                    break;
                            }
                        }

                        Feed FeedEntry = new Feed
                        {
                            FeedName = FeedName,
                            FeedUrl = FeedUrl,
                            LastRetrieved = LastRetrieved,
                            RetrieveLimitHrs = RetrieveLimitHrs,
                            RetentionDays = RetentionDays
                        };

                        Rows.Add(FeedEntry);
                    }
                }
            }

            return Rows;
        }
    }
}
