using System.Xml.Serialization;

namespace gautier.rss.data.RDFConversion
{
    public static class RDFConverter
    {
        public static RdfRDF CreateRDF(string rdfFilePath)
        {
            RdfRDF RDFDoc = new();

            var serializer = new XmlSerializer(typeof(RdfRDF));

            using (StringReader reader = new StringReader(File.ReadAllText(rdfFilePath)))
            {
                RdfRDF? rdfRdf = serializer.Deserialize(reader) as RdfRDF;

                if (rdfRdf != null)
                {
                    RDFDoc = rdfRdf;
                }
            }

            return RDFDoc;
        }
    }
}
