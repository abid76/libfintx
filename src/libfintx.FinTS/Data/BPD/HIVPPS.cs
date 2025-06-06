using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS
{
    internal class HIVPPS : SegmentBase
    {
        public int MaxCountOrders { get; set; }
        public int MaxCountSignatures { get; set; }
        public string SecCode { get; set; }
        public HivppsParam HivppsParam { get; set; }

        public HIVPPS(Segment segment) : base(segment)
        {
        }
    }

    internal class HivppsParam
    {
        public int MaxCountCTTIOptIn { get; set; }
        public bool DescriptionStructured { get; set; }
        public string TypePaymentStatusReport { get; set; }
        public bool BatchOrdersSupported { get; set; }
        public bool CountEntriesSupported { get; set; }
        public string SupportedPaymentStatusReportFormats { get; set; }
        public string VopOrderMandatory { get; set; }
    }
}
