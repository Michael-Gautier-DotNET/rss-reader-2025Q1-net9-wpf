using System.Windows;

namespace gautier.app.rss.reader.ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Style ApplyStyle(string styleName)
        {
            Style StyleElement = Current.TryFindResource(styleName) as Style;

            return StyleElement;
        }
    }
}
