using System.Data.SQLite;
using System.Text;

namespace gautier.rss.data.RSSDb
{
    internal class FeedArticleWriter
    {
        private static readonly string[] _ColumnNames = FeedArticleReader.TableColumnNames;

        internal static void AddFeedArticle(SQLiteConnection sqlConn, FeedArticleUnion article)
        {
            string[] ColumnNames = SQLUtil.StripColumnByName("id", _ColumnNames);

            StringBuilder CommandText = SQLUtil.CreateSQLInsertCMDText(FeedArticleReader.TableName, ColumnNames);

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedArticleReader.MapSQLParametersToAllTableColumns(SQLCmd, article.ArticleDetail, ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }

        internal static void ModifyFeedArticle(SQLiteConnection sqlConn, FeedArticleUnion article)
        {
            string[] ColumnNames = SQLUtil.StripColumnNames(new(){"id", "feed_name", "article_url"}, _ColumnNames);

            StringBuilder CommandText = SQLUtil.CreateSQLUpdateCMDText(FeedArticleReader.TableName, ColumnNames);
            CommandText.Append("feed_name = @feed_name AND article_url = @article_url;");

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedArticleReader.MapSQLParametersToAllTableColumns(SQLCmd, article.ArticleDetail, _ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }

        internal static void ModifyFeedArticleKey(SQLiteConnection sqlConn, string feedNameOld, string feedNameNew)
        {
            string CommandText = $"UPDATE {FeedArticleReader.TableName} SET feed_name = @FeedNameNew WHERE feed_name = @FeedNameOld;";

            using (SQLiteCommand SQLCmd = new(CommandText, sqlConn))
            {
                SQLCmd.Parameters.AddWithValue("@FeedNameOld", feedNameOld);
                SQLCmd.Parameters.AddWithValue("@FeedNameNew", feedNameNew);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }
    }
}
