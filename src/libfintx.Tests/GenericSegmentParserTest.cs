using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class GenericSegmentParserTest
    {
        [Fact]
        public void TestHikazs()
        {
            var segmentCode = "HIKAZS:23:6:4+20+1+1+90:N:N";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("HIKAZS", segment.Name);
            Assert.Equal(23, segment.Number);
            Assert.Equal(6, segment.Version);
            Assert.Equal(4, segment.Ref);
            Assert.Equal(4, segment.DataElements.Count);
            Assert.Equal("20", segment.DataElements[0].Value) ;
            Assert.Equal("1", segment.DataElements[1].Value);
            Assert.Equal("1", segment.DataElements[2].Value);
            Assert.True(segment.DataElements[3].IsDataElementGroup);
            Assert.Equal("90", segment.DataElements[3].DataElements[0].Value);
            Assert.Equal("N", segment.DataElements[3].DataElements[1].Value);
            Assert.Equal("N", segment.DataElements[3].DataElements[2].Value);
        }

        [Fact]
        public void TestHitan()
        {
            var segmentCode = "HITAN:5:7:3+S++eNmo9/2dEocBAACRO?+gjhW?+owAQA";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("HITAN", segment.Name);
            Assert.Equal(5, segment.Number);
            Assert.Equal(7, segment.Version);
            Assert.Equal(3, segment.Ref);
            Assert.Equal(3, segment.DataElements.Count);
            Assert.Equal("S", segment.DataElements[0].Value);
            Assert.Equal("", segment.DataElements[1].Value);
            Assert.Equal("eNmo9/2dEocBAACRO?+gjhW?+owAQA", segment.DataElements[2].Value);
        }

        [Fact]
        public void TestBinaryData_1()
        {
            var segmentCode = "ABCDE:1:2:3+@8@12345678";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("ABCDE", segment.Name);
            Assert.Equal(1, segment.Number);
            Assert.Equal(2, segment.Version);
            Assert.Equal(3, segment.Ref);

            Assert.Single(segment.DataElements);
            Assert.Equal("12345678", segment.DataElements[0].Value);
        }

        [Fact]
        public void TestBinaryData_2()
        {
            var segmentCode = "ABCDE:1:2:3+1+X:Y:@8@12345678";
            var segment = new Segment(segmentCode);
            segment = new GenericSegmentParser().ParseSegment(segment);

            Assert.Equal("ABCDE", segment.Name);
            Assert.Equal(1, segment.Number);
            Assert.Equal(2, segment.Version);
            Assert.Equal(3, segment.Ref);

            Assert.Equal(2, segment.DataElements.Count);
            Assert.Equal("1", segment.DataElements[0].Value);
            Assert.True(segment.DataElements[1].IsDataElementGroup);
            Assert.Equal(3, segment.DataElements[1].DataElements.Count);
            Assert.Equal("X", segment.DataElements[1].DataElements[0].Value);
            Assert.Equal("Y", segment.DataElements[1].DataElements[1].Value);
            Assert.Equal("12345678", segment.DataElements[1].DataElements[2].Value);
        }
    }
}
