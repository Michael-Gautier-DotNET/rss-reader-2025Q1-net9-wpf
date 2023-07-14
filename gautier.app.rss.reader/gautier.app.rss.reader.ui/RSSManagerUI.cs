using System;
using System.Windows;

namespace gautier.app.rss.reader.ui
{
    public partial class RSSManagerUI : Window
    {
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

            return;
        }
    }
}
