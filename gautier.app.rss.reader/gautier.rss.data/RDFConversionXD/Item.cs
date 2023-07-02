namespace gautier.rss.data.RDFConversionXD
{
    public class Item
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Section { get; set; } = string.Empty;
    }
}
