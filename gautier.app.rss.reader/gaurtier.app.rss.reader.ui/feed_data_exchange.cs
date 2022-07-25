using gautier.app.rss.reader.ui.Properties;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace gautier.app.rss.reader.ui
{
    internal class feed_data_exchange
    {
        internal static SortedList<string, feed> get_all_feeds()
        {
            var feeds = new SortedList<string, feed>();

            using (var sql_conn = get_sql_conn())
            {
                sql_conn.Open();

                using (var sql_com = sql_conn.CreateCommand())
                {
                    sql_com.CommandType = CommandType.StoredProcedure;
                    sql_com.CommandText = "get_all_feeds";

                    using (var col_reader = sql_com.ExecuteReader())
                    {
                        var col_count = col_reader.FieldCount;

                        while (col_reader.Read())
                        {
                            var feed_item = new feed();

                            for (var col_index = 0; col_index < col_count; col_index++)
                            {
                                var col_value = col_reader.GetString(col_index);
                                //Assume all is text/string.
                                switch (col_index)
                                {
                                    case 0: //feed_name
                                        feed_item.feed_name = col_value;
                                        break;
                                    case 1://feed_url
                                        feed_item.feed_url = col_value;
                                        break;
                                    case 2://last_retrieved
                                        feed_item.last_retrieved = col_value;
                                        break;
                                    case 3://retrieve_limit_hrs
                                        feed_item.retrieve_limit_hrs = col_value;
                                        break;
                                    case 4://retention_days
                                        feed_item.retention_days = col_value;
                                        break;
                                }
                            }

                            if (!feeds.ContainsKey(feed_item.feed_name))
                            {
                                feeds.Add(feed_item.feed_name, feed_item);
                            }
                        }
                    }
                }
            }

            return feeds;
        }

        internal static SortedList<string, SortedList<string, feed_article>> get_all_feed_articles()
        {
            var feeds_articles = new SortedList<string, SortedList<string, feed_article>>();

            using (var sql_conn = get_sql_conn())
            {
                sql_conn.Open();

                using (var sql_com = sql_conn.CreateCommand())
                {
                    sql_com.CommandType = CommandType.StoredProcedure;
                    sql_com.CommandText = "get_all_feeds_articles";

                    using (var col_reader = sql_com.ExecuteReader())
                    {
                        var col_count = col_reader.FieldCount;

                        while (col_reader.Read())
                        {
                            var feed_article_item = new feed_article();

                            for (var col_index = 0; col_index < col_count; col_index++)
                            {
                                var col_value = col_reader.GetString(col_index);
                                //Assume all is text/string.
                                switch (col_index)
                                {
                                    case 0: //feed_name
                                        feed_article_item.feed_name = col_value;
                                        break;
                                    case 1://headline_text
                                        feed_article_item.headline_text = col_value;
                                        break;
                                    case 2://article_summary
                                        feed_article_item.article_summary = col_value;
                                        break;
                                    case 3://article_text
                                        feed_article_item.article_text = col_value;
                                        break;
                                    case 4://article_date
                                        feed_article_item.article_date = col_value;
                                        break;
                                    case 5://article_url
                                        feed_article_item.article_url = col_value;
                                        break;
                                    case 6://row_insert_date_time
                                        feed_article_item.row_insert_date_time = col_value;
                                        break;
                                }
                            }

                            if (feeds_articles.ContainsKey(feed_article_item.feed_name))
                            {
                                var articles = feeds_articles[feed_article_item.feed_name];

                                if (!articles.ContainsKey(feed_article_item.article_url))
                                {
                                    articles.Add(feed_article_item.article_url, feed_article_item);
                                }
                            }
                        }
                    }
                }
            }

            return feeds_articles;
        }

        internal static List<string> get_feed_columns()
        {
            return new List<string>(Settings.Default.feeds_column_names as IEnumerable<string>);
        }

        internal static List<string> get_feed_article_columns()
        {
            return new List<string>(Settings.Default.feeds_articles_column_names as IEnumerable<string>);
        }

        private static SqlConnection get_sql_conn()
        {
            return new SqlConnection(Settings.Default.rss_connection_string_sqlserver);
        }
    }
}
