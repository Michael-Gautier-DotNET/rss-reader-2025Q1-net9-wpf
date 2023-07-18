using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

using gautier.rss.data;

namespace gautier.app.rss.reader.ui.UIData
{
    public class BindableFeed : DependencyObject
    {
        private static readonly DateTimeFormatInfo _InvariantFormat = DateTimeFormatInfo.InvariantInfo;

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(BindableFeed), new PropertyMetadata(-1));

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(BindableFeed), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(BindableFeed), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LastRetrievedProperty = DependencyProperty.Register("LastRetrieved", typeof(DateTime), typeof(BindableFeed), new PropertyMetadata(DateTime.Now));

        public static readonly DependencyProperty RetrieveLimitHrsProperty = DependencyProperty.Register("RetrieveLimitHrs", typeof(int), typeof(BindableFeed), new PropertyMetadata(1));

        public static readonly DependencyProperty RetentionDaysProperty = DependencyProperty.Register("RetentionDays", typeof(int), typeof(BindableFeed), new PropertyMetadata(45));

        public int Id
        {
            get => int.Parse($"{GetValue(IdProperty)}");
            set => SetValue(IdProperty, value);
        }

        public string Name
        {
            get => $"{GetValue(NameProperty)}";
            set => SetValue(NameProperty, value);

        }

        public string OriginalName { get; set; } = string.Empty;

        public string Url
        {
            get => $"{GetValue(UrlProperty)}";
            set => SetValue(UrlProperty, value);
        }

        public string OriginalUrl { get; set; } = string.Empty;

        public DateTime LastRetrieved
        {
            get => DateTime.Parse($"{GetValue(LastRetrievedProperty)}");
            set => SetValue(LastRetrievedProperty, value);
        }

        public int RetrieveLimitHrs
        {
            get => int.Parse($"{GetValue(RetrieveLimitHrsProperty)}");
            set => SetValue(RetrieveLimitHrsProperty, value);
        }

        public int RetentionDays
        {
            get => int.Parse($"{GetValue(RetentionDaysProperty)}");
            set => SetValue(RetentionDaysProperty, value);
        }

        internal static ObservableCollection<BindableFeed> ConvertFeeds(SortedList<string, Feed> feeds)
        {
            ObservableCollection<BindableFeed> BFeeds = new();

            foreach (Feed FeedEntry in feeds.Values)
            {
                BindableFeed BFeed = ConvertFeed(FeedEntry);

                BFeeds.Add(BFeed);
            }

            return BFeeds;
        }

        internal static List<Feed> ConvertFeeds(ObservableCollection<BindableFeed> feeds)
        {
            List<Feed> DFeeds = new();

            foreach (BindableFeed BFeed in feeds)
            {
                Feed DFeed = ConvertFeed(BFeed);

                DFeeds.Add(DFeed);
            }

            return DFeeds;
        }

        internal static Feed ConvertFeed(BindableFeed feed)
        {
            return new()
            {
                DbId = feed.Id,
                FeedName = feed.Name,
                FeedUrl = feed.Url,
                LastRetrieved = feed.LastRetrieved.ToString(_InvariantFormat.UniversalSortableDateTimePattern),
                RetrieveLimitHrs = $"{feed.RetrieveLimitHrs}",
                RetentionDays = $"{feed.RetentionDays}",
            };
        }

        internal static BindableFeed ConvertFeed(Feed feed)
        {
            return new()
            {
                Id = feed.DbId,
                Name = feed.FeedName,
                Url = feed.FeedUrl,
                OriginalName = feed.FeedName,
                OriginalUrl = feed.FeedUrl,
                LastRetrieved = DateTime.Parse(feed.LastRetrieved),
                RetrieveLimitHrs = int.Parse(feed.RetrieveLimitHrs),
                RetentionDays = int.Parse(feed.RetentionDays),
            };
        }

        internal static BindableFeed ConvertFeedNarrow(Feed feed)
        {
            return new()
            {
                /*
                 * Do not bring over these fields:
                    OriginalName = feed.FeedName,
                    OriginalUrl = feed.FeedUrl,
                 * They are used as change detection values and must remain constant.
                 */
                Id = feed.DbId,
                Name = feed.FeedName,
                Url = feed.FeedUrl,
                LastRetrieved = DateTime.Parse(feed.LastRetrieved),
                RetrieveLimitHrs = int.Parse(feed.RetrieveLimitHrs),
                RetentionDays = int.Parse(feed.RetentionDays),
            };
        }
    }
}
