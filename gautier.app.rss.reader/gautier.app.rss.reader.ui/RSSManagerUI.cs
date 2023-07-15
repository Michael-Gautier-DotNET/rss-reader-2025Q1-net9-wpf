using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private readonly StackPanel _FeedOptionsPanel = new()
        {
            FlowDirection = FlowDirection.RightToLeft,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Horizontal,
            Background = Brushes.IndianRed,
        };

        private readonly Button _SaveButton = new()
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Background = Brushes.Orange,
            BorderBrush = Brushes.OrangeRed,
            BorderThickness = new Thickness(1),
            Content = "Manage Feeds",
            Margin = new Thickness(4),
            Padding = new Thickness(4)
        };

        private readonly Button _DeleteButton = new()
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Background = Brushes.Orange,
            BorderBrush = Brushes.OrangeRed,
            BorderThickness = new Thickness(1),
            Content = "View Article",
            Margin = new Thickness(4),
            Padding = new Thickness(4)
        };

        private readonly DataGrid _FeedsGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch
        };

        private readonly Grid _UIRoot = new()
        {
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

            Content = _UIRoot;

            LayoutFeedInput();

            LayoutFeedOptionButtons();

            LayoutFeedUI();

            return;
        }

        private void LayoutFeedInput()
        {
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

                if (_UIRoot.Children[ColumnIndex] is not Label)
                {
                    ColDef.Width = new(1, GridUnitType.Star);
                }

                _UIRoot.ColumnDefinitions.Add(ColDef);

                Grid.SetColumn(_UIRoot.Children[ColumnIndex], ColumnIndex);
            }

            return;
        }

        private void LayoutFeedOptionButtons()
        {
            UIElement[] ReaderOptionElements =
            {
                    _SaveButton,
                    _DeleteButton,
                };

            foreach (UIElement El in ReaderOptionElements)
            {
                _FeedOptionsPanel.Children.Add(El);
            }

            return;
        }

        private void LayoutFeedUI()
        {
            UIElement[] Els =
            {
                _FeedsGrid,
                _FeedOptionsPanel
            };

            foreach (UIElement El in Els)
            {
                _UIRoot.Children.Add(El);
            }

            int VerticalChildrenCount = Els.Length;

            for (int RowIndex = 0; RowIndex < VerticalChildrenCount; RowIndex++)
            {
                RowDefinition RowDef = new()
                {
                    Height = new(1, GridUnitType.Auto)
                };

                /*
                 * Feeds Grid
                 */
                if (RowIndex == 0)
                {
                    RowDef.Height = new(4, GridUnitType.Star);
                }

                _UIRoot.RowDefinitions.Add(RowDef);

                /*
                 * The Feed input text boxes already set to row 1 (index == 0).
                 * Next available row starts at unused row + 1.
                 */
                int GridRow = RowIndex + 1;

                UIElement El = Els[RowIndex];

                Grid.SetRow(El, GridRow);
                Grid.SetColumnSpan(El, _UIRoot.ColumnDefinitions.Count);
            }

            return;
        }
    }
}
