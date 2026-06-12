using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;

namespace libfintx.FinTS
{
    public static class HKQTG
    {
        public static async Task<String> Init_HKQTG(FinTsClient client, string acknowlegementCode)
        {
            string segments = string.Empty;
            var connectionDetails = client.ConnectionDetails;
            AccountInformation activeAccount;
            if (client.activeAccount != null)
            {
                activeAccount = client.activeAccount;
            }
            else
            {
                activeAccount = new AccountInformation()
                {
                    AccountNumber = connectionDetails.Account,
                    AccountBankCode = connectionDetails.Blz.ToString(),
                    SubAccountFeature = connectionDetails.SubAccount,
                    AccountIban = connectionDetails.Iban,
                    AccountBic = connectionDetails.Bic,
                };
            }
            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);
            segments += "HKQTG:" + client.SegmentNumber + ":1+@" + acknowlegementCode.Length + "@" + acknowlegementCode + "'";

            if (Helper.IsTANRequired("HKQTG"))
            {
                client.SegmentNumber++;
                segments = HKTAN.Init_HKTAN(client, segments, "HKQTG");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
