namespace gautier.rss.data.RDFConversion2
{
    public class RDFDocument
    {
        public Channel Channel { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}
