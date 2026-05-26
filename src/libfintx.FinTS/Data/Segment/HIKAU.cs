using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    public class HIKAU : SegmentBase
    {
        public enum AcknowledgementCodeEnum
        {
            NotNeeded = 0,
            Done = 1,
            Pending = 2
        }

        public int StatementNumber { get; set; }

        public AcknowledgementCodeEnum? AcknowledgementCode { get; set; }

        public bool? PickupPossible { get; set; }

        public int? Year { get; set; }

        public DateTime? CreationDate { get; set; }

        public TimeSpan? CreationTime { get; set; }

        public string CreationType { get; set; }

        public HIKAU(Segment segment) : base(segment)
        {
        }
    }
}
