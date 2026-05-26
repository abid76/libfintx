using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIEKASegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HIEKA(segment);

            if (segment.DataElements.Count > 0)
            {
                result.Format = int.Parse(segment.DataElements[0].Value);
            }
            if (segment.DataElements.Count > 1 && segment.DataElements[1].DataElements.Count > 1)
            {
                result.StartDate = DateTime.ParseExact(segment.DataElements[1].DataElements[0].Value, "yyyyMMdd", null);
                result.EndDate = DateTime.ParseExact(segment.DataElements[1].DataElements[1].Value, "yyyyMMdd", null);
            }

            if (segment.Version == 1)
            {
                if (segment.DataElements.Count > 3)
                {
                    result.Statements = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[3].Value);
                }
                if (segment.DataElements.Count > 6)
                {
                    result.AcknowledgementCode = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[6].Value);
                }
            }
            else if ((segment.Version == 2 || segment.Version == 3 || segment.Version == 4))
            {
                if (segment.DataElements.Count > 3)
                {
                    result.Statements = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[3].Value);
                }
                if (segment.DataElements.Count > 12)
                {
                    result.AcknowledgementCode = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[12].Value);
                }
            }
            else if (segment.Version == 5)
            {
                if (segment.DataElements.Count > 2 && segment.DataElements[2].Value?.Length > 0)
                {
                    result.CreationDate = DateTime.ParseExact(segment.DataElements[2].Value, "yyyyMMdd", null);
                }
                if (segment.DataElements.Count > 3 && segment.DataElements[3].Value?.Length > 0)
                {
                    result.StatementsYear = int.Parse(segment.DataElements[3].Value);
                }
                if (segment.DataElements.Count > 4 && segment.DataElements[4].Value?.Length > 0)
                {
                    result.StatementsNumber = int.Parse(segment.DataElements[4].Value);
                }
                if (segment.DataElements.Count > 5 && segment.DataElements[5].Value?.Length > 0)
                {
                    result.Statements = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[5].Value);
                }
                if (segment.DataElements.Count > 14 && segment.DataElements[14].Value?.Length > 0)
                {
                    result.AcknowledgementCode = Encoding.GetEncoding("ISO-8859-1").GetBytes(segment.DataElements[14].Value);
                }
            }

            return result;
        }
    }
}
