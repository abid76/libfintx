using System;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class HIKAUSegmentParserTest
    {
        [Fact]
        public void Test_HIKAU_1()
        {
            var rawSegment = @"HIKAU:5:2:3+4+1+J+2026+20260430";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);

            var parser = new HIKAUSegmentParser();
            var hikau = (HIKAU) parser.ParseSegment(segment);

            Assert.Equal("HIKAU", segment.Name);
            Assert.Equal(5, segment.Number);
            Assert.Equal(2, segment.Version);
            Assert.Equal(4, hikau.StatementNumber);
            Assert.Equal("1", hikau.AcknowledgementCode);
            Assert.True(hikau.PickupPossible);
            Assert.Equal(2026, hikau.Year);
            Assert.Equal(new DateTime(2026, 4, 30), hikau.CreationDate);
            Assert.Null(hikau.CreationTime);
            Assert.Null(hikau.CreationType);
        }

        [Fact]
        public void Test_HIKAU_2()
        {
            var rawSegment = @"HIKAU:6:2:3+90001+1+N+2026+20260401";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);

            var parser = new HIKAUSegmentParser();
            var hikau = (HIKAU) parser.ParseSegment(segment);

            Assert.Equal(6, segment.Number);
            Assert.Equal(90001, hikau.StatementNumber);
            Assert.Equal("1", hikau.AcknowledgementCode);
            Assert.False(hikau.PickupPossible);
            Assert.Equal(2026, hikau.Year);
            Assert.Equal(new DateTime(2026, 4, 1), hikau.CreationDate);
            Assert.Null(hikau.CreationTime);
            Assert.Null(hikau.CreationType);
        }

        [Fact]
        public void Test_HIKAU_3()
        {
            var rawSegment = @"HIKAU:7:2:3+3+1+J+2026+20260401";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);

            var parser = new HIKAUSegmentParser();
            var hikau = (HIKAU) parser.ParseSegment(segment);

            Assert.Equal(7, segment.Number);
            Assert.Equal(3, hikau.StatementNumber);
            Assert.Equal("1", hikau.AcknowledgementCode);
            Assert.True(hikau.PickupPossible);
            Assert.Equal(2026, hikau.Year);
            Assert.Equal(new DateTime(2026, 4, 1), hikau.CreationDate);
            Assert.Null(hikau.CreationTime);
            Assert.Null(hikau.CreationType);
        }
    }
}
