using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.FinTS.Data.Segment
{
    internal class HISPASSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            // HISPAS:147:1:4+1+1+0+J:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.02

            var result = new HISPAS(segment);

            if (segment.DataElements.Count < 4)
            {
                throw new InvalidOperationException("HISPAS must contain at least 4 data elements.");
            }
            if (segment.DataElements[3].DataElements.Count < 4)
            {
                throw new InvalidOperationException("HISPAS Params SEPA must contain at least 4 data elements.");
            }

            result.IsSingleAccountRetrievalAllowed = segment.DataElements[3].DataElements[0].Value == "J";
            result.IsAccountNationalAllowed = segment.DataElements[3].DataElements[1].Value == "J";
            result.IsStructuredTransferPurposeAllowed = segment.DataElements[3].DataElements[2].Value == "J";

            for (int i = 3; i < segment.DataElements[3].DataElements.Count; i++)
            {
                result.SupportedPainSchemas.Add(segment.DataElements[3].DataElements[i].Value);
            }

            return result;
        }
    }
}
