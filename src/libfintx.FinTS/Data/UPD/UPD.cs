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
using System.Linq;
using System.Text.RegularExpressions;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static class UPD
    {
        public static string Value { get; set; }

        public static List<AccountInformation> AccountList { get; set; }

        public static void ParseUpd(string upd)
        {
            Value = upd;
            ParseAccounts(upd);
        }

        public static AccountInformation GetAccountInformations(string accountnumber, string bankcode)
        {
            return AccountList?.FirstOrDefault(a => a.AccountNumber == accountnumber && a.AccountBankCode == bankcode);
        }

        /// <summary>
        /// Parse accounts and store informations
        /// </summary>
        /// <param name="message"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private static bool ParseAccounts(string message)
        {
            AccountList = new List<AccountInformation>();
            try
            {
                List<string> segments = Helper.SplitSegments(message);

                foreach (var rawSegment in segments)
                {
                    var segment = Helper.Parse_Segment(rawSegment);
                    if (segment.Name != "HIUPD")
                    {
                        continue;
                    }

                    // HIUPD:165:6:4+0123456789::280:10050000+DE22100500000123456789+5985932562+10+EUR+Meier+Peter+Sparkassenbuch Gold

                    string Accountnumber = null;
                    string SubAccountFeature = null;
                    string Accountbankcode = null;
                    string Accountiban = null;
                    string Accountuserid = null;
                    string Accounttype = null;
                    string Accountcurrency = null;
                    string Accountowner = null;
                    List<AccountPermission> Accountpermissions = new List<AccountPermission>();

                    var accountInfo = segment.DataElements[0];
                    if (!accountInfo.IsDataElementGroup || accountInfo.DataElements.Count != 4)
                    {
                        throw new ArgumentException("Invalid HIUPD segment: missing/invalid account info.\n" + segment.Value);
                    }
                    Accountnumber = accountInfo.DataElements[0].Value;
                    SubAccountFeature = accountInfo.DataElements[1].Value;
                    Accountbankcode = accountInfo.DataElements[3].Value;

                    Accountiban = segment.DataElements[1].Value;
                    Accountuserid = segment.DataElements[2].Value;
                    Accounttype = segment.DataElements[3].Value;
                    Accountcurrency = segment.DataElements[4].Value;
                    Accountowner = segment.DataElements[5].Value;
                    Accounttype = segment.DataElements[7].Value;

                    if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                    {
                        // Account permissions
                        for (var i = 9; i < segment.DataElements.Count; i++)
                        {
                            var deg = segment.DataElements[i];
                            if (!deg.IsDataElementGroup)
                            {
                                break;
                            }
                            if (deg.DataElements.Count < 2)
                            {
                                throw new ArgumentException("Invalid account permission segment:\n" + deg.Value);
                            }

                            var permissionSegment = deg.DataElements[0].Value;
                            Accountpermissions.Add(new AccountPermission
                            {
                                Segment = permissionSegment,
                                Description = AccountPermission.Permission(permissionSegment)
                            });
                        }
                    }

                    if (Accountnumber?.Length > 2 || Accountiban?.Length > 2)
                    {
                        AccountList.Add(new AccountInformation()
                        {
                            AccountNumber = Accountnumber,
                            SubAccountFeature = SubAccountFeature,
                            AccountBankCode = Accountbankcode,
                            AccountIban = Accountiban,
                            AccountUserId = Accountuserid,
                            AccountType = Accounttype,
                            AccountCurrency = Accountcurrency,
                            AccountOwner = Accountowner,
                            AccountPermissions = Accountpermissions
                        });
                    };
                }

                if (AccountList.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
                return false;
            }
        }
    }
}
