using System.Xml.Serialization;

namespace gautier.rss.data.RDFConversion
{
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement(ElementName = "link")]
        public string Link { get; set; } = string.Empty;

        [XmlElement(ElementName = "description")]
        public string Description { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:date", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Date { get; set; } = string.Empty;

        [XmlElement(ElementName = "slash:section", Namespace = "http://purl.org/rss/1.0/modules/slash/")]
        public string Section { get; set; } = string.Empty;
    }
}
