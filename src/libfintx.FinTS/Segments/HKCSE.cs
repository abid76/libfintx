/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
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
    public static class HKCSE
    {
        /// <summary>
        /// Transfer terminated
        /// </summary>
        public static async Task<String> Init_HKCSE(FinTsClient client, string ReceiverName, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime ExecutionDay)
        {
            Log.Write("Starting job HKCSE: Transfer money terminated");
            var connectionDetails = client.ConnectionDetails;

            string sepaMessage = string.Empty;
            string segments = string.Empty;

            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            if (client.VopGvList.Contains("HKCSE"))
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

            if (client.SepaPainVersion == 1)
            {
                segments += "HKCSE:" + client.SegmentNumber + ":1+" + account + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";
                sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }
            else if (client.SepaPainVersion == 2)
            {
                segments += "HKCSE:" + client.SegmentNumber + ":1+" + account + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";
                sepaMessage = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }
            else if (client.SepaPainVersion == 3)
            {
                segments += "HKCSE:" + client.SegmentNumber + ":1+" + account + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03+@@";
                sepaMessage = pain00100303.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, ReceiverName, ReceiverIBAN, ReceiverBIC, Amount, Usage, ExecutionDay);
            }

            segments = segments.Replace("@@", "@" + (sepaMessage.Length - 1) + "@") + sepaMessage;

            if (Helper.IsTANRequired("HKCSE"))
            {
                client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCSE");
            }

            var message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
