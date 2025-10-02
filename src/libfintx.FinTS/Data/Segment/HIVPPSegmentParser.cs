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
            var result = VopCheckResult.FromValue(dataElement.DataElements[4].Value);
            result.Iban = dataElement.DataElements[0].Value;
            result.IbanAdditionalInfo = dataElement.DataElements[1].Value;
            result.DiffReceiverName = dataElement.DataElements[2].Value;
            result.OtherIdentification = dataElement.DataElements[3].Value;
            result.ReasonRvna = dataElement.DataElements[5].Value;

            return result;
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

        public static VopCheckResult FromValue(string value)
        {
            VopCheckResultCode? code = null;
            switch (value)
            {

                case "RCVC":
                    code = VopCheckResultCode.RCVC;
                    break;
                case "RVNM":
                    code = VopCheckResultCode.RVNM;
                    break;
                case "RVMC":
                    code = VopCheckResultCode.RVMC;
                    break;
                case "RVNA":
                    code = VopCheckResultCode.RVNA;
                    break;
                case "PDNG":
                    code = VopCheckResultCode.PDNG;
                    break;
                default:
                    break;
            }

            if (code != null)
                return new VopCheckResult { Result = code.Value };
            else
                return null;
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
