using gautier.rss.data;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        private static readonly string _FeedSaveDirectoryPath = @"C:\RSSFeeds\";
        private static readonly Feed[] _FeedInfos = GetStaticFeedInfos();
        internal static void Main(string[] args)
        {
            SetupFeedDirectory();

            FeedFileConverter.CreateStaticFeedFiles(_FeedSaveDirectoryPath, _FeedInfos);

            FeedFileConverter.TransformStaticFeedFiles(_FeedSaveDirectoryPath, _FeedInfos);

            return;
        }

        private static void SetupFeedDirectory()
        {
            if (Directory.Exists(_FeedSaveDirectoryPath) == false)
            {
                Directory.CreateDirectory(_FeedSaveDirectoryPath);
            }

            return;
        }

        public static Feed[] GetStaticFeedInfos()
        {
            Feed[] FeedInfos = new Feed[]
            {
                new Feed{
                    FeedName = "Ars Technica",
                    FeedUrl = "https://feeds.arstechnica.com/arstechnica/index"
                },
                new Feed{
                    FeedName = "Slashdot",//no more than once per 30 minutes
                    FeedUrl = "http://rss.slashdot.org/Slashdot/slashdotMainatom"
                },
                new Feed{
                    FeedName = "Phoronix",
                    FeedUrl = "https://www.phoronix.com/phoronix-rss.php"
                },/*
                  * 6/29/2023
                  * File format for this feed is not readable through the Syndication API
                  *    at the moment.
                  *    Generates the following result:
                  *    System.Xml.XmlException
                          HResult=0x80131940
                          Message=The element with name 'RDF' and namespace 'http://www.w3.org/1999/02/22-rdf-syntax-ns#' is not an allowed feed format.
                          Source=System.ServiceModel.Syndication
                  *     ------------------------------------------
                  *     Will come back to this file later. Commenting it out so as to move forward with 
                  *     the overall API and implementation build out.

                */new Feed{
                    FeedName = "DistroWatch",
                    FeedUrl = "https://distrowatch.com/news/dw.xml"
                }
            };

            return FeedInfos;
        }
    }
}