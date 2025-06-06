using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIVPPSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HIVPPS(segment);

            result.MaxCountOrders = Convert.ToInt32(segment.DataElements[0].Value);
            result.MaxCountSignatures = Convert.ToInt32(segment.DataElements[1].Value);
            result.SecCode = segment.DataElements[2].Value;
            if (segment.DataElements.Count > 3 && segment.DataElements[3].DataElements != null && segment.DataElements[3].DataElements.Count > 0)
            {
                result.ParamCheckOrder = ParseHivppsParam(segment.DataElements[3]);
            }

            return result;
        }

        private ParamNameComparisonCheckOrder ParseHivppsParam(DataElement dataElement)
        {
            return new ParamNameComparisonCheckOrder
            {
                MaxCountCTTIOptIn = Convert.ToInt32(dataElement.DataElements[0].Value),
                DescriptionStructured = dataElement.DataElements[1].Value == "J",
                TypePaymentStatusReport = dataElement.DataElements[2].Value,
                BatchOrdersSupported = dataElement.DataElements[3].Value == "J",
                CountEntriesSupported = dataElement.DataElements[4].Value == "J",
                SupportedPaymentStatusReportFormats = dataElement.DataElements[5].Value,
                VopOrderMandatory = dataElement.DataElements[6].Value
            };
        }
    }
}
