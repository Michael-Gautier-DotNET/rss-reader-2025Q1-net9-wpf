using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using gautier.app.rss.reader.ui.UIData;
using gautier.rss.data;

namespace gautier.app.rss.reader.ui
{
    /// <summary>
    /// Root layout frame for RSS Reader
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TabControl _ReaderTabs = new()
        {
            Background = Brushes.Orange,
            BorderBrush = Brushes.Beige,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(1)
        };

        private readonly List<TabItem> _ReaderTabItems = new();

        private readonly Grid _ReaderFeedDetail = new();

        private readonly TextBlock _ReaderFeedName = new();
        private readonly TextBlock _ReaderHeadline = new();
        private readonly WebBrowser _ReaderArticle = new();

        private static readonly string _EmptyArticle = @"<html><head><title>test</title></head><body><div>&nbsp;</div></body></html>";

        private bool _FeedsInitialized = false;

        private SortedList<string, Feed> _Feeds = null;
        private SortedList<string, FeedArticle> _FeedsArticles = null;
        private int _FeedIndex = -1;

        private readonly TimeSpan _QuickTimeSpan = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _MidTimeSpan = TimeSpan.FromMinutes(1);

        private DispatcherTimer _FeedUpdateTimer;

        private readonly BackgroundWorker _WindowInitializationTask = new();

        private readonly StackPanel _ReaderOptionsPanel = new()
        {
            FlowDirection = FlowDirection.RightToLeft,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Orientation = Orientation.Horizontal,
            Background = Brushes.IndianRed,
        };

        private readonly Button _ReaderManagerButton = new()
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

        private readonly Button _ReaderArticleLaunchButton = new()
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

        public MainWindow()
        {
            InitializeComponent();

            return;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _FeedUpdateTimer = new()
            {
                Interval = _QuickTimeSpan
            };

            _FeedUpdateTimer.Tick += UpdateFeedsOnInterval;

            _WindowInitializationTask.DoWork += WindowInitializationTask_DoWork;
            _WindowInitializationTask.RunWorkerCompleted += WindowInitializationTask_RunWorkerCompleted;

            _FeedUpdateTimer.Start();

            _ReaderManagerButton.Click += ReaderManagerButton_Click;
            _ReaderArticleLaunchButton.Click += ReaderArticleLaunchButton_Click;

            return;
        }

        private void ReaderArticleLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            //Show article in system's web browser.

            if (Article != null)
            {
                string ArticleUrl = Article.ArticleUrl;

                ProcessStartInfo UrlActivator = new()
                {
                    FileName = ArticleUrl,
                    UseShellExecute = true,
                };

                Process.Start(UrlActivator);
            }

            return;
        }

        private void ReaderManagerButton_Click(object sender, RoutedEventArgs e)
        {
            /*Stop any timers and RSS feed article updates.*/

            RSSManagerUI UI = new();

            _FeedUpdateTimer.Stop();

            UI.ShowDialog();

            CheckRSSManagerUIUpdates(UI);

            _FeedUpdateTimer.Start();

            return;
        }

        private void CheckRSSManagerUIUpdates(RSSManagerUI ui)
        {
            ObservableCollection<BindableFeed> BFeeds = ui.Feeds;

            int BFeedCount = BFeeds.Count;
            int DFeedCount = _Feeds.Count;

            /*
             * Check for feed name changes.
             */
            for (int BIndex = 0; BIndex < BFeedCount; BIndex++)
            {
                BindableFeed BFeed = BFeeds[BIndex];

                string UpdatedFeedName = BFeed.Name;
                string OriginalFeedName = BFeed.OriginalName;

                for (int DIndex = 0; DIndex < DFeedCount; DIndex++)
                {
                    string FeedName = _Feeds.Keys[DIndex];

                    if (OriginalFeedName == FeedName && UpdatedFeedName != FeedName)
                    {
                        TabItem FoundTab = FindRSSFeedTab(FeedName);

                        if ($"{FoundTab.Header}" == FeedName)
                        {
                            FoundTab.Header = UpdatedFeedName;
                        }

                        Feed FeedEntry = _Feeds[FeedName];

                        FeedEntry.FeedName = UpdatedFeedName;

                        _Feeds.Remove(OriginalFeedName);

                        _Feeds[UpdatedFeedName] = FeedEntry;
                    }
                }
            }

            /*
             * Check for deletion.
             * Check this last since the comparison is done based on name.
             * The collection SortedList<string, Feed> is updated prior to this process putting it in 
             * a reliable state for comparison with the database.
             * 
             * I could compare the collections ObservableCollection<BindableFeed> vs SortedList<string, Feed>
             * to detect deletions but when it comes to deleted data, you want to be absolutely sure.
             * 
             * Check the database and prune any feeds from the UI not represented in the database.
             */
            PruneFeedsFollowingManagementUpdate();



            return;
        }

        private TabItem FindRSSFeedTab(string name)
        {
            ItemCollection Tabs = _ReaderTabs.Items;

            TabItem FoundTab = new();

            foreach (TabItem Tab in Tabs)
            {
                string TabHeader = $"{Tab.Header}";

                if (TabHeader == name)
                {
                    FoundTab = Tab;

                    break;
                }
            }

            return FoundTab;
        }

        private void PruneFeedsFollowingManagementUpdate()
        {
            SortedList<string, Feed> DbFeeds = FeedDataExchange.GetAllFeeds(FeedConfiguration.SQLiteDbConnectionString);

            List<string> FeedNames = new(_Feeds.Keys);

            foreach (string FeedName in FeedNames)
            {
                if (DbFeeds.ContainsKey(FeedName) == false)
                {
                    _Feeds.Remove(FeedName);

                    TabItem FoundTab = FindRSSFeedTab(FeedName);

                    if ($"{FoundTab.Header}" == FeedName)
                    {
                        _ReaderTabItems.Remove(FoundTab);
                        _ReaderTabs.Items.Remove(FoundTab);
                    }
                }
            }

            return;
        }

        private void UpdateFeedsOnInterval(object sender, EventArgs e)
        {
            _FeedUpdateTimer?.Stop();

            bool IsUsingQuickTimeSpan = _FeedUpdateTimer?.Interval == _QuickTimeSpan;

            if (IsUsingQuickTimeSpan)
            {
                _FeedUpdateTimer.Interval = _MidTimeSpan;
            }

            _WindowInitializationTask.RunWorkerAsync();

            return;
        }

        private void WindowInitializationTask_DoWork(object sender, DoWorkEventArgs e)
        {
            _Feeds = FeedDataExchange.GetAllFeeds(FeedConfiguration.SQLiteDbConnectionString);

            if (_FeedsInitialized)
            {
                DownloadFeeds();
            }

            return;
        }

        private void WindowInitializationTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Action UIThreadAction = () =>
            {
                if (_FeedsInitialized)
                {
                    ApplyNewFeeds();
                }
                else
                {
                    _FeedIndex = _Feeds.Count > -1 ? 0 : -1;

                    InitializeFeedConfigurations();

                    LayoutHeadlinesSection();

                    LayoutDetailSection();

                    ApplyFeed();

                    _FeedsInitialized = true;

                    _ReaderTabs.SelectionChanged += ReaderTabs_SelectionChanged;
                }
            };

            Dispatcher.BeginInvoke(UIThreadAction);

            if (_FeedUpdateTimer?.IsEnabled == false)
            {
                _FeedUpdateTimer?.Stop();
            }

            return;
        }

        private void InitializeFeedConfigurations()
        {
            IList<string> FeedNames = _Feeds.Keys;

            foreach (string FeedName in FeedNames)
            {
                TabItem ReaderTab = new()
                {
                    Header = FeedName,
                    Content = new ListBox
                    {
                        DisplayMemberPath = "HeadlineText",
                        SelectedValuePath = "ArticleUrl",
                    }
                };

                _ReaderTabItems.Add(ReaderTab);
                _ReaderTabs.Items.Add(ReaderTab);

                (ReaderTab.Content as ListBox).SelectionChanged += Headline_SelectionChanged;
            }

            return;
        }

        private void LayoutReaderOptionButtons()
        {
            UIElement[] ReaderOptionElements =
            {
                    _ReaderArticleLaunchButton,
                    _ReaderManagerButton,
                };

            foreach (UIElement El in ReaderOptionElements)
            {
                _ReaderOptionsPanel.Children.Add(El);
            }

            return;
        }

        private void LayoutDetailSection()
        {
            UIElement[] Els =
            {
                _ReaderFeedName,
                _ReaderHeadline,
                _ReaderArticle,
                _ReaderOptionsPanel
            };

            foreach (UIElement El in Els)
            {
                _ReaderFeedDetail.Children.Add(El);
            }

            int VerticalChildrenCount = _ReaderFeedDetail.Children.Count;

            for (int RowIndex = 0; RowIndex < VerticalChildrenCount; RowIndex++)
            {
                RowDefinition RowDef = new()
                {
                    Height = new(1, GridUnitType.Auto)
                };

                if (RowIndex == 2)
                {
                    RowDef.Height = new(4, GridUnitType.Star);
                }

                _ReaderFeedDetail.RowDefinitions.Add(RowDef);

                Grid.SetRow(_ReaderFeedDetail.Children[RowIndex], RowIndex);
            }

            LayoutReaderOptionButtons();

            return;
        }

        private void LayoutHeadlinesSection()
        {
            UIElement[] Els =
            {
                _ReaderTabs,
                _ReaderFeedDetail
            };

            foreach (UIElement El in Els)
            {
                UIRoot.Children.Add(El);
            }

            int HorizontalChildrenCount = UIRoot.Children.Count;

            for (int ColumnIndex = 0; ColumnIndex < HorizontalChildrenCount; ColumnIndex++)
            {
                ColumnDefinition ColDef = new()
                {
                    Width = new(1, GridUnitType.Star)
                };

                UIRoot.ColumnDefinitions.Add(ColDef);

                Grid.SetColumn(UIRoot.Children[ColumnIndex], ColumnIndex);
            }

            return;
        }

        private bool IsFeedIndexValid
        {
            get => _FeedIndex > -1 && _FeedIndex < _ReaderTabItems.Count;
        }

        private TabItem ReaderTab
        {
            get => IsFeedIndexValid ? _ReaderTabItems[_FeedIndex] : null;
        }

        private ListBox? FeedHeadlines
        {
            get => ReaderTab?.Content as ListBox;
        }

        private FeedArticle Article
        {
            get => FeedHeadlines.SelectedItem as FeedArticle;
        }

        private void Headline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyArticle(Article);

            return;
        }

        private void ApplyArticle(FeedArticle? article)
        {
            string ArticleText = _EmptyArticle;

            if (string.IsNullOrWhiteSpace(article?.ArticleText) == false)
            {
                ArticleText = article?.ArticleText;
            }
            else if (string.IsNullOrWhiteSpace(article?.ArticleSummary) == false)
            {
                ArticleText = article?.ArticleSummary;
            }

            _ReaderHeadline.Text = article?.HeadlineText ?? string.Empty;
            _ReaderArticle.NavigateToString(ArticleText);

            return;
        }

        private void ReaderTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _FeedIndex = _ReaderTabs.SelectedIndex;

            ApplyFeed();

            return;
        }

        private void ApplyFeed()
        {
            if (IsFeedIndexValid)
            {
                var FeedName = _ReaderFeedName.Text = $"{ReaderTab.Header}";

                if (FeedHeadlines != null && FeedHeadlines.HasItems == false)
                {
                    _FeedsArticles = FeedDataExchange.GetFeedArticles(FeedConfiguration.SQLiteDbConnectionString, FeedName);

                    var IndexedFeedArticles = _FeedsArticles.Values;

                    FeedHeadlines.ItemsSource = IndexedFeedArticles;
                }
            }

            ApplyArticle(FeedHeadlines?.SelectedItem as FeedArticle);

            return;
        }

        private void DownloadFeeds()
        {
            Feed[] FeedEntries = new List<Feed>(_Feeds.Values).ToArray();

            FeedEntries = FeedDataExchange.MergeFeedEntries(FeedConfiguration.FeedDbFilePath, FeedEntries);

            FeedFileConverter.CreateStaticFeedFiles(FeedConfiguration.FeedSaveDirectoryPath, FeedConfiguration.FeedDbFilePath, FeedEntries);

            FeedFileConverter.TransformStaticFeedFiles(FeedConfiguration.FeedSaveDirectoryPath, FeedEntries);

            FeedDataExchange.ImportStaticFeedFilesToDatabase(FeedConfiguration.FeedSaveDirectoryPath, FeedConfiguration.FeedDbFilePath, FeedEntries);

            return;
        }

        private void ApplyNewFeeds()
        {
            List<string> RSSFeedNames = new(_Feeds.Keys);

            foreach (string FeedName in RSSFeedNames)
            {
                Feed FeedEntry = _Feeds[FeedName];

                Console.WriteLine($"Feed:\t{FeedName}");
                Console.WriteLine($"\t\tLast Retrieved {FeedEntry.LastRetrieved} from {FeedEntry.FeedUrl}");
                Console.WriteLine($"\t\t\tConfiguration: Retrieve Limit Hrs {FeedEntry.RetrieveLimitHrs}, Retention Days {FeedEntry.RetentionDays}");
                /*
                 * Output article information.
                 */

                SortedList<string, FeedArticle> Articles = FeedDataExchange.GetFeedArticles(FeedConfiguration.SQLiteDbConnectionString, FeedName);

                List<string> ArticleUrls = new(Articles.Keys);

                foreach (string ArticleUrl in ArticleUrls)
                {
                    FeedArticle Article = Articles[ArticleUrl];

                    Console.WriteLine($"URL: {ArticleUrl}");
                    Console.WriteLine($"\t\tHeadline: {Article.HeadlineText}");
                    /*Console.WriteLine($"\t\t\t\tText: {Article.ArticleSummary}");*/
                    Console.WriteLine($"\n\n\n");
                }
            }

            return;
        }
    }
}
