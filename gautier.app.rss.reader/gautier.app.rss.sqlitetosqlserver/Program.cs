using gautier.app.rss.sqlitetosqlserver.Properties;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace gautier.app.rss.sqlitetosqlserver
{
    internal class program
    {
        static internal string rss_db_dir = Settings.Default.rss_db_dir;

        static int Main()
        {
            var feeds = new SortedList<string, feed>();

            if (Directory.Exists(rss_db_dir))
            {
                var rss_db_paths = Directory.GetFiles(rss_db_dir);

                if (rss_db_paths.Length > 0)
                {
                    foreach (var rss_db_path in rss_db_paths)
                    {
                        var conn_string = $"Data Source=\"{rss_db_path}\"";

                        using (var db_conn = new SqliteConnection(conn_string))
                        {
                            db_conn.Open();

                            update_feeds(feeds, db_conn);
                            update_feed_articles(feeds, db_conn);
                        }
                    }

                    using (var sql_conn = new SqlConnection(Settings.Default.sql_connection_string_feeds_db_sqlserver))
                    {
                        sql_conn.Open();

                        foreach (var feed_name in feeds.Keys)
                        {
                            var feed_item = feeds[feed_name];

                            insert_feed_item(sql_conn, feed_item);

                            var feed_urls = feed_item.feed_articles.Keys;

                            foreach (var feed_url in feed_urls)
                            {
                                var feed_article_item = feed_item.feed_articles[feed_url];

                                insert_feed_article_item(sql_conn, feed_article_item);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No files in directory");
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
                Console.WriteLine("Missing directory:");
                Console.WriteLine(rss_db_dir);

                Thread.Sleep(5000);
            }

            return 0;
        }

        private static void insert_feed_item(SqlConnection sql_conn, feed feed_item)
        {
            using (var sql_com = sql_conn.CreateCommand())
            {
                sql_com.CommandType = CommandType.StoredProcedure;
                sql_com.CommandText = "insert_feed";

                var col_names = get_feed_columns();

                for (var col_index = 0; col_index < col_names.Count; col_index++)
                {
                    var col_name = col_names[col_index];

                    var sql_param = sql_com.Parameters.Add($"@{col_name}", SqlDbType.NVarChar, -1);

                    switch (col_index)
                    {
                        case 0: //feed_name
                            sql_param.SqlValue = feed_item.feed_name;
                            break;
                        case 1://feed_url
                            sql_param.SqlValue = feed_item.feed_url;
                            break;
                        case 2://last_retrieved
                            sql_param.SqlValue = feed_item.last_retrieved;
                            break;
                        case 3://retrieve_limit_hrs
                            sql_param.SqlValue = feed_item.retrieve_limit_hrs;
                            break;
                        case 4://retention_days
                            sql_param.SqlValue = feed_item.retention_days;
                            break;
                    }
                }

                sql_com.ExecuteNonQuery();
            }

            return;
        }

        private static void insert_feed_article_item(SqlConnection sql_conn, feed_article feed_article_item)
        {
            using (var sql_com = sql_conn.CreateCommand())
            {
                sql_com.CommandType = CommandType.StoredProcedure;
                sql_com.CommandText = "insert_feed_article";

                var col_names = get_feed_article_columns();

                for (var col_index = 0; col_index < col_names.Count; col_index++)
                {
                    var col_name = col_names[col_index];

                    var sql_param = sql_com.Parameters.Add($"@{col_name}", SqlDbType.NVarChar, -1);

                    switch (col_index)
                    {
                        case 0: //feed_name
                            sql_param.SqlValue = feed_article_item.feed_name;
                            break;
                        case 1://headline_text
                            sql_param.SqlValue = feed_article_item.headline_text;
                            break;
                        case 2://article_summary
                            sql_param.SqlValue = feed_article_item.article_summary;
                            break;
                        case 3://article_text
                            sql_param.SqlValue = feed_article_item.article_text;
                            break;
                        case 4://article_date
                            sql_param.SqlValue = feed_article_item.article_date;
                            break;
                        case 5://article_url
                            sql_param.SqlValue = feed_article_item.article_url;
                            break;
                        case 6://row_insert_date_time
                            sql_param.SqlValue = feed_article_item.row_insert_date_time;
                            break;
                    }
                }

                sql_com.ExecuteNonQuery();
            }

            return;
        }

        static List<string> get_feed_columns()
        {
            return new List<string>
                {
                    "feed_name",
                    "feed_url",
                    "last_retrieved",
                    "retrieve_limit_hrs",
                    "retention_days"
                };
        }

        static void update_feeds(SortedList<string, feed> feeds, SqliteCommand sql_command)
        {
            using (var col_reader = sql_command.ExecuteReader())
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

            return;
        }

        static void update_feeds(SortedList<string, feed> feeds, SqliteConnection db_conn)
        {
            var sql_command = db_conn.CreateCommand();

            sql_command.CommandText = $"select {string.Join(",", get_feed_columns())} from feeds;";

            update_feeds(feeds, sql_command);

            return;
        }

        /*Feed Articles*/
        static List<string> get_feed_article_columns()
        {
            return new List<string>
                {
                    "feed_name",
                    "headline_text",
                    "article_summary",
                    "article_text",
                    "article_date",
                    "article_url",
                    "row_insert_date_time"
                };
        }

        static void update_feed_articles(SortedList<string, feed> feeds, SqliteCommand sql_command)
        {
            using (var col_reader = sql_command.ExecuteReader())
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

                    if (feeds.ContainsKey(feed_article_item.feed_name))
                    {
                        var feed_articles = feeds[feed_article_item.feed_name].feed_articles;

                        if (!feed_articles.ContainsKey(feed_article_item.article_url))
                        {
                            feed_articles.Add(feed_article_item.article_url, feed_article_item);
                        }
                    }
                }
            }

            return;
        }

        static void update_feed_articles(SortedList<string, feed> feeds, SqliteConnection db_conn)
        {
            var sql_command = db_conn.CreateCommand();

            sql_command.CommandText = $"select {string.Join(",", get_feed_article_columns())} from feeds_articles;";

            update_feed_articles(feeds, sql_command);

            return;
        }
    }
}
