using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Message;

namespace libfintx.FinTS
{
    public static class HKEKP
    {
        public static async Task<String> Init_HKEKP(FinTsClient client, int? bankStatementNumber = null, int? bankStatementYear = null)
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
            segments += "HKEKP:" + client.SegmentNumber + ":" + client.HkekpVersion + "+" + Helper.CreateAccountInfo(client);
            if (bankStatementNumber.HasValue)
            {
                segments += "+" + bankStatementNumber.Value.ToString();
            }
            if (bankStatementYear.HasValue)
            {
                segments += "+" + bankStatementYear.Value.ToString();
            }
            segments += "'";

            if (Helper.IsTANRequired("HKEKP"))
            {
                client.SegmentNumber++;
                segments = HKTAN.Init_HKTAN(client, segments, "HKEKP");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
