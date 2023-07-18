using System.IO;

using gautier.rss.data.RSSDb;

namespace gautier.app.rss.reader.ui.UIData
{
    internal static class FeedConfiguration
    {
        internal static string FeedSaveDirectoryPath => Directory.GetCurrentDirectory();

        internal static string FeedDbFilePath => @"rss.db";
        internal static string SQLiteDbConnectionString => SQLUtil.GetSQLiteConnectionString(FeedDbFilePath, 3);
    }
}
