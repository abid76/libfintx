using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.BPD;

namespace libfintx.FinTS.Data.Segment
{
    public class HIEKPSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HIEKPS(segment);
            if (segment.DataElements.Count > 3)
            {
                var paramDataElements = segment.DataElements[3];
                if (paramDataElements.DataElements.Count > 3)
                {
                    result.BankStatementNumberAllowed = paramDataElements.DataElements[0].Value == "J";
                    result.AcknowledgementNeeded = paramDataElements.DataElements[1].Value == "J";
                    result.CountEntriesAllowed = paramDataElements.DataElements[2].Value == "J";
                }
            }
            return result;
        }
    }
}
