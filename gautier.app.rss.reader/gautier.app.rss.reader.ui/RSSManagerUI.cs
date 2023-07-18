using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using gautier.app.rss.reader.ui.UIData;
using gautier.rss.data;

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
            MaxLength = 800
        };

        private readonly TextBox _FeedUrl = new()
        {
            MaxLength = 10000
        };

        private readonly Slider _RetrieveLimitHrs = new()
        {
            Minimum = 1,
            Maximum = 14,
            Value = 1,
            TickFrequency = 1,
            Ticks = { 1, 2, 3, 4, 8, 10, 14 },
            IsSnapToTickEnabled = true,
            TickPlacement = TickPlacement.Both,
            AutoToolTipPlacement = AutoToolTipPlacement.BottomRight,
        };

        private readonly Slider _RetentionDays = new()
        {
            Minimum = 1,
            Maximum = 60,
            Value = 45,
            TickFrequency = 1,
            Ticks = { 1, 3, 6, 9, 10, 20, 30, 45, 60 },
            IsSnapToTickEnabled = true,
            TickPlacement = TickPlacement.Both,
            AutoToolTipPlacement = AutoToolTipPlacement.BottomRight,
        };

        private readonly StackPanel _FeedOptionsPanel = new()
        {
            FlowDirection = FlowDirection.RightToLeft,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Horizontal,
            Background = Brushes.IndianRed,
        };

        private readonly Button _NewButton = new()
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Background = Brushes.Orange,
            BorderBrush = Brushes.OrangeRed,
            BorderThickness = new Thickness(1),
            Content = "New Feed",
            Margin = new Thickness(4),
            Padding = new Thickness(4)
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
            Content = "Save Feed",
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
            Content = "Delete Feed",
            Margin = new Thickness(4),
            Padding = new Thickness(4)
        };

        private SortedList<string, Feed> _OriginalFeeds;

        private ObservableCollection<BindableFeed> _Feeds = new();

        public ObservableCollection<BindableFeed> Feeds => _Feeds;

        private readonly DataGrid _FeedsGrid = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            AutoGenerateColumns = false,
            IsReadOnly = true,
            SelectionMode = DataGridSelectionMode.Single,
        };

        private BindableFeed CurrentFeed
        {
            get => _FeedsGrid.SelectedItem as BindableFeed;
        }

        private readonly Grid _UIRoot = new();

        private readonly Grid _FeedInputGrid = new();

        public RSSManagerUI()
        {
            _OriginalFeeds = FeedDataExchange.GetAllFeeds(FeedConfiguration.SQLiteDbConnectionString);

            Initialized += RSSManagerUI_Initialized;

            return;
        }

        private void RSSManagerUI_Initialized(object? sender, EventArgs e)
        {
            Title = "Gautier RSS | Feed Manager";
            MinHeight = 680;
            MinWidth = 1330;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.Manual;
            WindowState = WindowState.Maximized;

            LayoutFeedInput();

            LayoutFeedsGrid();

            LayoutFeedOptionButtons();

            LayoutFeedUI();

            Content = _UIRoot;

            _FeedsGrid.SelectionChanged += FeedsGrid_SelectionChanged;
            _DeleteButton.Click += DeleteButton_Click;
            _SaveButton.Click += SaveButton_Click;
            _NewButton.Click += NewButton_Click;

            if (_Feeds.Count > 0)
            {
                _FeedsGrid.SelectedIndex = 0;
            }

            _FeedName.Focus();

            return;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _FeedsGrid.SelectedIndex = -1;

            ResetInput();

            _FeedName.Focus();

            return;
        }

        private BindableFeed ResetInput()
        {
            BindableFeed BFeed = new();

            _FeedName.Text = BFeed.Name;
            _FeedUrl.Text = BFeed.Url;
            _RetrieveLimitHrs.Value = BFeed.RetrieveLimitHrs;
            _RetentionDays.Value = BFeed.RetentionDays;

            return BFeed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            BindableFeed CFeed = CurrentFeed ?? new();

            bool InputIsValid = CheckInput(CFeed);

            if (InputIsValid == false)
            {
                return;
            }

            /*
             * The assignment action here should simultaneously update the Data Grid.
             */
            CFeed.Name = _FeedName.Text;
            CFeed.Url = _FeedUrl.Text;
            CFeed.RetrieveLimitHrs = Convert.ToInt32(_RetrieveLimitHrs.Value);
            CFeed.RetentionDays = Convert.ToInt32(_RetentionDays.Value);

            /*
             * Set the date/time back a few days to trigger download of the feed
             *    when it is newly added.
             */
            if (CFeed.Id < 1)
            {
                CFeed.LastRetrieved = DateTime.Today.AddDays(-4);
            }

            Feed DbInputFeed = BindableFeed.ConvertFeed(CFeed);

            Feed UpdatedFeed = FeedDataExchange.UpdateFeedConfigurationInDatabase(FeedConfiguration.FeedDbFilePath, DbInputFeed);

            bool InputAndUpdatedFeedsMatch = DbInputFeed.FeedName == UpdatedFeed.FeedName;

            if (InputAndUpdatedFeedsMatch)
            {
                bool Found = false;

                foreach (BindableFeed FeedEntry in _Feeds)
                {
                    Found = UpdatedFeed.FeedName == FeedEntry.Name;

                    if (Found)
                    {
                        break;
                    }
                }

                if (Found == false)
                {
                    /*
                     * Downside to using ConvertFeed(UFeed) is loss of original name and original url used to update the main UI.
                     * Do not use BindableFeed.ConvertFeed(UFeed);
                     */
                    BindableFeed FeedOutput = BindableFeed.ConvertFeedNarrow(UpdatedFeed);

                    _Feeds.Add(CFeed);

                    _FeedsGrid.SelectedItem = FeedOutput;
                }
            }

            return;
        }

        private bool CheckInput(BindableFeed feed)
        {
            int ErrorCount = 0;

            StringBuilder Errors = new();

            Action<string> DispatchError = (string errorMessage) =>
            {
                Errors.AppendLine(errorMessage);

                ErrorCount++;
            };

            string FeedName = _FeedName.Text;
            string FeedUrl = _FeedUrl.Text;

            if (string.IsNullOrWhiteSpace(FeedName))
            {
                DispatchError($"Feed Name cannot be blank.");
            }

            if (string.IsNullOrWhiteSpace(FeedUrl))
            {
                DispatchError($"Feed Url cannot be blank.");
            }

            /*
             * Unique Name Validation.
             */
            SortedList<string, int> Names = new();

            foreach (BindableFeed FeedEntry in _Feeds)
            {
                string EntryFeedName = FeedEntry.Name;

                if (Names.ContainsKey(EntryFeedName))
                {
                    int RowCount = Names[EntryFeedName];

                    RowCount++;

                    Names[EntryFeedName] = RowCount;
                }
                else
                {
                    if (feed.Id != FeedEntry.Id)
                    {
                        Names[EntryFeedName] = 1;
                    }
                }
            }

            if (Names.ContainsKey(FeedName))
            {
                DispatchError($"Feed Name must be unique.");
            }

            /*
             * Unique Url Validation.
             */
            SortedList<string, int> Urls = new();

            foreach (BindableFeed FeedEntry in _Feeds)
            {
                string EntryUrl = FeedEntry.Url;

                if (Urls.ContainsKey(EntryUrl))
                {
                    int RowCount = Urls[EntryUrl];

                    RowCount++;

                    Urls[EntryUrl] = RowCount;
                }
                else
                {
                    if (feed.Id != FeedEntry.Id)
                    {
                        Urls[EntryUrl] = 1;
                    }
                }
            }

            if (Urls.ContainsKey(FeedUrl))
            {
                DispatchError($"Feed Url must be unique.");
            }

            bool IsValidUrl = RSSNetClient.ValidateUrlIsHttpOrHttps(FeedUrl);

            if (IsValidUrl == false)
            {
                DispatchError($"Feed Url must be in the proper format starting with http:// or https://.");
            }

            bool IsValid = ErrorCount == 0;

            if (IsValid == false)
            {
                MessageBox.Show(Errors.ToString());
            }

            return IsValid;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            BindableFeed CFeed = CurrentFeed;

            Feed DFeed = BindableFeed.ConvertFeed(CFeed);

            if (DFeed != null && DFeed.DbId > 0)
            {
                bool IsDeleted = FeedDataExchange.RemoveFeedFromDatabase(FeedConfiguration.FeedDbFilePath, DFeed.DbId);

                if (IsDeleted)
                {
                    _Feeds.Remove(CFeed);

                    if (_Feeds.Count > 0)
                    {
                        _FeedsGrid.SelectedIndex = 0;
                    }
                }
            }

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
                if (El is FrameworkElement)
                {
                    FrameworkElement FEl = El as FrameworkElement;

                    FEl.Margin = new Thickness(12);
                    FEl.VerticalAlignment = VerticalAlignment.Center;
                }

                _FeedInputGrid.Children.Add(El);
            }

            int HorizontalChildrenCount = _FeedInputGrid.Children.Count;

            for (int ColumnIndex = 0; ColumnIndex < HorizontalChildrenCount; ColumnIndex++)
            {
                ColumnDefinition ColDef = new()
                {
                    Width = new(1, GridUnitType.Auto)
                };

                if (_FeedInputGrid.Children[ColumnIndex] is not Label)
                {
                    switch (ColumnIndex)
                    {
                        case 5:
                        case 7:
                            ColDef.Width = new(0.2, GridUnitType.Star);
                            break;
                        default:
                            ColDef.Width = new(1, GridUnitType.Star);
                            break;
                    }
                }

                _FeedInputGrid.ColumnDefinitions.Add(ColDef);

                Grid.SetColumn(_FeedInputGrid.Children[ColumnIndex], ColumnIndex);
            }

            return;
        }

        private void LayoutFeedOptionButtons()
        {
            UIElement[] ReaderOptionElements =
            {
                _NewButton,
                _SaveButton,
                _DeleteButton,
            };

            foreach (UIElement El in ReaderOptionElements)
            {
                _FeedOptionsPanel.Children.Add(El);
            }

            return;
        }

        private void LayoutFeedsGrid()
        {
            KeyValuePair<string, string>[] GridColumnNames =
            {
                new KeyValuePair<string, string>("Name", "Name"),
                new KeyValuePair<string, string>("Url", "Url"),
                new KeyValuePair < string, string >("Retrieve Limit (Hrs)", "RetrieveLimitHrs"),
                new KeyValuePair < string, string >("Retention Days", "RetentionDays"),
                new KeyValuePair < string, string >("Last Retrieved", "LastRetrieved")
            };

            for (int ColI = 0; ColI < GridColumnNames.Length; ColI++)
            {
                DataGridColumn Col;

                KeyValuePair<string, string> ColumnNamePair = GridColumnNames[ColI];

                DataGridLength ColumnWidth = new(1, DataGridLengthUnitType.Star);

                switch (ColI)
                {
                    case 2:
                    case 3:
                        ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
                        break;
                }

                Col = new DataGridTextColumn()
                {
                    Header = ColumnNamePair.Key,
                    Binding = new Binding(ColumnNamePair.Value),
                    Width = ColumnWidth,
                };

                if (ColI == 1)
                {
                    Col = new DataGridHyperlinkColumn()
                    {
                        Header = ColumnNamePair.Key,
                        Binding = new Binding(ColumnNamePair.Value),
                        Width = ColumnWidth,
                    };
                }

                _FeedsGrid.Columns.Add(Col);
            }

            _Feeds = BindableFeed.ConvertFeeds(_OriginalFeeds);

            _FeedsGrid.ItemsSource = _Feeds;

            return;
        }

        private void FeedsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
             * ResetInput has side-effects. It clears the input fields and returns a default instance.
             * Handles cases where changing selected index is not recognized by the Grid.
             * Such a case can be when there are no rows yet.
             */
            BindableFeed BFeed = CurrentFeed ?? ResetInput();

            _FeedName.Text = BFeed.Name;
            _FeedUrl.Text = BFeed.Url;
            _RetrieveLimitHrs.Value = BFeed.RetrieveLimitHrs;
            _RetentionDays.Value = BFeed.RetentionDays;

            _FeedName.Focus();

            return;
        }

        private void LayoutFeedUI()
        {
            UIElement[] Els =
            {
                _FeedInputGrid,
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
                if (RowIndex == 1)
                {
                    RowDef.Height = new(4, GridUnitType.Star);
                }

                _UIRoot.RowDefinitions.Add(RowDef);

                Grid.SetRow(Els[RowIndex], RowIndex);
            }

            return;
        }
    }
}
