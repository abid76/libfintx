using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libfintx.Sepa.pain_002_001_10
{
    public class Pain00200110
    {
        public static libfintx.Sepa.pain_002_001_10.Document Create(string xml)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Document));
            using (var reader = new StringReader(xml))
            {
                return (libfintx.Sepa.pain_002_001_10.Document)serializer.Deserialize(reader);
            }
        }   
    }
}
