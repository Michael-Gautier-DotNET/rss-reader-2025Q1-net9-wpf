namespace gautier.rss.data.RDFConversionXD
{
    public class RDFDocument
    {
        public Channel Channel { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}
