using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Perimeter.Extensions
{
   
    internal static class ObjectSerializer<T>
    {
        // Serialize to xml 
        public static string ToXml(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }
            return stringBuilder.ToString();
        }

        // Deserialize from xml 
        public static T FromXml(string xml)
        {
            T value=default(T);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StringReader stringReader = new StringReader(xml))
                {
                    if(!string.IsNullOrEmpty(xml))
                    {
                        object deserialized = serializer.Deserialize(stringReader);
                        value = (T)deserialized;
                    }
                }

            }
            catch (System.Exception ex)
            {

                
            }

            return value;
        }
    }
}
