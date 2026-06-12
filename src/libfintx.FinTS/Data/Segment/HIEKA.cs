using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    public class HIEKA : SegmentBase
    {
        public int Format { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[] Statements { get; set; }

        public string AcknowledgementCode { get; set; }

        public DateTime CreationDate { get; internal set; }

        public int StatementsYear { get; internal set; }

        public int StatementsNumber { get; internal set; }

        public HIEKA(Segment segment) : base(segment)
        {
        }
    }
}
