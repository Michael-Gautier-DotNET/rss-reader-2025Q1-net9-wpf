using System.Xml.Linq;

namespace gautier.rss.data.FeedXml
{
    /*
     * Class design credit goes to Newsboat RSS API
     * https://newsboat.org/
     * https://github.com/newsboat/newsboat/
     * https://www.newsbeuter.org/devel.html
     * https://github.com/akrennmair/newsbeuter
     */
    public class XFeedParser
    {
        public const string XmlNamespaceContent = "http://purl.org/rss/1.0/modules/content/";
        public const string XmlNamespaceITunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        public const string XmlNamespaceDC = "http://purl.org/dc/elements/1.1/";
        public const string XmlNamespaceAtom0_3 = "http://purl.org/atom/ns#";
        public const string XmlNamespaceAtom1_0 = "http://www.w3.org/2005/Atom";
        public const string XmlNamespaceXml = "http://www.w3.org/XML/1998/namespace";

        private XDocument _FeedDocument = new();
        private XDocType _DocType = XDocType.Unknown;

        private bool _ProcessingHeader = false;
        private bool _ProcessingEntry = false;

        public XDocType DocType
        {
            get
            {
                return _DocType;
            }
        }

        public XFeed ParseFile(string filePath)
        {
            XFeed Feed = new();

            _FeedDocument = XDocument.Load(filePath);

            XElement? RootNode = _FeedDocument.Document?.Root;

            ParseNode(RootNode ?? new XElement(XName.Get(XmlNamespaceAtom1_0)), Feed);

            return Feed;
        }

        private void ParseNode(XElement node, XFeed feed)
        {
            XArticle Article = feed.Articles.Count > 0 ? feed.Articles.Last() : new();

            switch (node.Name.LocalName)
            {
                /*Feed/Atom Level*/
                case "feed":
                    {
                        if (_ProcessingEntry == false)
                        {
                            _ProcessingHeader = true;
                            _DocType = XDocType.ATOM_1_0;

                            DetectATOMVersion(node);
                        }
                    }
                    break;
                case "rss":
                    {
                        if (_ProcessingEntry == false)
                        {
                            _ProcessingHeader = true;

                            DetectRSSVersion(node);
                        }
                    }
                    break;
                case "RDF":
                    {
                        if (_ProcessingEntry == false)
                        {
                            _ProcessingHeader = true;

                            _DocType = XDocType.RDF;
                        }
                    }
                    break;
                case "channel":
                    {
                        if (_ProcessingEntry == false)
                        {
                            _ProcessingHeader = true;
                        }
                    }
                    break;
                case "title":
                    {
                        if (_ProcessingEntry)
                        {
                            Article.Title = node.Value;
                        }
                        else if (_ProcessingHeader)
                        {
                            feed.Title = node.Value;
                        }
                    }
                    break;
                case "link":
                case "id":
                    {
                        if (_ProcessingEntry)
                        {
                            string HRef = node.Value;

                            if (string.IsNullOrWhiteSpace(HRef))
                            {
                                HRef = GetHrefAttrValue(node);
                            }

                            Article.Link = HRef;
                        }
                        else if (_ProcessingHeader)
                        {
                            feed.Link = node.Value;
                        }
                    }
                    break;
                case "description":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.Description = node.Value;
                        }
                        else if (_ProcessingEntry)
                        {
                            Article.Description = node.Value;
                        }
                    }
                    break;
                case "lastBuildDate":
                case "pubDate":
                case "date":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.PublicationDate = node.Value;
                        }
                        else if (_ProcessingEntry)
                        {
                            Article.PublicationDate = node.Value;
                        }
                    }
                    break;
                case "language":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.Language = node.Value;
                        }
                    }
                    break;
                case "updatePeriod":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.UpdatePeriod = node.Value;
                        }
                    }
                    break;
                case "updateFrequency":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.UpdateFrequency = node.Value;
                        }
                    }
                    break;
                case "generator":
                    {
                        if (_ProcessingHeader)
                        {
                            feed.Generator = node.Value;
                        }
                    }
                    break;
                case "item":
                case "entry":
                    {
                        if (_ProcessingHeader)
                        {
                            _ProcessingHeader = false;
                            _ProcessingEntry = true;
                        }

                        Article = new();

                        feed.Articles.Add(Article);
                    }
                    break;
                case "creator":
                    {
                        if (_ProcessingEntry)
                        {
                            Article.Creator = node.Value;
                        }
                    }
                    break;
                case "guid":
                    {
                        if (_ProcessingEntry)
                        {
                            Article.Guid = node.Value;

                            if (string.IsNullOrWhiteSpace(Article.Link))
                            {
                                Article.Link = node.Value;
                            }

                            foreach (XAttribute Attr in node.Attributes())
                            {
                                switch (Attr.Name.LocalName)
                                {
                                    case "isPermaLink":
                                        {
                                            Article.GuidIsPermaLink = (node.Value == "true");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "encoded":
                case "summary":
                    {
                        if (_ProcessingEntry)
                        {
                            Article.ContentEncoded = node.Value;
                        }
                    }
                    break;
            }

            foreach (XElement NextElement in node.Elements())
            {
                ParseNode(NextElement, feed);
            }

            return;
        }

        private static string GetHrefAttrValue(XElement node)
        {
            string HRef = string.Empty;

            foreach (XAttribute Attr in node.Attributes())
            {
                switch (Attr.Name.LocalName)
                {
                    case "href":
                        {
                            HRef = Attr.Value;
                        }
                        break;
                }

                if (string.IsNullOrWhiteSpace(HRef) == false)
                {
                    break;
                }
            }

            return HRef;
        }

        private void DetectRSSVersion(XElement node)
        {
            foreach (XAttribute Attr in node.Attributes())
            {
                switch (Attr.Name.LocalName)
                {
                    case "version":
                        {
                            switch (Attr.Value)
                            {
                                case "0.91":
                                    {
                                        _DocType = XDocType.RSS_0_91;
                                    }
                                    break;
                                case "0.92":
                                    {
                                        _DocType = XDocType.RSS_0_92;
                                    }
                                    break;
                                case "1.0":
                                    {
                                        _DocType = XDocType.RSS_1_0;
                                    }
                                    break;
                                case "2.0":
                                    {
                                        _DocType = XDocType.RSS_2_0;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            return;
        }

        private void DetectATOMVersion(XElement node)
        {
            XNamespace NameSpaceValue = node.GetDefaultNamespace();

            switch (NameSpaceValue.NamespaceName)
            {
                case XmlNamespaceAtom0_3:
                    {
                        _DocType = XDocType.ATOM_0_3_NONS;
                    }
                    break;
                case XmlNamespaceAtom1_0:
                    {
                        _DocType = XDocType.ATOM_1_0;
                    }
                    break;
                default:
                    {
                        CheckAtomAttributes(node);
                    }
                    break;
            }

            return;
        }

        private void CheckAtomAttributes(XElement node)
        {
            foreach (XAttribute Attr in node.Attributes())
            {
                switch (Attr.Name.LocalName)
                {
                    case "version":
                        {
                            switch (Attr.Value)
                            {
                                case "0.3":
                                    {
                                        _DocType = XDocType.ATOM_0_3;
                                    }
                                    break;
                                case "1.0":
                                    {
                                        _DocType = XDocType.ATOM_1_0;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            return;
        }
    }
}
