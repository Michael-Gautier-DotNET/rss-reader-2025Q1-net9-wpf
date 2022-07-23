using System;
using System.Windows;
using System.Windows.Controls;

namespace gaurtier.app.rss.reader.ui
{
    /// <summary>
    /// Root layout frame for RSS Reader
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _reader_button = null;

        public MainWindow()
        {
            InitializeComponent();

            return;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _reader_button = new Button
            {
                Content = "reader"
            };

            root_content.Children.Add(_reader_button);

            return;
        }
    }
}
