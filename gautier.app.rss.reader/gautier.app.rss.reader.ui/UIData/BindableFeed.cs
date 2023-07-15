using System;
using System.ComponentModel;
using System.Windows;

using gautier.rss.data;

namespace gautier.app.rss.reader.ui.UIData
{
    public class BindableFeed : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(BindableFeed), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(BindableFeed), new PropertyMetadata(string.Empty, UrlPropertyChanged));

        public static readonly DependencyProperty LastRetrievedProperty = DependencyProperty.Register("LastRetrieved", typeof(DateTime), typeof(BindableFeed), new PropertyMetadata(DateTime.Now));

        public static readonly DependencyProperty RetrieveLimitHrsProperty = DependencyProperty.Register("RetrieveLimitHrs", typeof(int), typeof(BindableFeed), new PropertyMetadata(1));

        public static readonly DependencyProperty RetentionDaysProperty = DependencyProperty.Register("RetentionDays", typeof(int), typeof(BindableFeed), new PropertyMetadata(45));

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler UrlValidationFailed;

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

        public string LastRetrieved
        {
            get => $"{GetValue(LastRetrievedProperty)}";
            set => SetValue(LastRetrievedProperty, value);
        }

        public string RetrieveLimitHrs
        {
            get => $"{GetValue(RetrieveLimitHrsProperty)}";
            set => SetValue(RetrieveLimitHrsProperty, value);
        }

        public string RetentionDays
        {
            get => $"{GetValue(RetentionDaysProperty)}";
            set => SetValue(RetentionDaysProperty, value);
        }

        private static void UrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableFeed BFeed = d as BindableFeed;

            string PreviousUrl = e.OldValue as string;
            string NewUrl = e.NewValue as string;

            bool IsValidUrl = RSSNetClient.ValidateUrlIsHttpOrHttps(NewUrl);

            if (IsValidUrl)
            {
                BFeed.Url = NewUrl;
                BFeed.OriginalUrl = PreviousUrl;

                BFeed.OnPropertyChanged(nameof(Url));
            }
            else
            {
                BFeed.Url = PreviousUrl;
                BFeed.OnUrlValidationFailed(PreviousUrl);
            }

            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return;
        }

        protected virtual void OnUrlValidationFailed(string previousUrl)
        {
            UrlValidationFailed?.Invoke(this, EventArgs.Empty);

            return;
        }
    }
}
