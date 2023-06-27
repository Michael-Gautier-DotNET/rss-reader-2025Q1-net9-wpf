using System.ServiceModel.Syndication;
using System.Xml;

namespace gautier.rss.data
{
    /// <summary>
    /// Handles network communication. Translates rss XML data into C# object format.
    /// </summary>
    public class RSSNetClient
    {
        public List<FeedArticle> GetArticles(Feed feedInfo)
        {
            List<FeedArticle> Articles = new();

            SyndicationFeed ExternalFeed;

            using(var FeedReader = XmlReader.Create(feedInfo.FeedUrl))
            {
                ExternalFeed = SyndicationFeed.Load(FeedReader);
            }

            return Articles;
        }
    }
}
