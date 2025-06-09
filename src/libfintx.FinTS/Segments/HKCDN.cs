﻿/*	
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
using static libfintx.FinTS.HKCDE;

namespace libfintx.FinTS
{
    public class HKCDN
    {
        /// <summary>
        /// Submit bankers order
        /// </summary>
        public static async Task<String> Init_HKCDN(FinTsClient client, string OrderId, string Receiver, string ReceiverIBAN, string ReceiverBIC, decimal Amount, string Usage, DateTime FirstTimeExecutionDay, TimeUnit timeUnit, string Rota, int ExecutionDay, DateTime? LastExecutionDay)
        {
            Log.Write("Starting job HKCDN: Modify bankers order");

            string segments = string.Empty;
            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            if (client.VopGvList.Contains("HKCDN"))
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

            var connectionDetails = client.ConnectionDetails;
            segments += "HKCDN:" + client.SegmentNumber + ":1+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03+@@";

            var sepaMessage = pain00100103.Create(connectionDetails.AccountHolder, connectionDetails.Iban, connectionDetails.Bic, Receiver, ReceiverIBAN, ReceiverBIC, Amount, Usage, new DateTime(1999, 1, 1)).Replace("'", "");
            segments += segments.Replace("@@", "@" + sepaMessage.Length + "@") + sepaMessage;

            segments += "++" + OrderId + "+" + FirstTimeExecutionDay.ToString("yyyyMMdd") + ":" + (char) timeUnit + ":" + Rota + ":" + ExecutionDay;
            if (LastExecutionDay != null)
                segments += ":" + LastExecutionDay.Value.ToString("yyyyMMdd");

            segments += "'";

            if (Helper.IsTANRequired("HKCDN"))
            {
                client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCDN");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var TAN = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, TAN);

            return TAN;
        }
    }
}
