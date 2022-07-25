using System.Collections.Generic;

namespace gautier.app.rss.reader.ui
{
    public class feed
    {
        public string feed_name { get; set; } = string.Empty;
        public string feed_url { get; set; } = string.Empty;
        public string last_retrieved { get; set; } = string.Empty;
        public string retrieve_limit_hrs { get; set; } = string.Empty;
        public string retention_days { get; set; } = string.Empty;

        public SortedList<string, feed_article> feed_articles { get; set; } = new SortedList<string, feed_article>();

        public override string ToString()
        {
            return $"{feed_name} {feed_url} {last_retrieved} {retrieve_limit_hrs} {retention_days}";
        }
    }
}
