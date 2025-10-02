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
using System.Collections.Generic;
using System.Threading.Tasks;
using libfintx.Logger.Log;
using libfintx.Sepa;
using libfintx.FinTS.Message;

namespace libfintx.FinTS
{
    public static class HKCME
    {
        /// <summary>
        /// Collective transfer terminated
        /// </summary>
        public static async Task<String> Init_HKCME(FinTsClient client, List<Pain00100203CtData> PainData, string NumberofTransactions, decimal TotalAmount, DateTime ExecutionDay)
        {
            Log.Write("Starting job HKCME: Collective transfer money terminated");

            var TotalAmount_ = TotalAmount.ToString().Replace(",", ".");

            string segments = string.Empty;
            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            if (client.VopGvList.Contains("HKCME"))
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

            var connectionDetails = client.ConnectionDetails;
            segments += "HKCME:" + client.SegmentNumber + ":1+" + account + TotalAmount_ + ":EUR++" + " + urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.002.03+@@";

            var painMessage = pain00100203.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, PainData, NumberofTransactions, TotalAmount, ExecutionDay);

            segments += segments.Replace("@@", "@" + (painMessage.Length - 1) + "@") + painMessage;

            if (Helper.IsTANRequired("HKCME"))
            {
                client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCME");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
