using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Vop
{
    public class VopResult
    {
        public string PaymentStatusReport { get; set; }
        public string PaymentStatusReportDescriptor { get; set; }
        public bool IsVopConfirmed { get; set; }
    }
}
