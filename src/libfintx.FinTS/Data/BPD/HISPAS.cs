using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS
{
    internal class HISPAS : SegmentBase
    {
        public bool IsSingleAccountRetrievalAllowed { get; set; }

        public bool IsAccountNationalAllowed { get; set; }

        public bool IsStructuredTransferPurposeAllowed { get; set; }

        public List<string> SupportedPainSchemas { get; } = new List<string>();

        public HISPAS(Segment segment) : base(segment)
        {
        }
    }
}
