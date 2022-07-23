namespace gautier.app.rss.sqlitetosqlserver
{
    internal class feed_article
    {
        internal string feed_name { get; set; } = string.Empty;
        internal string headline_text { get; set; } = string.Empty;
        internal string article_summary { get; set; } = string.Empty;
        internal string article_text { get; set; } = string.Empty;
        internal string article_date { get; set; } = string.Empty;
        internal string article_url { get; set; } = string.Empty;
        internal string row_insert_date_time { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{headline_text} {article_url} {article_date}";
        }
    }
}
