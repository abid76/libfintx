using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS;
using libfintx.FinTS.Data.Segment;
using Xunit;
using static libfintx.FinTS.Data.Segment.VopCheckResult;

namespace libfintx.Tests
{
    public class HIVPPSSegmentParserTest
    {
        [Fact]
        public void Test_HIVPPS()
        {
            var rawSegment = @"HIVPPS:94:1:4+1+0+0+1000:N:V:J:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10:HKCUM:HKCUI:HKCCS:HKCDE:HKCDN:HKIPD:HKIDA:HKCSE:HKCSA:HKIPT:HKTIA:HKIPZ:HKCCM:HKCME:HKIPM:HKIPE:DKZDF";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HIVPPSSegmentParser();
            var hivpps = (HIVPPS)parser.ParseSegment(segment);
            Assert.Equal("HIVPPS", hivpps.Name);
            Assert.Equal(1, hivpps.MaxCountOrders);
            Assert.Equal(0, hivpps.MaxCountSignatures);
            Assert.Equal("0", hivpps.SecCode);
            Assert.NotNull(hivpps.ParamCheckOrder);
            Assert.False(hivpps.ParamCheckOrder.DescriptionStructured);
            Assert.Equal("V", hivpps.ParamCheckOrder.TypePaymentStatusReport);
            Assert.Equal("urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10", hivpps.ParamCheckOrder.SupportedPaymentStatusReportFormats);
            Assert.Equal(17, hivpps.ParamCheckOrder.VopOrderMandatory.Count);
        }
    }
}
