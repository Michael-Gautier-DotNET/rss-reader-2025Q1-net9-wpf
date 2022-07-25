namespace gautier.app.rss.reader.ui
{
    public class feed_article
    {
        public string feed_name { get; set; } = string.Empty;
        public string headline_text { get; set; } = string.Empty;
        public string article_summary { get; set; } = string.Empty;
        public string article_text { get; set; } = string.Empty;
        public string article_date { get; set; } = string.Empty;
        public string article_url { get; set; } = string.Empty;
        public string row_insert_date_time { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{headline_text} {article_url} {article_date}";
        }
    }
}
