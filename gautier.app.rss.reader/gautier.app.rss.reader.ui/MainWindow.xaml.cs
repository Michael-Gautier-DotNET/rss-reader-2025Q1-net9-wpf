using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using gautier.rss.data;

namespace gautier.app.rss.reader.ui
{
    /// <summary>
    /// Root layout frame for RSS Reader
    /// </summary>
    public partial class MainWindow : Window
    {
        private TabControl reader_tabs = null;
        private List<TabItem> reader_tab_items = new();

        private Grid reader_feed_detail = new();

        private TextBlock reader_feed_name = new();
        private TextBlock reader_headline = new();
        private WebBrowser reader_article = new();
        private FeedArticle article = null;

        private readonly string empty_article = @"<html><head><title>test</title></head><body><div>&nbsp;</div></body></html>";

        private SortedList<string, Feed> feeds = null;
        private SortedList<string, SortedList<string, FeedArticle>> feeds_articles = null;
        private int feed_index = -1;

        private BackgroundWorker WindowInitializationTask = new();
        private BackgroundWorker FeedArticlesRetrieveTask = new();

        public MainWindow()
        {
            InitializeComponent();

            return;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            InitializeValues();

            WindowInitializationTask.DoWork += WindowInitializationTask_DoWork;
            WindowInitializationTask.RunWorkerCompleted += WindowInitializationTask_RunWorkerCompleted;

            WindowInitializationTask.RunWorkerAsync();

            FeedArticlesRetrieveTask.DoWork += FeedArticlesRetrieveTask_DoWork;
            FeedArticlesRetrieveTask.RunWorkerCompleted += FeedArticlesRetrieveTask_RunWorkerCompleted;

            return;
        }

        private void WindowInitializationTask_DoWork(object sender, DoWorkEventArgs e)
        {
            feeds = feed_data_exchange.get_all_feeds(Properties.Settings.Default.rss_connection_string_sqlserver);

            return;
        }

        private void WindowInitializationTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitializeFeedConfigurations();

            LayoutHeadlinesSection();

            LayoutDetailSection();

            FeedArticlesRetrieveTask.RunWorkerAsync();

            return;
        }

        private void FeedArticlesRetrieveTask_DoWork(object sender, DoWorkEventArgs e)
        {
            feeds_articles = feed_data_exchange.get_all_feed_articles(Properties.Settings.Default.rss_connection_string_sqlserver);

            feed_index = feeds_articles.Count > -1 ? 0 : -1;

            return;
        }

        private void FeedArticlesRetrieveTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ApplyFeed();

            reader_tabs.SelectionChanged += ReaderTabs_SelectionChanged;

            return;
        }

        private void InitializeFeedConfigurations()
        {
            var feed_names = feeds.Keys;

            foreach (var feed_name in feed_names)
            {
                var reader_tab = new TabItem
                {
                    Header = feed_name,
                    Content = new ListBox
                    {
                        DisplayMemberPath = "headline_text",
                        SelectedValuePath = "article_url",
                    }
                };

                reader_tab_items.Add(reader_tab);
                reader_tabs.Items.Add(reader_tab);

                (reader_tab.Content as ListBox).SelectionChanged += Headline_SelectionChanged;
            }

            return;
        }

        private void InitializeValues()
        {
            reader_tabs = new TabControl
            {
                Background = Brushes.Orange,
                BorderBrush = Brushes.Beige,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(1)
            };

            return;
        }

        private void LayoutDetailSection()
        {
            foreach (var ch in new UIElement[] { reader_feed_name, reader_headline, reader_article })
            {
                reader_feed_detail.Children.Add(ch);
            }

            var vertical_children_count = reader_feed_detail.Children.Count;

            for (var row_index = 0; row_index < vertical_children_count; row_index++)
            {
                var row_def = new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                };

                if (row_index == 2)
                {
                    row_def.Height = new GridLength(4, GridUnitType.Star);
                }

                reader_feed_detail.RowDefinitions.Add(row_def);

                Grid.SetRow(reader_feed_detail.Children[row_index], row_index);
            }

            return;
        }

        private void LayoutHeadlinesSection()
        {
            foreach (var ch in new UIElement[] { reader_tabs, reader_feed_detail })
            {
                root_content.Children.Add(ch);
            }

            var horizontal_children_count = root_content.Children.Count;

            for (var col_index = 0; col_index < horizontal_children_count; col_index++)
            {
                var col_def = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                root_content.ColumnDefinitions.Add(col_def);

                Grid.SetColumn(root_content.Children[col_index], col_index);
            }

            return;
        }

        private bool IsFeedIndexValid
        {
            get
            {
                return feed_index > -1 && feed_index < reader_tab_items.Count;
            }
        }

        private TabItem ReaderTab
        {
            get
            {
                return IsFeedIndexValid ? reader_tab_items[feed_index] : null;
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

        private void ApplyArticle(FeedArticle article)
        {
            this.article = article;

            var article_text = article?.article_text ?? empty_article;

            reader_headline.Text = article?.headline_text ?? string.Empty;
            reader_article.NavigateToString(article_text);

            return;
        }

        private void ReaderTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            feed_index = reader_tabs.SelectedIndex;

            ApplyFeed();

            return;
        }

        private void ApplyFeed()
        {
            if (IsFeedIndexValid)
            {
                var feed_name = reader_feed_name.Text = $"{ReaderTab.Header}";

                if (feeds_articles.ContainsKey(feed_name))
                {
                    if (FeedHeadlines != null && FeedHeadlines.HasItems == false)
                    {
                        var indexed_feed_articles = feeds_articles[feed_name];

                        FeedHeadlines.ItemsSource = indexed_feed_articles.Values;
                    }
                }
            }

            ApplyArticle(FeedHeadlines?.SelectedItem as FeedArticle);

            return;
        }
    }
}
