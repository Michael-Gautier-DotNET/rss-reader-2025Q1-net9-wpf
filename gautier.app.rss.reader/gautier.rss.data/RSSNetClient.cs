using System.ServiceModel.Syndication;
using System.Xml;

using gautier.rss.data.RDFConversionXD;

namespace gautier.rss.data
{
    /// <summary>
    /// Handles network communication. Translates rss XML data into C# object format.
    /// </summary>
    public static class RSSNetClient
    {
        public static void CreateRSSFeedFile(string feedUrl, string rssFeedFilePath)
        {
            using XmlReader feedXml = XmlReader.Create(feedUrl);

            using XmlWriter feedXmlWriter = XmlWriter.Create(rssFeedFilePath);

            feedXmlWriter.WriteNode(feedXml, false);

            return;
        }

        public static SyndicationFeed CreateRSSSyndicationFeed(string rssFeedFilePath)
        {
            SyndicationFeed RSSFeed = new();

            if (File.Exists(rssFeedFilePath) == true)
            {
                try
                {
                    using XmlReader RSSXmlFile = XmlReader.Create(rssFeedFilePath);

                    RSSFeed = SyndicationFeed.Load(RSSXmlFile);
                }
                catch (XmlException xmlE)
                {
                    bool ExceptionContainsRDF = xmlE.Message.Contains("'RDF'");
                    bool ExceptionContainsInvalidFormat = xmlE.Message.Contains("not an allowed feed format");

                    if (ExceptionContainsRDF && ExceptionContainsInvalidFormat)
                    {
                        RSSFeed = SyndicationConverter.ConvertToSyndicationFeed(rssFeedFilePath);
                    }
                }

            }

            return RSSFeed;
        }
    }
}
