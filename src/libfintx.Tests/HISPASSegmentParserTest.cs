using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using libfintx.FinTS;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class HISPASSegmentParserTest
    {
        [Fact]
        public void Test_HISPAS_1()
        {
            var rawData = @"HISPAS:24:1:4+1+1+1+J:J:N:sepade?:xsd?:pain.008.001.02.xsd:sepade?:xsd?:pain.001.001.03.xsd:sepade?:xsd?:pain.001.001.03_GBIC_2.xsd:sepade?:xsd?:pain.008.001.02_GBIC_2.xsd:sepade?:xsd?:pain.001.001.03_GBIC_3.xsd:sepade?:xsd?:pain.008.001.02_GBIC_3.xsd:sepade?:xsd?:pain.001.001.09_GBIC_4.xsd:sepade?:xsd?:pain.008.001.08_GBIC_4.xsd";
            var segment = new Segment(rawData);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HISPASSegmentParser();
            var hispas = (HISPAS) parser.ParseSegment(segment);
            Assert.NotNull(hispas);
            Assert.True(hispas.IsSingleAccountRetrievalAllowed);
            Assert.True(hispas.IsAccountNationalAllowed);
            Assert.False(hispas.IsStructuredTransferPurposeAllowed);
            Assert.Equal(8, hispas.SupportedPainSchemas.Count);
        }

        [Fact]
        public void Test_HISPAS_2()
        {
            var rawData = @"HISPAS:138:2:4+1+1+0+J:N:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.09:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.02:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.08";
            var segment = new Segment(rawData);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HISPASSegmentParser();
            var hispas = (HISPAS) parser.ParseSegment(segment);
            Assert.NotNull(hispas);
            Assert.True(hispas.IsSingleAccountRetrievalAllowed);
            Assert.False(hispas.IsAccountNationalAllowed);
            Assert.False(hispas.IsStructuredTransferPurposeAllowed);
            Assert.Equal(4, hispas.SupportedPainSchemas.Count);
        }
    }
}
