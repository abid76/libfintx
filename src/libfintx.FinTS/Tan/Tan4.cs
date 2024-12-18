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
    public static class Tan4
    {
        /// <summary>
        /// TAN process 4
        /// </summary>
        public static async Task<String> Send_TAN4(FinTsClient client, string TAN, string MediumName)
        {
            Log.Write("Starting job TAN process 4");

            string segments = string.Empty;

            // Version 3
            if (client.HktanVersion == 3)
                segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HktanVersion + "+4+++++++" + MediumName + "'";
            // Version 4
            else if (client.HktanVersion == 4)
                segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HktanVersion + "+4++++++++" + MediumName + "'";
            // Version 5
            else if (client.HktanVersion == 5)
                segments = "HKTAN:" + SEG_NUM.Seg3 + ":" + client.HktanVersion + "+4++++++++++" + MediumName + "'";

            client.SegmentNumber = Convert.ToInt16(SEG_NUM.Seg3);

            return await FinTSMessage.Send(client, FinTSMessage.Create(client, client.MessageNumber, client.DialogId, segments, client.TanProcessCode, TAN));
        }
    }
}
