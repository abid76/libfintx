﻿/*	
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

namespace libfintx.FinTS
{
    public static class HKPPD
    {
        /// <summary>
        /// Load prepaid
        /// </summary>
        public static async Task<String> Init_HKPPD(FinTsClient client, int MobileServiceProvider, string PhoneNumber, int Amount)
        {
            Log.Write("Starting job HKPPD: Load prepaid");

            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            var connectionDetails = client.ConnectionDetails;
            string segments = "HKPPD:" + client.SegmentNumber + ":2+" + connectionDetails.Iban + ":" + connectionDetails.Bic + "+" + MobileServiceProvider + "+" + PhoneNumber + "+" + Amount + ",:EUR'";

            if (Helper.IsTANRequired("HKPPD"))
            {
                client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKPPD");
            }

            string message = FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode);
            var response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
