using System.Data.SQLite;
using System.Text;

namespace gautier.rss.data.RSSDb
{
    internal class FeedArticleWriter
    {
        private static readonly string[] _ColumnNames = FeedArticleReader.GetSQLFeedsArticlesColumnNames();

        internal static void AddFeedArticle(SQLiteConnection sqlConn, FeedArticleUnion article)
        {
            StringBuilder CommandText = SQLUtil.CreateSQLInsertCMDText(FeedArticleReader.TableName, _ColumnNames);

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedArticleReader.CreateFeedArticleParameters(SQLCmd, article.ArticleDetail, _ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }

        internal static void ModifyFeedArticle(SQLiteConnection sqlConn, FeedArticleUnion article)
        {
            StringBuilder CommandText = SQLUtil.CreateSQLUpdateCMDText(FeedArticleReader.TableName, _ColumnNames);
            CommandText.Append("feed_name = @feed_name AND article_url = @article_url;");

            using (SQLiteCommand SQLCmd = new(CommandText.ToString(), sqlConn))
            {
                FeedArticleReader.CreateFeedArticleParameters(SQLCmd, article.ArticleDetail, _ColumnNames);

                SQLCmd.ExecuteNonQuery();
            }

            return;
        }
    }
}
