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
        private TextBlock _reader_article = new TextBlock();
        private string _article_url = string.Empty;

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

            var feeds = feed_data_exchange.get_all_feeds();

            var feed_names = feeds.Keys;

            foreach (var feed_name in feed_names)
            {
                var reader_tab = new TabItem
                {
                    Header = feed_name
                };

                _reader_tab_items.Add(reader_tab);
                _reader_tabs.Items.Add(reader_tab);
            }

            root_content.Children.Add(_reader_tabs);
            root_content.Children.Add(_reader_feed_detail);

            _reader_feed_detail.Children.Add(_reader_feed_name);
            _reader_feed_detail.Children.Add(_reader_headline);
            _reader_feed_detail.Children.Add(_reader_article);

            return;
        }

        private void _reader_tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var feed_index = _reader_tabs.SelectedIndex;

            if(feed_index > -1 && feed_index < _reader_tab_items.Count)
            {
                var reader_tab_item = _reader_tab_items[feed_index];

                _reader_feed_name.Text = $"{reader_tab_item.Header}";
            }

            return;
        }


    }
}
