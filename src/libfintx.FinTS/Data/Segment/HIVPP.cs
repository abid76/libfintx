using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    public class HIVPP : SegmentBase
    {
        public string VopId { get; set; }

        public string VopIdValidUntil { get; set; }

        public string PollingId { get; set; }

        public string PaymentStatusReportDesc { get; set; }

        public string PaymentStatusReport { get; set; }

        public VopCheckResult VopCheckResult { get; set; }

        public string AdditionalInfo { get; set; }

        public int? WaitUntilNextPolling { get; set; }

        public HIVPP(Segment segment) : base(segment)
        {
        }
    }
}
