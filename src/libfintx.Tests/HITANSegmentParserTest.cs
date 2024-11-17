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
    public class HITANSegmentParserTest
    {
        [Fact]
        public void Test_HITAN_1()
        {
            var rawData = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\PhotoTAN_message_1.txt"));
            List<string> rawSegments = Helper.SplitEncryptedSegments(rawData);
            List<Segment> segments = new List<Segment>();
            foreach (var item in rawSegments)
            {
                var segment = Helper.Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            var hitanSegment = segments.Where(s => s.Name == "HITAN").FirstOrDefault();

            Assert.NotNull(hitanSegment);
            Assert.Equal("HITAN", hitanSegment.Name);
            Assert.Equal(6, hitanSegment.DataElements.Count);
            Assert.Equal(3030, hitanSegment.DataElements[4].Length);
        }

        [Fact]
        public void Test_HITAN_3()
        {
            var rawData = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\PhotoTAN_message_3.txt"));
            List<string> rawSegments = Helper.SplitEncryptedSegments(rawData);
            List<Segment> segments = new List<Segment>();
            foreach (var item in rawSegments)
            {
                var segment = Helper.Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            var hitanSegment = segments.Where(s => s.Name == "HITAN").FirstOrDefault();

            Assert.NotNull(hitanSegment);
            Assert.Equal("HITAN", hitanSegment.Name);
            Assert.Equal(6, hitanSegment.DataElements.Count);
            Assert.Equal(3030, hitanSegment.DataElements[4].Length);
        }
    }
}
