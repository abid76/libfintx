using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    internal class HIKAUSegmentParser : ISegmentParser
    {
        private static bool? ParseBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value == "J" || value == "1")
                return true;

            if (value == "N" || value == "0")
                return false;

            return null;
        }

        private static int? ParseInteger(string value)
        {
            if (int.TryParse(value, out var result))
                return result;

            return null;
        }

        private static DateTime? ParseDate(string value)
        {
            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return null;
        }

        private static TimeSpan? ParseTime(string value)
        {
            if (DateTime.TryParseExact(value, new[] { "HHmmss", "HHmm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed.TimeOfDay;

            return null;
        }

        public Segment ParseSegment(Segment segment)
        {
            var result = new HIKAU(segment);

            if (segment.DataElements.Count > 0 && segment.DataElements[0].Value?.Length > 0)
            {
                result.StatementNumber = int.Parse(segment.DataElements[0].Value);
            }

            if (segment.DataElements.Count > 1)
            {
                result.AcknowledgementCode = segment.DataElements[1].Value;
            }

            if (segment.DataElements.Count > 2)
            {
                result.PickupPossible = ParseBoolean(segment.DataElements[2].Value);
            }

            if (segment.DataElements.Count > 3)
            {
                result.Year = ParseInteger(segment.DataElements[3].Value);
            }

            if (segment.DataElements.Count > 4)
            {
                result.CreationDate = ParseDate(segment.DataElements[4].Value);
            }

            if (segment.DataElements.Count > 5)
            {
                result.CreationTime = ParseTime(segment.DataElements[5].Value);
            }

            if (segment.DataElements.Count > 6)
            {
                result.CreationType = segment.DataElements[6].Value;
            }

            return result;
        }
    }
}
