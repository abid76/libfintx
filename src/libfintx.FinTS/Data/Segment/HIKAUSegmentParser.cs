using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIKAUSegmentParser : SegmentParserBase
    {
        public override Segment ParseSegment(Segment segment)
        {
            var result = new HIKAU(segment);

            if (segment.DataElements.Count > 0 && segment.DataElements[0].Value?.Length > 0)
            {
                result.StatementNumber = ParseInteger(segment.DataElements[0].Value) ?? 0;
            }

            if (segment.DataElements.Count > 1)
            {
                result.AcknowledgementCode = segment.DataElements[1].Value;
            }

            if (segment.DataElements.Count > 2)
            {
                result.PickupPossible = ParseBoolean(segment.DataElements[2].Value);
            }

            if (segment.DataElements.Count > 3)
            {
                result.Year = ParseInteger(segment.DataElements[3].Value);
            }

            if (segment.DataElements.Count > 4)
            {
                result.CreationDate = ParseDate(segment.DataElements[4].Value);
            }

            if (segment.DataElements.Count > 5)
            {
                result.CreationTime = ParseTime(segment.DataElements[5].Value);
            }

            if (segment.DataElements.Count > 6)
            {
                result.CreationType = segment.DataElements[6].Value;
            }

            return result;
        }
    }
}
