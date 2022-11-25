using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS
{
    internal class HISPAS : SegmentBase
    {
        public bool IsAccountNationalAllowed { get; set; }

        public HISPAS(Segment segment) : base(segment)
        {
        }
    }
}
