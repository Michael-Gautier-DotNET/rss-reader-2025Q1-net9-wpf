using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using gautier.rss.data;
using gautier.rss.data.RSSDb;

namespace gautier.app.rss.reader.ui
{
    /// <summary>
    /// Root layout frame for RSS Reader
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string _FeedDbFilePath = @"C:\RSSFeeds\feeds_db\rss.db";
        private static readonly string _SQLiteDbConnectionString = SQLUtil.GetSQLiteConnectionString(_FeedDbFilePath, 3);

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
            _Feeds = FeedDataExchange.GetAllFeeds(_SQLiteDbConnectionString);

            return;
        }

        private void WindowInitializationTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Action UIThreadAction = () =>
            {
                if (_FeedsInitialized == false)
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
                TabItem ReaderTab = new TabItem
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

        private void LayoutDetailSection()
        {
            foreach (UIElement El in new UIElement[] { _ReaderFeedName, _ReaderHeadline, _ReaderArticle })
            {
                _ReaderFeedDetail.Children.Add(El);
            }

            int VerticalChildrenCount = _ReaderFeedDetail.Children.Count;

            for (int RowIndex = 0; RowIndex < VerticalChildrenCount; RowIndex++)
            {
                RowDefinition RowDef = new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                };

                if (RowIndex == 2)
                {
                    RowDef.Height = new GridLength(4, GridUnitType.Star);
                }

                _ReaderFeedDetail.RowDefinitions.Add(RowDef);

                Grid.SetRow(_ReaderFeedDetail.Children[RowIndex], RowIndex);
            }

            return;
        }

        private void LayoutHeadlinesSection()
        {
            foreach (UIElement El in new UIElement[] { _ReaderTabs, _ReaderFeedDetail })
            {
                UIRoot.Children.Add(El);
            }

            int HorizontalChildrenCount = UIRoot.Children.Count;

            for (int ColumnIndex = 0; ColumnIndex < HorizontalChildrenCount; ColumnIndex++)
            {
                var ColDef = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                UIRoot.ColumnDefinitions.Add(ColDef);

                Grid.SetColumn(UIRoot.Children[ColumnIndex], ColumnIndex);
            }

            return;
        }

        private bool IsFeedIndexValid
        {
            get
            {
                return _FeedIndex > -1 && _FeedIndex < _ReaderTabItems.Count;
            }
        }

        private TabItem ReaderTab
        {
            get
            {
                return IsFeedIndexValid ? _ReaderTabItems[_FeedIndex] : null;
            }
        }

        private ListBox? FeedHeadlines
        {
            get
            {
                return ReaderTab?.Content as ListBox;
            }
        }

        private void Headline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyArticle(FeedHeadlines.SelectedItem as FeedArticle);

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
                    _FeedsArticles = FeedDataExchange.GetFeedArticles(_SQLiteDbConnectionString, FeedName);

                    var IndexedFeedArticles = _FeedsArticles.Values;

                    FeedHeadlines.ItemsSource = IndexedFeedArticles;
                }
            }

            ApplyArticle(FeedHeadlines?.SelectedItem as FeedArticle);

            return;
        }
    }
}
