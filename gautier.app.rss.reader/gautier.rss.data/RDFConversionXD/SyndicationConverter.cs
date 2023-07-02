using System.ServiceModel.Syndication;

namespace gautier.rss.data.RDFConversion2
{
    public class SyndicationConverter
    {
        public static SyndicationFeed ConvertToSyndicationFeed(string rdfFilePath)
        {
            RDFLoader rdfLoader = new RDFLoader();
            RDFDocument rdfDocument = rdfLoader.Load(rdfFilePath);

            // Create a new SyndicationFeed
            SyndicationFeed feed = new SyndicationFeed();
            feed.Title = new TextSyndicationContent(rdfDocument.Channel.Title);
            feed.Description = new TextSyndicationContent(rdfDocument.Channel.Description);
            feed.Links.Add(new SyndicationLink(new Uri(rdfDocument.Channel.Link)));
            feed.Language = rdfDocument.Channel.Language;

            List<SyndicationItem> SyndicationItems = new List<SyndicationItem>();

            // Map RDF items to SyndicationItems
            foreach (var item in rdfDocument.Items)
            {
                SyndicationItem syndicationItem = new SyndicationItem();
                syndicationItem.Title = new TextSyndicationContent(item.Title);
                syndicationItem.Summary = new TextSyndicationContent(item.Description);
                syndicationItem.Links.Add(new SyndicationLink(new Uri(item.Link)));
                syndicationItem.PublishDate = item.Date;

                // Add the item to the feed
                SyndicationItems.Add(syndicationItem);
            }

            feed.Items = SyndicationItems;

            return feed;
        }
    }
}
