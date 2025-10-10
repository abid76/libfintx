using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;

namespace libfintx.FinTS
{
    public static class HKVPP
    {
        public static string Init_HKVPP(FinTsClient client, string segments)
        {
            segments = segments + "HKVPP:" + client.SegmentNumber + ":1+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10'";
            return segments;
        }

        public static async Task<string> Init_HKVPP_Poll(FinTsClient client)
        {
            var segments = "HKVPP:" + client.SegmentNumber + ":1+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10";

            segments += "+@" + client.VopPollingId.Length + "@" + client.VopPollingId + "++" + client.VopRefPoint + "'";

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
