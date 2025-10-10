using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS
{
    public static class HKVPA
    {
        public static string Init_HKVPA(FinTsClient client, string segments)
        {
            segments = segments + "HKVPA:" + client.SegmentNumber + ":1+@" + client.VopId.Length + "@" + client.VopId + "'"; 
            return segments;
        }
    }
}
