/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2021 Abid Hussain
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Threading.Tasks;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public static class HKCDE
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static async Task<String> Init_HKCDE(FinTsClient client, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDE: Submit bankers order");
            var connectionDetails = client.ConnectionDetails;

            string sepaMessage = string.Empty;
            string segments = string.Empty;
            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            if (client.VopGvList.Contains("HKCDE"))
            {
                if (client.Vop)
                {
                    segments = HKVPP.Init_HKVPP(client, segments);
                    client.SegmentNumber++;
                }
                else
                {
                    segments = HKVPA.Init_HKVPA(client, segments);
                    client.SegmentNumber++;
                }
            }

            var account = Helper.CreateAccountInfo(client);

            segments += "HKCDE:" + client.SegmentNumber + ":1+" + account + "+" + client.SepaPainSchema + "+@@";
            if (client.SepaPainVersion == 1)
            {
                sepaMessage = client.LastSepaMessage ?? pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }
            else if (client.SepaPainVersion == 2)
            {
                sepaMessage = client.LastSepaMessage ?? pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }
            else if (client.SepaPainVersion == 3)
            {
                sepaMessage = client.LastSepaMessage ?? pain00100303.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }
            else if (client.SepaPainVersion == 9)
            {
                sepaMessage = client.LastSepaMessage ?? pain00100109.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1));
            }
            client.LastSepaMessage = sepaMessage;

            segments = segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char) timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDE"))
            {
                client.SegmentNumber++;
                segments = HKTAN.Init_HKTAN(client, segments, "HKCDE");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }

        public enum TimeUnit
        {
            Monthly = 'M',
            Weekly = 'W'
        }
    }
}
