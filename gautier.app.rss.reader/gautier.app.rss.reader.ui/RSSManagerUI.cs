using System;
using System.Windows;
using System.Windows.Controls;

namespace gautier.app.rss.reader.ui
{
    public partial class RSSManagerUI : Window
    {
        private readonly Label _FeedNameLabel = new()
        {
            Content = "Feed Name"
        };

        private readonly Label _FeedUrlLabel = new()
        {
            Content = "Feed Url"
        };

        private readonly Label _RetrieveLimitHrsLabel = new()
        {
            Content = "Update Loop (in Hours)"
        };

        private readonly Label _RetentionDaysLabel = new()
        {
            Content = "Article Expires (in Days)"
        };

        private readonly TextBox _FeedName = new()
        {
        };

        private readonly TextBox _FeedUrl = new()
        {
        };

        private readonly TextBox _RetrieveLimitHrs = new()
        {
        };

        private readonly TextBox _RetentionDays = new()
        {
        };

        private readonly Grid _UIRoot = new()
        {
            ShowGridLines = true,
            VerticalAlignment = VerticalAlignment.Top
        };

        public RSSManagerUI()
        {
            Initialized += RSSManagerUI_Initialized;

            return;
        }

        private void RSSManagerUI_Initialized(object? sender, EventArgs e)
        {
            Title = "Gautier RSS | Feed Manager";
            Height = 450;
            Width = 800;
            SizeToContent = SizeToContent.Manual;
            WindowState = WindowState.Maximized;

            UIElement[] Els =
            {
                _FeedNameLabel,
                _FeedName,
                _FeedUrlLabel,
                _FeedUrl,
                _RetrieveLimitHrsLabel,
                _RetrieveLimitHrs,
                _RetentionDaysLabel,
                _RetentionDays,
            };

            foreach (UIElement El in Els)
            {
                (El as FrameworkElement).Margin = new Thickness(12);

                _UIRoot.Children.Add(El);
            }

            int HorizontalChildrenCount = _UIRoot.Children.Count;

            for (int ColumnIndex = 0; ColumnIndex < HorizontalChildrenCount; ColumnIndex++)
            {
                ColumnDefinition ColDef = new()
                {
                    Width = new(1, GridUnitType.Auto)
                };

                if(_UIRoot.Children[ColumnIndex] is not Label)
                {
                    ColDef.Width = new(1, GridUnitType.Star);
                }

                _UIRoot.ColumnDefinitions.Add(ColDef);

                Grid.SetColumn(_UIRoot.Children[ColumnIndex], ColumnIndex);
            }

            Content = _UIRoot;

            return;
        }
    }
}
