using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    public class HIEKP : SegmentBase
    {
        public byte[] Statements { get; set; }

        public string AcknowledgementCode { get; set; }

        public HIEKP(Segment segment) : base(segment)
        {
        }
    }
}
