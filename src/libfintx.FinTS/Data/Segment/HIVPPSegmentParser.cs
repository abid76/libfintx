using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIVPPSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HIVPP(segment);

            result.VopId = segment.DataElements[0].Value;
            result.VopIdValidUntil = segment.DataElements[1].Value;
            result.PollingId = segment.DataElements[2].Value;
            result.PaymentStatusReportDesc = segment.DataElements[3].Value;
            result.PaymentStatusReport = segment.DataElements[4].Value;
            result.VopCheckResult = ParseVopCheckResult(segment.DataElements[5]);
            result.AdditionalInfo = segment.DataElements[6].Value;
            result.WaitUntilNextPolling = segment.DataElements[7].Value != null ? Convert.ToInt32(segment.DataElements[7].Value) : (int?) null;

            return result;
        }

        private VopCheckResult ParseVopCheckResult(DataElement dataElement)
        {
            VopCheckResult vopCheckResult = new VopCheckResult
            {
                Iban = dataElement.DataElements[0].Value,
                IbanAdditionalInfo = dataElement.DataElements[1].Value,
                DiffReceiverName = dataElement.DataElements[2].Value,
                OtherIdentification = dataElement.DataElements[3].Value,
                VopCheckResultCode = dataElement.DataElements[4].Value,
                ReasonRvna = dataElement.DataElements[5].Value
            };
            return vopCheckResult;
        }
    }

    internal class VopCheckResult
    {
        public string Iban { get; set; }
        public string IbanAdditionalInfo { get; set; }
        public string DiffReceiverName { get; set; }
        public string OtherIdentification { get; set; }
        public string VopCheckResultCode { get; set; }
        public string ReasonRvna { get; set; }
    }
}
