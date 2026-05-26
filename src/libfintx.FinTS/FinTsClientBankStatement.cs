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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using libfintx.FinTS.Camt;
using libfintx.FinTS.Camt.Camt052;
using libfintx.FinTS.Camt.Camt053;
using libfintx.FinTS.Data;
using libfintx.FinTS.Data.Segment;
using libfintx.FinTS.Statement;
using libfintx.FinTS.Swift;

namespace libfintx.FinTS
{
    public partial class FinTsClient
    {
        /// <summary>
        /// Account transactions in SWIFT-format
        /// </summary>
        /// <param name="connectionDetails">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz, Account, IBAN, BIC</param>  
        /// <param name="anonymous"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>
        /// Transactions
        /// </returns>
        public async Task<HBCIDialogResult<List<BankStatement>>> BankStatements(TANDialog tanDialog)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result.TypedResult<List<BankStatement>>();

            result = await ProcessSCA(result, tanDialog);
            if (result.HasError)
                return result.TypedResult<List<BankStatement>>();

            // Success
            string BankCode = await Transaction.HKKAU(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result.TypedResult<List<BankStatement>>();

            result = await ProcessSCA(result, tanDialog);
            if (result.HasError)
                return result.TypedResult<List<BankStatement>>();

            BankCode = result.RawData;

            var bankStatements = new List<BankStatement>();

            var values = Helper.SplitEncryptedSegments(BankCode);

            var segments = new List<Segment>();
            foreach (var item in values)
            {
                var segment = Helper.Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            foreach (var segment in segments)
            {
                if (segment.Name == "HIKAU")
                {
                    var hikau = segment as HIKAU;
                    bankStatements.Add(new BankStatement()
                    {
                        StatementNumber = hikau.StatementNumber,
                        AcknowledgementCode = hikau.AcknowledgementCode,
                        PickupPossible = hikau.PickupPossible,
                        Year = hikau.Year,
                        CreationDate = hikau.CreationDate,
                        CreationTime = hikau.CreationTime,
                        CreationType = hikau.CreationType
                    });
                }
            }

            return result.TypedResult(bankStatements);
        }
    }
}
