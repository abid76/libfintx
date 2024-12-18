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

namespace libfintx.FinTS
{
    public static class HKTAN
    {
        /// <summary>
        /// Set tan process
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static string Init_HKTAN(FinTsClient client, string segments, string segmentId)
        {
            if (String.IsNullOrEmpty(client.TanMedium)) // TAN Medium Name not set
            {
                // Erweiterung decoupled
                // Torsten: Gemäß meiner Auffassung sendet HTAN#7 das Segment deckungsgleich HKTAN#6
                if (client.HktanVersion >= 6)
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+" + segmentId + "'";
                else
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+'";
            }
            else // TAN Medium Name set
            {
                // Version 3, Process 4
                if (client.HktanVersion == 3)
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4++++++++" + client.TanMedium + "'";
                // Version 4, Process 4
                if (client.HktanVersion == 4)
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+++++++++" + client.TanMedium + "'";
                // Version 5, Process 4
                if (client.HktanVersion == 5)
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+++++++++++" + client.TanMedium + "'";
                // Version 6, Process 4
                if (client.HktanVersion == 6)
                {
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+" + segmentId + "+++++++++" + client.TanMedium + "'";
                }
                // Version 7, Process 4
                // Erweiterung decoupled
                // Torsten: Gemäß meiner Auffassung sendet HTAN#7 das Segment deckungsgleich HKTAN#6
                if (client.HktanVersion == 7)
                {
                    segments = segments + "HKTAN:" + client.SegmentNumber + ":" + client.HktanVersion + "+4+" + segmentId + "+++++++++" + client.TanMedium + "'";
                }
            }

            return segments;
        }
    }
}
