using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;

namespace libfintx.FinTS
{
    public static class HKKAU
    {
        public static async Task<String> Init_HKKAU(FinTsClient client)
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
            segments += "HKKAU:" + client.SegmentNumber + ":" + client.HkkauVersion + "+" + Helper.CreateAccountInfo(client) + "'";

            if (Helper.IsTANRequired("HKKAU"))
            {
                client.SegmentNumber++;
                segments = HKTAN.Init_HKTAN(client, segments, "HKKAU");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
