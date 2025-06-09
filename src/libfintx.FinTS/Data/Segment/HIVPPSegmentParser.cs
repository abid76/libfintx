using System;
using System.Collections.Generic;
using System.Text;
using static libfintx.FinTS.Data.Segment.VopCheckResult;

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
            result.PaymentStatusReportDescriptor = segment.DataElements[3].Value;
            result.PaymentStatusReport = segment.DataElements[4].Value;
            result.VopCheckResultSingleTransaction = ParseVopCheckResult(segment.DataElements[5]);
            result.AdditionalInfo = segment.DataElements[6].Value;
            result.WaitUntilNextPolling = segment.DataElements[7].Value != null ? Convert.ToInt32(segment.DataElements[7].Value) : (int?) null;

            return result;
        }

        private VopCheckResult ParseVopCheckResult(DataElement dataElement)
        {
            var result = (VopCheckResultCode?) null;
            switch (dataElement.DataElements[4].Value)
            {
                case "RCVC":
                    result = VopCheckResultCode.RCVC;
                    break;
                case "RVNM":
                    result = VopCheckResultCode.RVNM;
                    break;
                case "RVMC":
                    result = VopCheckResultCode.RVMC;
                    break;
                case "RVNA":
                    result = VopCheckResultCode.RVNA;
                    break;
                case "PDNG":
                    result = VopCheckResultCode.PDNG;
                    break;
            }

            VopCheckResult vopCheckResult = new VopCheckResult
            {
                Iban = dataElement.DataElements[0].Value,
                IbanAdditionalInfo = dataElement.DataElements[1].Value,
                DiffReceiverName = dataElement.DataElements[2].Value,
                OtherIdentification = dataElement.DataElements[3].Value,
                Result = result,
                ReasonRvna = dataElement.DataElements[5].Value
            };
            return vopCheckResult;
        }
    }

    public class VopCheckResult
    {
        public enum VopCheckResultCode
        {
            RCVC,
            RVNM,
            RVMC,
            RVNA,
            PDNG
        }

        public bool IsMatch => Result == VopCheckResultCode.RCVC;
        public bool IsCloseMatch => Result == VopCheckResultCode.RVMC;
        public bool IsNotApplicable => Result == VopCheckResultCode.RVNA;
        public bool IsNoMatch => Result == VopCheckResultCode.RVNM;
        public bool IsPending => Result == VopCheckResultCode.PDNG;

        public string Iban { get; set; }
        public string IbanAdditionalInfo { get; set; }
        public string DiffReceiverName { get; set; }
        public string OtherIdentification { get; set; }
        public VopCheckResultCode? Result { get; set; }
        public string ReasonRvna { get; set; }
    }
}
