using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS
{
    public static class HKVPP
    {
        public static string Init_HKVPP(FinTsClient client, string segments)
        {
            segments = segments + "HKVPP:" + client.SegmentNumber + ":1+?+" + client.VopPollingId + "+" + client.VopRefPoint + "'"; 
            return segments;
        }
    }
}
