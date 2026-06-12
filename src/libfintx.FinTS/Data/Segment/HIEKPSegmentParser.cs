using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIEKPSegmentParser : SegmentParserBase
    {
        public override Segment ParseSegment(Segment segment)
        {
            var result = new HIEKP(segment);
            if (segment.DataElements.Count > 0)
            {
                result.Statements = ParseBytesFromBase64(segment.DataElements[0].Value);
            }
            if (segment.DataElements.Count > 1)
            {
                result.AcknowledgementCode = segment.DataElements[1].Value;
            }

            return result;
        }
    }
}
