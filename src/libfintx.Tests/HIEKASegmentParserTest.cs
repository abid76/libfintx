using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;
using Xunit;
using static libfintx.FinTS.Data.Segment.VopCheckResult;

namespace libfintx.Tests
{
    public class HIEKASegmentParserTest
    {
        [Fact]
        public void Test_HIEKA_1()
        {
            var rawSegment = @"HIEKA:5:5:3+3+20260430:20260430++2026+4+@3@abc+++++++++@92@ÅÒÁ@@ððððððððððððððððððððò÷ôôôò÷ðñóððôaòðòö@@@@@@@@@@@ðððððððððñòðòö`ðõ`ðñ`ðóKòôKôñKõòõóùöðñ";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HIEKASegmentParser();
            var hieka = (HIEKA) parser.ParseSegment(segment);
            Assert.Equal("HIEKA", segment.Name);
            Assert.Equal(5, segment.Number);
            Assert.Equal(5, segment.Version);
            Assert.Equal(3, hieka.Format);
            Assert.Equal(new DateTime(2026, 4, 30), hieka.StartDate);
            Assert.Equal(new DateTime(2026, 4, 30), hieka.EndDate);
            Assert.Equal(2026, hieka.StatementsYear);
            Assert.Equal(4, hieka.StatementsNumber);
            Assert.Equal("abc", Encoding.GetEncoding("ISO-8859-1").GetString(hieka.Statements));
            Assert.Equal("ÅÒÁ@@ððððððððððððððððððððò÷ôôôò÷ðñóððôaòðòö@@@@@@@@@@@ðððððððððñòðòö`ðõ`ðñ`ðóKòôKôñKõòõóùöðñ", Encoding.GetEncoding("ISO-8859-1").GetString(hieka.AcknowledgementCode));
        }
    }
}
