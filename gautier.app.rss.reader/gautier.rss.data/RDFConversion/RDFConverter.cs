using System.Xml;
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
                RdfRDF? rdfRdf = new();

                var XMLReaderSetup = new XmlReaderSettings();
                XMLReaderSetup.IgnoreWhitespace = true;
                XMLReaderSetup.IgnoreComments = true;
                XMLReaderSetup.IgnoreProcessingInstructions = true;
                XMLReaderSetup.ValidationType = ValidationType.None;
                XMLReaderSetup.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None | System.Xml.Schema.XmlSchemaValidationFlags.AllowXmlAttributes;

                using (var xmlReader = XmlReader.Create(reader, XMLReaderSetup))
                {
                    rdfRdf = serializer.Deserialize(xmlReader) as RdfRDF;
                }

                if (rdfRdf != null)
                {
                    RDFDoc = rdfRdf;
                }
            }

            return RDFDoc;
        }
    }
}
