using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS.Data.BPD
{
    public class HIEKAS : SegmentBase
    {
        public HIEKAS(Segment.Segment segment) : base(segment)
        {
        }

        public bool BankStatementNumberAllowed { get; set; }

        public bool AcknowledgementNeeded { get; set; }

        public bool CountEntriesAllowed { get; set; }

        public List<string> SupportedFormats { get; set; } = new List<string>();

        public bool IsPdfFormatSupported => SupportedFormats.Contains("3");
    }
}
