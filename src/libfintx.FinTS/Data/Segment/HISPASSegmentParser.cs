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

            if (result.Version == 1)
            {
                if (segment.DataElements.Count < 4)
                {
                    throw new InvalidOperationException("HISPAS must contain at least 4 data elements.");
                }
                if (segment.DataElements[3].DataElements.Count < 4)
                {
                    throw new InvalidOperationException("HISPAS Params SEPA must contain at least 4 data elements.");
                }
            }
            else if (result.Version == 2)
            {
                if (segment.DataElements.Count < 4)
                {
                    throw new InvalidOperationException("HISPAS must contain at least 4 data elements.");
                }
                if (segment.DataElements[3].DataElements.Count < 5)
                {
                    throw new InvalidOperationException("HISPAS Params SEPA must contain at least 5 data elements.");
                }
            }

            var paramsSepaAccount = segment.DataElements[3];

            result.IsSingleAccountRetrievalAllowed = paramsSepaAccount.DataElements[0].Value == "J";
            result.IsAccountNationalAllowed = paramsSepaAccount.DataElements[1].Value == "J";
            result.IsStructuredTransferPurposeAllowed = paramsSepaAccount.DataElements[2].Value == "J";
            int startIdx = result.Version == 1 ? 3 : 4;
            for (int i = startIdx; i < paramsSepaAccount.DataElements.Count; i++)
            {
                result.SupportedPainSchemas.Add(paramsSepaAccount.DataElements[i].Value);
            }

            return result;
        }
    }
}
