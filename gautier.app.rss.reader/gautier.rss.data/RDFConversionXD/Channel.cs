namespace gautier.rss.data.RDFConversionXD
{
    public class Channel
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Rights { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string UpdatePeriod { get; set; } = string.Empty;
        public int UpdateFrequency { get; set; }
        public DateTime UpdateBase { get; set; }
    }
}
