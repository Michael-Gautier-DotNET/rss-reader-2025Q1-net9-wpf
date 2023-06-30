using System.Xml.Serialization;

namespace gautier.rss.data.RDFConversion
{
    [XmlRoot(ElementName = "rdf:RDF", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public class RdfRDF
    {
        [XmlElement(ElementName = "channel")]
        public Channel? Channel { get; set; }

        [XmlElement(ElementName = "item")]
        public List<Item>? Items { get; set; }
    }
}
