using System.Data.SQLite;
using System.Text;

namespace gautier.rss.data.RSSDb
{
    internal class FeedWriter
    {
        private static readonly string[] _ColumnNames = FeedReader.GetSQLFeedsColumnNames();

        internal static void AddFeed(SQLiteConnection sqlConn, Feed feedHeader)
        {
            StringBuilder CommandText = SQLUtil.CreateSQLInsertCMDText(FeedReader.TableName, _ColumnNames);

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedReader.CreateFeedParameters(SQLCmd, feedHeader, _ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }

        internal static void ModifyFeed(SQLiteConnection sqlConn, Feed feedHeader)
        {
            StringBuilder CommandText = SQLUtil.CreateSQLUpdateCMDText(FeedReader.TableName, _ColumnNames);
            CommandText.Append("feed_name = @feed_name;");

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedReader.CreateFeedParameters(SQLCmd, feedHeader, _ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }
    }
}
