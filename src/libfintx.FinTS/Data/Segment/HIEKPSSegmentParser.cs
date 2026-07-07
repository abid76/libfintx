using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.BPD;

namespace libfintx.FinTS.Data.Segment
{
    public class HIEKPSSegmentParser : SegmentParserBase
    {
        public override Segment ParseSegment(Segment segment)
        {
            var result = new HIEKPS(segment);
            if (segment.DataElements.Count > 3)
            {
                var paramDataElements = segment.DataElements[3];
                if (paramDataElements.DataElements.Count > 2)
                {
                    result.BankStatementNumberAllowed = ParseBoolean(paramDataElements.DataElements[0].Value) ?? false;
                    result.AcknowledgementNeeded = ParseBoolean(paramDataElements.DataElements[1].Value) ?? false;
                    result.CountEntriesAllowed = ParseBoolean(paramDataElements.DataElements[2].Value) ?? false;
                }
            }
            return result;
        }
    }
}
