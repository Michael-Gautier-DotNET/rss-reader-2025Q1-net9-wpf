using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

using gautier.rss.data.FeedXml;
using gautier.rss.data.RDFConversionXD;

namespace gautier.rss.data
{
    /// <summary>
    /// Handles network communication. Translates rss XML data into C# object format.
    /// </summary>
    public static class RSSNetClient
    {
        public static void CreateRSSFeedFile(string feedUrl, string rssFeedFilePath)
        {
            using XmlReader feedXml = XmlReader.Create(feedUrl);

            using XmlWriter feedXmlWriter = XmlWriter.Create(rssFeedFilePath);

            feedXmlWriter.WriteNode(feedXml, false);

            return;
        }

        public static SyndicationFeed CreateRSSSyndicationFeed(string rssFeedFilePath)
        {
            SyndicationFeed RSSFeed = new();

            if (File.Exists(rssFeedFilePath) == true)
            {
                try
                {
                    using XmlReader RSSXmlFile = XmlReader.Create(rssFeedFilePath);

                    RSSFeed = SyndicationFeed.Load(RSSXmlFile);
                }
                catch (XmlException xmlE)
                {
                    bool ExceptionContainsRDF = xmlE.Message.Contains("'RDF'");
                    bool ExceptionContainsInvalidFormat = xmlE.Message.Contains("not an allowed feed format");

                    if (ExceptionContainsRDF && ExceptionContainsInvalidFormat)
                    {
                        RSSFeed = SyndicationConverter.ConvertToSyndicationFeed(rssFeedFilePath);
                    }
                }

            }

            return RSSFeed;
        }

        public static XFeed CreateRSSXFeed(string rssFeedFilePath)
        {
            XFeed RSSFeed = new();

            if (File.Exists(rssFeedFilePath) == true)
            {
                XFeedParser Parser = new();

                RSSFeed = Parser.ParseFile(rssFeedFilePath);
            }

            return RSSFeed;
        }

        public static bool ValidateUrlIsHttpOrHttps(string UrlValue)
        {
            bool IsValidUrl = false;

            if (string.IsNullOrEmpty(UrlValue) == false)
            {
                IsValidUrl = ValidateUrlIsHttpOrHttpsRegEx(UrlValue);

                /*
                 * Validate using Uri class.
                 */
                if (IsValidUrl == false)
                {
                    bool InitialCheck = Uri.IsWellFormedUriString(UrlValue, UriKind.Absolute);

                    if (InitialCheck)
                    {
                        IsValidUrl = ValidateUrlIsHttpOrHttpsURI(UrlValue);
                    }
                }

                if (IsValidUrl == false)
                {
                    IsValidUrl = ValidateUrlIsHttpOrHttpsText(UrlValue);
                }
            }

            return IsValidUrl;
        }

        public static bool ValidateUrlIsHttpOrHttpsText(string url)
        {
            bool IsValidUrl = false;

            /*Check protocol scheme*/
            bool InitialCheck = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            string Host = string.Empty;

            /*Protocol scheme is valid. Obtain Host.*/
            if (InitialCheck)
            {
                int DoubleSlashIndex = url.IndexOf("//", StringComparison.OrdinalIgnoreCase);

                int HostStartIndex = DoubleSlashIndex + 2;

                int HostEndIndex = url.IndexOf("/", HostStartIndex, StringComparison.OrdinalIgnoreCase);

                if (HostEndIndex == -1)
                {
                    HostEndIndex = url.Length;
                }

                Host = url[HostStartIndex..HostEndIndex];
            }

            /*Host is valid, check the remainder of the url.*/
            if (InitialCheck && string.IsNullOrEmpty(Host) == false)
            {
                List<char> ValidChars = new()
                        {
                            '.',//Dot
                            '-',//Dash
                            '_',//Underscore
                        };

                int ValidCharCount = 0;

                foreach (char Character in Host)
                {
                    if (char.IsLetterOrDigit(Character) || ValidChars.Contains(Character))
                    {
                        ValidCharCount++;
                    }
                }

                IsValidUrl = (ValidCharCount == url.Length);
            }

            return IsValidUrl;
        }

        public static bool ValidateUrlIsHttpOrHttpsRegEx(string url)
        {
            /*
             * Validate with Regular Expression.
             */
            string ValidationPattern = @"^(https?://)[^\s/$.?#].[^\s]*$";

            bool IsValidUrl = Regex.IsMatch(url, ValidationPattern);
            return IsValidUrl;
        }

        public static bool ValidateUrlIsHttpOrHttpsURI(string url)
        {
            bool IsUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? UriValue);

            bool IsHttpOrHttps = false;

            if (IsUri)
            {
                IsHttpOrHttps = UriValue?.Scheme == Uri.UriSchemeHttp || UriValue?.Scheme == Uri.UriSchemeHttps;
            }

            return IsHttpOrHttps;
        }

    }
}
