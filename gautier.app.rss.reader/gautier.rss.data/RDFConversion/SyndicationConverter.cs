using System.ServiceModel.Syndication;

namespace gautier.rss.data.RDFConversion
{
    public static class SyndicationConverter
    {
        public static SyndicationFeed ConvertToSyndicationFeed(RdfRDF rdfRdf)
        {
            var feed = new SyndicationFeed();

            var RDFChannel = rdfRdf.Channel ?? new();

            Uri ChannelUrl = new Uri("http://127.0.0.1");

            if(string.IsNullOrWhiteSpace(RDFChannel.Link) == false)
            {
                ChannelUrl = new Uri(RDFChannel.Link);
            }

            // Set channel properties
            feed.Title = new TextSyndicationContent(RDFChannel.Title);
            feed.Links.Add(new SyndicationLink(ChannelUrl));
            feed.Description = new TextSyndicationContent(RDFChannel.Description);
            feed.Language = RDFChannel.Language;

            // Set channel author
            var authors = new SyndicationPerson[]
            {
                new SyndicationPerson(RDFChannel.Creator),
            };

            foreach (var author in authors)
            {
                feed.Authors.Add(author);
            }

            // Set channel categories
            var categories = new SyndicationCategory[]
            {
                new SyndicationCategory(RDFChannel.Subject),
            };

            foreach (var category in categories)
            {
                feed.Categories.Add(category);
            }

            // Set channel updated date
            if (DateTime.TryParse(RDFChannel.Date, out DateTime updatedDate))
            {
                feed.LastUpdatedTime = updatedDate;
            }

            var RDFItems = rdfRdf.Items ?? new();
            // Set feed items
            foreach (var item in RDFItems)
            {
                var feedItem = new SyndicationItem();
                feedItem.Title = new TextSyndicationContent(item.Title);
                feedItem.Links.Add(new SyndicationLink(new Uri(item.Link)));
                feedItem.Summary = new TextSyndicationContent(item.Description);

                // Set item published date
                if (DateTime.TryParse(item.Date, out DateTime publishedDate))
                {
                    feedItem.PublishDate = publishedDate;
                }

                // Set item categories
                var itemCategories = new SyndicationCategory[]
                {
                    new SyndicationCategory(item.Section),
                };

                foreach (var category in itemCategories)
                {
                    feedItem.Categories.Add(category);
                }

                feed.Items.Append(feedItem);
            }

            return feed;
        }
    }
}
