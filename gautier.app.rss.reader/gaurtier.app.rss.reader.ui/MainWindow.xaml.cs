using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gautier.app.rss.reader.ui
{
    /// <summary>
    /// Root layout frame for RSS Reader
    /// </summary>
    public partial class MainWindow : Window
    {
        private TabControl _reader_tabs = null;
        private List<TabItem> _reader_tab_items = new List<TabItem>();

        private StackPanel _reader_feed_detail = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        private TextBlock _reader_feed_name = new TextBlock();
        private TextBlock _reader_headline = new TextBlock();
        private WebBrowser _reader_article = new WebBrowser();
        private feed_article _article = null;

        private const string _empty_article = @"<html><head><title>test</title></head><body><div>&nbsp;</div></body></html>";

        private SortedList<string, feed> _feeds = null;
        private SortedList<string, SortedList<string, feed_article>> _feeds_articles = null;
        private int _feed_index = -1;

        public MainWindow()
        {
            InitializeComponent();

            return;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _reader_tabs = new TabControl
            {
                Background = Brushes.Orange,
                BorderBrush = Brushes.Beige,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(1)
            };

            _reader_tabs.SelectionChanged += _reader_tabs_SelectionChanged;

            _feeds = feed_data_exchange.get_all_feeds();
            _feeds_articles = feed_data_exchange.get_all_feed_articles();

            var feed_names = _feeds.Keys;

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

                _reader_tab_items.Add(reader_tab);
                _reader_tabs.Items.Add(reader_tab);

                (reader_tab.Content as ListBox).SelectionChanged += Headline_SelectionChanged;
            }

            root_content.Children.Add(_reader_tabs);
            root_content.Children.Add(_reader_feed_detail);

            _reader_feed_detail.Children.Add(_reader_feed_name);
            _reader_feed_detail.Children.Add(_reader_headline);
            _reader_feed_detail.Children.Add(_reader_article);

            return;
        }


        private bool feed_index_valid
        {
            get
            {
                return _feed_index > -1 && _feed_index < _reader_tab_items.Count;
            }
        }

        private TabItem reader_tab
        {
            get
            {
                return feed_index_valid ? _reader_tab_items[_feed_index] : null;
            }
        }

        private ListBox feed_headlines
        {
            get
            {
                return reader_tab.Content as ListBox;
            }
        }

        private void Headline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            apply_article(feed_headlines.SelectedItem as feed_article);

            return;
        }

        private void apply_article(feed_article article)
        {
            _article = article;

            var article_text = article?.article_text ?? _empty_article;

            _reader_headline.Text = article?.headline_text ?? string.Empty;
            _reader_article.NavigateToString(article_text);

            return;
        }

        private void _reader_tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _feed_index = _reader_tabs.SelectedIndex;

            if (feed_index_valid)
            {
                var feed_name = _reader_feed_name.Text = $"{reader_tab.Header}";

                if (_feeds_articles.ContainsKey(feed_name))
                {
                    if (feed_headlines != null && feed_headlines.HasItems == false)
                    {
                        var indexed_feed_articles = _feeds_articles[feed_name];

                        feed_headlines.ItemsSource = indexed_feed_articles.Values;
                    }
                }
            }

            apply_article(feed_headlines.SelectedItem as feed_article);

            return;
        }
    }
}
