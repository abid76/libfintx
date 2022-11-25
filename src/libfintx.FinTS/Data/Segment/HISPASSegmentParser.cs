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
            var result = new HISPAS(segment);

            // HISPAS:147:1:4+1+1+0+J:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.02
            var match = Regex.Match(segment.Payload, @"(?<2>\d{1,3})\+(?<3>\d)\+(?<4>\d)\+(?<5>.+)");
            if (!match.Success)
                throw new ArgumentException($"Could not parse segment '{segment.Name}':{Environment.NewLine}{segment.Payload}");

            var paramSepaAccount = match.Groups["5"].Value;
            match = Regex.Match(paramSepaAccount, @"(?<1>J|N):(?<2>J|N)");
            if (!match.Success)
                throw new ArgumentException($"Could not parse SEPA account info in segment '{segment.Name}':{Environment.NewLine}{paramSepaAccount}");

            result.IsAccountNationalAllowed = "J".Equals(match.Groups["2"].Value);

            return result;
        }
    }
}
