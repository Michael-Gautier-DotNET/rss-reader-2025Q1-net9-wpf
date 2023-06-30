using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace gautier.rss.data.RDFConversion
{
    public class Channel
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement(ElementName = "link")]
        public string Link { get; set; } = string.Empty;

        [XmlElement(ElementName = "description")]
        public string Description { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:language", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Language { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:rights", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Rights { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:date", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Date { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:publisher", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Publisher { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Creator { get; set; } = string.Empty;

        [XmlElement(ElementName = "dc:subject", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Subject { get; set; } = string.Empty;

        [XmlElement(ElementName = "syn:updatePeriod", Namespace = "http://purl.org/rss/1.0/modules/syndication/")]
        public string UpdatePeriod { get; set; } = string.Empty;

        [XmlElement(ElementName = "syn:updateFrequency", Namespace = "http://purl.org/rss/1.0/modules/syndication/")]
        public string UpdateFrequency { get; set; } = string.Empty;

        [XmlElement(ElementName = "syn:updateBase", Namespace = "http://purl.org/rss/1.0/modules/syndication/")]
        public string UpdateBase { get; set; } = string.Empty;
    }
}
