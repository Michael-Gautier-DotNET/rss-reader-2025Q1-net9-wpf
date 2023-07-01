using System.Xml;

namespace gautier.rss.data.RDFConversion2
{
    public class RDFLoader
    {
        public RDFDocument Load(string filePath)
        {
            RDFDocument rdfDocument = new RDFDocument();

            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            if (document.DocumentElement != null)
            {
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    switch (node.LocalName)
                    {
                        case "channel":
                            {
                                ParseChannel(node, rdfDocument);
                            }
                            break;
                        case "item":
                            {
                                Item item = new Item();
                                ParseItem(node, item);
                                rdfDocument.Items.Add(item);
                            }
                            break;
                    }
                }
            }

            return rdfDocument;
        }

        private void ParseChannel(XmlNode? channelNode, RDFDocument rdfDocument)
        {
            if (channelNode is null)
                return;

            Channel channel = new Channel();

            foreach (XmlNode node in channelNode.ChildNodes)
            {
                if (node is XmlElement element)
                {
                    switch (element.LocalName)
                    {
                        case "title":
                            channel.Title = element.InnerText;
                            break;
                        case "link":
                            channel.Link = element.InnerText;
                            break;
                        case "description":
                            channel.Description = element.InnerText;
                            break;
                        case "language":
                            channel.Language = element.InnerText;
                            break;
                        case "rights":
                            channel.Rights = element.InnerText;
                            break;
                        case "date":
                            DateTime.TryParse(element.InnerText, out DateTime dateValue);
                            channel.Date = dateValue;
                            break;
                        case "publisher":
                            channel.Publisher = element.InnerText;
                            break;
                        case "creator":
                            channel.Creator = element.InnerText;
                            break;
                        case "subject":
                            channel.Subject = element.InnerText;
                            break;
                        case "updatePeriod":
                            channel.UpdatePeriod = element.InnerText;
                            break;
                        case "updateFrequency":
                            int.TryParse(element.InnerText, out int updateFrequencyValue);
                            channel.UpdateFrequency = updateFrequencyValue;
                            break;
                        case "updateBase":
                            DateTime.TryParse(element.InnerText, out DateTime updateBaseValue);
                            channel.UpdateBase = updateBaseValue;
                            break;
                    }
                }
            }

            rdfDocument.Channel = channel;
        }

        private void ParseItem(XmlNode itemNode, Item item)
        {
            foreach (XmlNode node in itemNode.ChildNodes)
            {
                if (node is XmlElement element)
                {
                    switch (element.LocalName)
                    {
                        case "title":
                            item.Title = element.InnerText;
                            break;
                        case "link":
                            item.Link = element.InnerText;
                            break;
                        case "description":
                            item.Description = element.InnerText;
                            break;
                        case "date":
                            DateTime.TryParse(element.InnerText, out DateTime dateValue);
                            item.Date = dateValue;
                            break;
                        case "section":
                            item.Section = element.InnerText;
                            break;
                    }
                }
            }
        }
    }
}
