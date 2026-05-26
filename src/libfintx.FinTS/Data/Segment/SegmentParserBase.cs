using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace libfintx.FinTS.Data.Segment
{
    public abstract class SegmentParserBase : ISegmentParser
    {
        public abstract Segment ParseSegment(Segment segment);
        protected static bool? ParseBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value == "J" || value == "1")
                return true;

            if (value == "N" || value == "0")
                return false;

            return null;
        }

        protected static int? ParseInteger(string value)
        {
            if (int.TryParse(value, out var result))
                return result;

            return null;
        }

        protected static DateTime? ParseDate(string value)
        {
            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return null;
        }

        protected static TimeSpan? ParseTime(string value)
        {
            if (DateTime.TryParseExact(value, new[] { "HHmmss", "HHmm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed.TimeOfDay;

            return null;
        }

        protected static byte[] ParseBytes(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
        }
    }
}
