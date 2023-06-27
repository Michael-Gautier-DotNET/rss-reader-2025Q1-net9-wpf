using gautier.rss.data;
using System.Xml;

namespace gautier.app.rss.feeddownloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string feedSaveDirectoryPath = @"C:\RSSFeeds\";

            if (Directory.Exists(feedSaveDirectoryPath) == false)
            {
                Directory.CreateDirectory(feedSaveDirectoryPath);
            }

            List<Feed> FeedInfos = new List<Feed>
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
                },
                new Feed{
                    FeedName = "DistroWatch",
                    FeedUrl = "https://distrowatch.com/news/dw.xml"
                }
            };

            foreach (var FeedInfo in FeedInfos)
            {
                using (var feedXml = XmlReader.Create(FeedInfo.FeedUrl))
                {
                    var FeedFilePath = Path.Combine(feedSaveDirectoryPath, $"{FeedInfo.FeedName}.xml");

                    using(var feedXmlWriter = XmlWriter.Create(FeedFilePath))
                    {
                        feedXmlWriter.WriteNode(feedXml, true);
                    }
                }
            }

            Console.WriteLine("Gautier RSS Reader WPF");
        }
    }
}