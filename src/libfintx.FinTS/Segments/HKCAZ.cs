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
using libfintx.FinTS.Camt;
using libfintx.FinTS.Message;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class HKCAZ
    {
        /// <summary>
        /// Transactions in camt053 format
        /// </summary>
        public static async Task<String> Init_HKCAZ(FinTsClient client, string FromDate, string ToDate, string Startpoint, CamtVersion camtVers)
        {
            string segments = string.Empty;
            var connectionDetails = client.ConnectionDetails;
            AccountInformation activeAccount;
            if (client.activeAccount != null)
                activeAccount = client.activeAccount;
            else
                activeAccount = new AccountInformation()
                {
                    AccountNumber = connectionDetails.Account,
                    AccountBankCode = connectionDetails.Blz.ToString(),
                    SubAccountFeature = connectionDetails.SubAccount,
                    AccountIban = connectionDetails.Iban,
                    AccountBic = connectionDetails.Bic,
                };

            client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);

            switch (camtVers)
            {
                case CamtVersion.Camt052:
                    Log.Write("Starting job HKCAZ: Request transactions in camt052 format");

                    if (string.IsNullOrEmpty(FromDate))
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + client.HICAZS_Camt + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + client.HICAZS_Camt + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + client.HICAZS_Camt + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + client.HICAZS_Camt + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg3);
                    break;

                case CamtVersion.Camt053:
                    Log.Write("Starting job HKCAZ: Request transactions in camt053 format");

                    if (string.IsNullOrEmpty(FromDate))
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N++++" + Startpoint + "'";
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Startpoint))
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "'";
                        }
                        else
                        {
                            segments = "HKCAZ:" + client.SEGNUM + ":" + client.HICAZS + "+" + connectionDetails.Iban + ":" + connectionDetails.Bic + ":" + connectionDetails.Account + ":" + activeAccount.SubAccountFeature + ":280:" + connectionDetails.Blz + "+" + CamtScheme.Camt053 + "+N+" + FromDate + "+" + ToDate + "++" + Startpoint + "'";
                        }
                    }

                    break;

                default: // -> Will never happen, only for compiler
                    return string.Empty;
            }

            if (Helper.IsTANRequired("HKCAZ"))
            {
                client.SEGNUM = Convert.ToInt16(SEG_NUM.Seg4);
                segments = HKTAN.Init_HKTAN(client, segments, "HKCAZ");
            }

            string message = FinTSMessage.Create(client, client.HNHBS, client.HNHBK, segments, client.HIRMS);
            string response = await FinTSMessage.Send(client, message);

            Helper.Parse_Message(client, response);

            return response;
        }
    }
}
