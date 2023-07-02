using System.Xml;

namespace gautier.rss.data.RDFConversion2
{
    public class RDFLoader
    {
        public RDFDocument Load(string filePath)
        {
            var RDFDoc = new RDFDocument();

            var RDFXmlDoc = new XmlDocument();
            RDFXmlDoc.Load(filePath);

            if (RDFXmlDoc.DocumentElement is not null)
            {
                var RootNode = RDFXmlDoc.DocumentElement;

                foreach (XmlNode XNode in RootNode.ChildNodes)
                {
                    switch (XNode.LocalName)
                    {
                        case "channel":
                            {
                                var RDFChannel = ParseChannel(XNode);

                                RDFDoc.Channel = RDFChannel;
                            }
                            break;
                        case "item":
                            {
                                var RDFItem = ParseItem(XNode);

                                RDFDoc.Items.Add(RDFItem);
                            }
                            break;
                    }
                }
            }

            return RDFDoc;
        }

        private Channel ParseChannel(XmlNode? channelNode)
        {
            Channel RDFChannel = new();

            if (channelNode is null)
            {
                return RDFChannel;
            }

            foreach (XmlNode ChannelContentNode in channelNode.ChildNodes)
            {
                if (ChannelContentNode is XmlElement XElement)
                {
                    switch (XElement.LocalName)
                    {
                        case "title":
                            {
                                RDFChannel.Title = XElement.InnerText;
                            }
                            break;
                        case "link":
                            {
                                RDFChannel.Link = XElement.InnerText;
                            }
                            break;
                        case "description":
                            {
                                RDFChannel.Description = XElement.InnerText;
                            }
                            break;
                        case "language":
                            {
                                RDFChannel.Language = XElement.InnerText;
                            }
                            break;
                        case "rights":
                            {
                                RDFChannel.Rights = XElement.InnerText;
                            }
                            break;
                        case "date":
                            {
                                DateTime.TryParse(XElement.InnerText, out DateTime dateValue);
                                RDFChannel.Date = dateValue;
                            }
                            break;
                        case "publisher":
                            {
                                RDFChannel.Publisher = XElement.InnerText;
                            }
                            break;
                        case "creator":
                            {
                                RDFChannel.Creator = XElement.InnerText;
                            }
                            break;
                        case "subject":
                            {
                                RDFChannel.Subject = XElement.InnerText;
                            }
                            break;
                        case "updatePeriod":
                            {
                                RDFChannel.UpdatePeriod = XElement.InnerText;
                            }
                            break;
                        case "updateFrequency":
                            {
                                int.TryParse(XElement.InnerText, out int updateFrequencyValue);
                                RDFChannel.UpdateFrequency = updateFrequencyValue;
                            }
                            break;
                        case "updateBase":
                            {
                                DateTime.TryParse(XElement.InnerText, out DateTime updateBaseValue);
                                RDFChannel.UpdateBase = updateBaseValue;
                            }
                            break;
                    }
                }
            }

            return RDFChannel;
        }

        private Item ParseItem(XmlNode node)
        {
            Item RDFItem = new();

            if(node is null)
            {
                return RDFItem;
            }

            foreach (XmlNode ItemContentNode in node.ChildNodes)
            {
                if (ItemContentNode is XmlElement XElement)
                {
                    switch (XElement.LocalName)
                    {
                        case "title":
                            {
                                RDFItem.Title = XElement.InnerText;
                            }
                            break;
                        case "link":
                            {
                                RDFItem.Link = XElement.InnerText;
                            }
                            break;
                        case "description":
                            {
                                RDFItem.Description = XElement.InnerText;
                            }
                            break;
                        case "date":
                            {
                                DateTime.TryParse(XElement.InnerText, out DateTime dateValue);
                                RDFItem.Date = dateValue;
                            }
                            break;
                        case "section":
                            {
                                RDFItem.Section = XElement.InnerText;
                            }
                            break;
                    }
                }
            }

            return RDFItem;
        }
    }
}
