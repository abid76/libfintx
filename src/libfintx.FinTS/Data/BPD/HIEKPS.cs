using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS.Data.BPD
{
    public class HIEKPS : SegmentBase
    {
        public HIEKPS(Segment.Segment segment) : base(segment)
        {
        }

        public bool BankStatementNumberAllowed { get; set; }

        public bool AcknowledgementNeeded { get; set; }

        public bool CountEntriesAllowed { get; set; }
    }
}
