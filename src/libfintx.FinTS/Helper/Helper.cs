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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using libfintx.FinTS.Camt;
using libfintx.FinTS.Data.Segment;
using libfintx.Globals;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static partial class Helper
    {
        public const string DefaultEncoding = "ISO-8859-1";

        /// <summary>
        /// Escapes all special Characters (':', '+', ''') with a question mark '?'.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EscapeHbciString(string str)
        {
            return str?.Replace(":", "?:").Replace("'", "?'").Replace("+", "?+");
        }

        /// <summary>
        /// Combine byte arrays
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

        /// <summary>
        /// Encode to Base64
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.GetEncoding(DefaultEncoding).GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Encode to Base64
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static byte[] EncodeTo64Bytes(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.GetEncoding(DefaultEncoding).GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return Encoding.GetEncoding(DefaultEncoding).GetBytes(returnValue);
        }

        /// <summary>
        /// Decode from Base64
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.GetEncoding(DefaultEncoding).GetString(encodedDataAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Decode from Base64 default
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        public static string DecodeFrom64EncodingDefault(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.GetEncoding(DefaultEncoding).GetString(encodedDataAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Encrypt -> HNVSD
        /// </summary>
        /// <param name="Segments"></param>
        /// <returns></returns>
        public static string Encrypt(string Segments)
        {
            return "HNVSD:999:1+@" + Segments.Length + "@" + Segments + "'";
        }

        /// <summary>
        /// Extract value from string
        /// </summary>
        /// <param name="StrSource"></param>
        /// <param name="StrStart"></param>
        /// <param name="StrEnd"></param>
        /// <returns></returns>
        public static string Parse_String(string StrSource, string StrStart, string StrEnd)
        {
            int Start, End;

            if (StrSource.Contains(StrStart) && StrSource.Contains(StrEnd))
            {
                Start = StrSource.IndexOf(StrStart, 0) + StrStart.Length;
                End = StrSource.IndexOf(StrEnd, Start);

                return StrSource.Substring(Start, End - Start);
            }
            else
            {
                return string.Empty;
            }
        }

        public static Segment Parse_Segment(string segmentCode)
        {
            Segment segment = null;
            try
            {
                segment = SegmentParserFactory.ParseSegment(segmentCode);
            }
            catch (Exception ex)
            {
                Log.Write($"Couldn't parse segment: {ex.Message}{Environment.NewLine}{segmentCode}");
            }
            return segment;
        }

        /// <summary>
        /// Parsing segment -> UPD, BPD
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="BLZ"></param>
        /// <param name="HBCIVersion"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static List<HBCIBankMessage> Parse_Segments(FinTsClient client, string Message)
        {
            Log.Write("Parsing segments ...");

            try
            {
                var connDetails = client.ConnectionDetails;
                List<HBCIBankMessage> result = new List<HBCIBankMessage>();

                List<string> rawSegments = SplitEncryptedSegments(Message);

                List<Segment> segments = new List<Segment>();
                foreach (var item in rawSegments)
                {
                    Segment segment = Parse_Segment(item);
                    if (segment != null)
                        segments.Add(segment);
                }

                // BPD
                string bpd = string.Empty;
                var bpaMatch = Regex.Match(Message, @"(HIBPA.+?)\b(HITAN|HNHBS|HISYN|HIUPA)\b");
                if (bpaMatch.Success)
                    bpd = bpaMatch.Groups[1].Value;
                if (bpd.Length > 0)
                {
                    if (bpd.EndsWith("''"))
                        bpd = bpd.Substring(0, bpd.Length - 1);

                    SaveBPD(connDetails.Blz, bpd);
                    BPD.ParseBpd(bpd);
                }

                // UPD
                string upd = string.Empty;
                var upaMatch = Regex.Match(Message, @"(HIUPA.+?)\b(HITAN|HNHBS|HIKIM)\b");
                if (upaMatch.Success)
                    upd = upaMatch.Groups[1].Value;
                if (upd.Length > 0)
                {
                    SaveUPD(connDetails.Blz, connDetails.UserId, upd);
                    UPD.ParseUpd(upd);
                }

                if (UPD.AccountList != null)
                {
                    //Add BIC to Account information (Not retrieved bz UPD??)
                    foreach (AccountInformation accInfo in UPD.AccountList)
                        accInfo.AccountBic = connDetails.Bic;
                }

                foreach (var segment in segments)
                {
                    if (segment.Name == "HIRMG")
                    {
                        // HIRMG:2:2+9050::Die Nachricht enthÃ¤lt Fehler.+9800::Dialog abgebrochen+9010::Initialisierung fehlgeschlagen, Auftrag nicht bearbeitet.
                        // HIRMG:2:2+9800::Dialogabbruch.

                        foreach (var message in segment.DataElements)
                        {
                            var bankMessage = Parse_BankCode_Message(message);
                            if (bankMessage != null)
                                result.Add(bankMessage);
                        }

                        //string[] HIRMG_messages = segment.Payload.Split('+');
                        //foreach (var HIRMG_message in HIRMG_messages)
                        //{
                        //    var message = Parse_BankCode_Message(HIRMG_message);
                        //    if (message != null)
                        //        result.Add(message);
                        //}
                    }

                    if (segment.Name == "HIRMS")
                    {
                        // HIRMS:3:2:2+9942::PIN falsch. Zugang gesperrt.'

                        foreach (var message in segment.DataElements)
                        {
                            var bankMessage = Parse_BankCode_Message(message);
                            if (bankMessage != null)
                                result.Add(bankMessage);
                        }

                        //string[] HIRMS_messages = segment.Payload.Split('+');
                        //foreach (var HIRMS_message in HIRMS_messages)
                        //{
                        //    var message = Parse_BankCode_Message(HIRMS_message);
                        //    if (message != null)
                        //        result.Add(message);
                        //}

                        var securityMessage = result.FirstOrDefault(m => m.Code == "3920");
                        if (securityMessage != null)
                        {
                            int? tanProcessCode = null;
                            var tanProcessList = new List<int>();

                            List<string> procedures = securityMessage.ParamList;

                            foreach (string value in procedures)
                            {
                                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int i))
                                {
                                    if (value.StartsWith("9") && i > 0)
                                    {
                                        if (tanProcessCode == null)
                                        {
                                            tanProcessCode = i;
                                        }
                                        tanProcessList.Add(i);
                                    }
                                }
                            }
                            if (client.TanProcessCode == null)
                            {
                                client.TanProcessCode = tanProcessCode;
                            }
                            else
                            {
                                if (!tanProcessList.Contains(client.TanProcessCode.GetValueOrDefault()))
                                    throw new Exception($"Invalid HIRMS/Tan-Mode {client.TanProcessCode} detected. Please choose one of the allowed modes: {string.Join(", ", tanProcessList)}");
                            }
                            client.AllowedTanProcesses = tanProcessList;
                        }
                    }

                    if (segment.Name == "HNHBK")
                    {
                        if (segment.DataElements.Count < 3)
                            throw new InvalidOperationException($"Expected segment '{segment}' to contain at least 3 data elements in payload.");

                        var dialogId = segment.DataElements[2].Value;
                        client.DialogId = dialogId;
                    }

                    if (segment.Name == "HISYN")
                    {
                        client.SystemId = segment.Payload;
                        Log.Write("Customer System ID: " + client.SystemId);
                    }

                    if (segment.Name == "HNHBS")
                    {
                        if (segment.Payload == null || segment.Payload == "0")
                            client.MessageNumber = 2;
                        else
                            client.MessageNumber = Convert.ToInt32(segment.Payload) + 1;
                    }

                    if (segment.Name == "HISALS")
                    {
                        if (client.HksalVersion < segment.Version)
                            client.HksalVersion = segment.Version;
                    }

                    if (segment.Name == "HITANS")
                    {
                        var hitans = (HITANS) segment;
                        if (client.TanProcessCode == null)
                        {
                            // Die höchste HKTAN-Version auswählen, welche in den erlaubten TAN-Verfahren (3920) enthalten ist.
                            if (hitans.TanProcesses.Select(tp => tp.TanCode).Intersect(client.AllowedTanProcesses).Any())
                                client.HktanVersion = segment.Version;
                        }
                        else
                        {
                            if (hitans.TanProcesses.Any(tp => tp.TanCode == Convert.ToInt32(client.TanProcessCode)))
                                client.HktanVersion = segment.Version;
                        }

                        if (TanProcesses.Items == null)
                            TanProcesses.Items = new List<TanProcess>();
                        // Nur solche TAN-Verfahren berücksichtigen, die den erlaubten TAN-Verfahren entsprechen
                        if (client.HktanVersion == 0 || client.HktanVersion == segment.Version)
                        {
                            TanProcesses.Items.AddRange(hitans.TanProcesses
                                .Where(ht => client.AllowedTanProcesses.Exists(t => t == Convert.ToInt16(ht.TanCode)))
                                .Select(ht => new TanProcess() { ProcessName = ht.Name, ProcessNumber = ht.TanCode.ToString() }));
                        }
                    }

                    if (segment.Name == "HITAN")
                    {
                        // HITAN:5:7:3+S++8578-06-23-13.22.43.709351
                        // HITAN:5:7:4+4++8578-06-23-13.22.43.709351+Bitte Auftrag in Ihrer App freigeben.
                        if (segment.DataElements.Count < 3)
                            throw new InvalidOperationException($"Invalid HITAN segment '{segment}'. Payload must have at least 3 data elements.");
                        client.HktanOrderRef = segment.DataElements[2].Value;
                    }

                    if (segment.Name == "HIKAZS")
                    {
                        if (client.HkkazVersion == 0)
                        {
                            client.HkkazVersion = segment.Version;
                        }
                        else
                        {
                            if (segment.Version > client.HkkazVersion)
                                client.HkkazVersion = segment.Version;
                        }
                    }

                    if (segment.Name == "HICAZS")
                    {
                        var scheme = CamtScheme.Camt052_001_02; // Fallback

                        if (segment.Payload.Contains("camt.052.001.02"))
                        {
                            scheme = CamtScheme.Camt052_001_02;
                        }
                        else if (segment.Payload.Contains("camt.052.001.08"))
                        {
                            scheme = CamtScheme.Camt052_001_08;
                        }

                        if (client.HkcazCamtScheme == null || !client.HkcazCamtScheme.Contains(scheme))
                        {
                            client.HkcazCamtScheme += (client.HkcazCamtScheme != null ? ":" : "") + scheme;
                        }
                    }

                    if (segment.Name == "HISPAS")
                    {
                        var hispas = segment as HISPAS;
                        if (client.HispasVersion < segment.Version)
                        {
                            client.HispasVersion = segment.Version;

                            if (hispas.Payload.Contains("pain.001.001.03"))
                                client.SepaPainVersion = 1;
                            else if (hispas.Payload.Contains("pain.001.002.03"))
                                client.SepaPainVersion = 2;
                            else if (hispas.Payload.Contains("pain.001.003.03"))
                                client.SepaPainVersion = 3;

                            if (client.SepaPainVersion == 0)
                                client.SepaPainVersion = 3; // -> Fallback. Most banks accept the newest pain version

                            client.SepaAccountNationalAllowed = hispas.IsAccountNationalAllowed;
                        }
                    }

                    if (segment.Name == "HIVPPS" || segment.Name == "HIVPAS" || segment.Name == "HIVOOS")
                    {
                        client.Vop = true;
                        if (segment.Name == "HIVPPS")
                        {
                            client.VopGvList = segment.DataElements.Select(d => d.Value).ToList();
                        }
                    }
                }

                // Fallback if HIKAZS is not delivered by BPD (eg. Postbank)
                if (client.HkkazVersion == 0)
                    client.HkkazVersion = 0;

                return result;
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());

                throw new InvalidOperationException($"Software error.", ex);
            }
        }

        /// <summary>
        /// Parsing bank message
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static List<Segment> Parse_Message(FinTsClient client, string Message)
        {
            List<string> values = SplitEncryptedSegments(Message);

            List<Segment> segments = new List<Segment>();
            foreach (var item in values)
            {
                Segment segment = Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            foreach (var segment in segments)
            {
                if (segment.Name == "HNHBS")
                {
                    if (segment.Payload == null || segment.Payload == "0")
                        client.MessageNumber = 2;
                    else
                        client.MessageNumber = Convert.ToInt32(segment.Payload) + 1;
                }

                if (segment.Name == "HITAN")
                {
                    // HITAN:5:7:3+S++8578-06-23-13.22.43.709351
                    // HITAN:5:7:4+4++8578-06-23-13.22.43.709351+Bitte Auftrag in Ihrer App freigeben.
                    // HITAN:5:6:4+4++76ma3j/MKH0BAABsRcJNhG?+owAQA+Eine neue TAN steht zur Abholung bereit.  Die TAN wurde reserviert am  16.11.2021 um 13?:54?:59 Uhr. Eine Push-Nachricht wurde versandt.  Bitte geben Sie die TAN ein.'
                    if (segment.DataElements.Count < 3)
                        throw new InvalidOperationException($"Invalid HITAN segment '{segment}'. Payload must have at least 3 data elements.");
                    client.HktanOrderRef = segment.DataElements[2].Value;
                }

                if (segment.Name == "HIRMS")
                {
                    client.VopNeeded = segment.DataElements.Select(d => Parse_BankCode_Message(d)).FirstOrDefault(m => m.Code == "3091") == null;
                }

                if (segment.Name == "HIVPP")
                {
                    HIVPP hivpp = segment as HIVPP;
                    client.VopPollingId = hivpp.PollingId;
                    client.VopId = hivpp.VopId;
                    client.VopStatusReport = hivpp.PaymentStatusReport;
                }
            }

            return segments;
        }

        /// <summary>
        /// Parse balance
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static AccountBalance Parse_Balance(string Message)
        {
            var hirms = Message.Substring(Message.IndexOf("HIRMS") + 5);
            hirms = hirms.Substring(0, (hirms.Contains("'") ? hirms.IndexOf('\'') : hirms.Length));
            var hirmsParts = hirms.Split(':');

            AccountBalance balance = new AccountBalance();
            balance.Message = hirmsParts[hirmsParts.Length - 1];

            if (Message.Contains("+0020::"))
            {
                var hisal = Message.Substring(Message.IndexOf("HISAL") + 5);
                hisal = hisal.Substring(0, (hisal.Contains("'") ? hisal.IndexOf('\'') : hisal.Length));
                var hisalParts = hisal.Split('+');

                balance.Successful = true;

                var hisalAccountParts = hisalParts[1].Split(':');
                if (hisalAccountParts.Length == 4)
                {
                    balance.AccountType = new AccountInformation()
                    {
                        AccountNumber = hisalAccountParts[0],
                        AccountBankCode = hisalAccountParts.Length > 3 ? hisalAccountParts[3] : null,
                        AccountType = hisalParts[2],
                        AccountCurrency = hisalParts[3],
                        AccountBic = !string.IsNullOrEmpty(hisalAccountParts[1]) ? hisalAccountParts[1] : null
                    };
                }
                else if (hisalAccountParts.Length == 2)
                {
                    balance.AccountType = new AccountInformation()
                    {
                        AccountIban = hisalAccountParts[0],
                        AccountBic = hisalAccountParts[1]
                    };
                }

                var hisalBalanceParts = hisalParts[4].Split(':');
                if (hisalBalanceParts[1].IndexOf("e-9", StringComparison.OrdinalIgnoreCase) >= 0)
                    balance.Balance = 0; // Deutsche Bank liefert manchmal "E-9", wenn der Kontostand 0 ist. Siehe Test_Parse_Balance und https://homebanking-hilfe.de/forum/topic.php?t=24155
                else
                    balance.Balance = Convert.ToDecimal($"{(hisalBalanceParts[0] == "D" ? "-" : "")}{hisalBalanceParts[1]}");


                //from here on optional fields / see page 46 in "FinTS_3.0_Messages_Geschaeftsvorfaelle_2015-08-07_final_version.pdf"
                if (hisalParts.Length > 5 && hisalParts[5].Contains(":"))
                {
                    var hisalMarkedBalanceParts = hisalParts[5].Split(':');
                    balance.MarkedTransactions = Convert.ToDecimal($"{(hisalMarkedBalanceParts[0] == "D" ? "-" : "")}{hisalMarkedBalanceParts[1]}");
                }

                if (hisalParts.Length > 6 && hisalParts[6].Contains(":"))
                {
                    balance.CreditLine = Convert.ToDecimal(hisalParts[6].Split(':')[0].TrimEnd(','));
                }

                if (hisalParts.Length > 7 && hisalParts[7].Contains(":"))
                {
                    balance.AvailableBalance = Convert.ToDecimal(hisalParts[7].Split(':')[0].TrimEnd(','));
                }

                /* ---------------------------------------------------------------------------------------------------------
                 * In addition to the above fields, the following fields from HISAL could also be implemented:
                 * 
                 * - 9/Bereits verfügter Betrag
                 * - 10/Überziehung
                 * - 11/Buchungszeitpunkt
                 * - 12/Fälligkeit 
                 * 
                 * Unfortunately I'm missing test samples. So I drop support unless we get test messages for this fields.
                 ------------------------------------------------------------------------------------------------------------ */
            }
            else
            {
                balance.Successful = false;

                string msg = string.Empty;
                for (int i = 1; i < hirmsParts.Length; i++)
                {
                    msg = msg + "??" + hirmsParts[i].Replace("::", ": ");
                }
                Log.Write(msg);
            }

            return balance;
        }

        internal static string Parse_Transactions_Startpoint(string bankCode)
        {
            return Regex.Match(bankCode, @"\+3040::[^:]+:(?<startpoint>[^'\+:]+)['\+:]").Groups["startpoint"].Value;
        }

        public static List<string> Parse_TANMedium(string BankCode)
        {
            // HITAB:4:4:3+0+G:1:::::::::::Abids iPhone 14+G:1:::::::::::iPhone 11
            // HITAB:5:4:3+0+A:1:::::::::::Handy::::::::+A:2:::::::::::iPhone Abid::::::::
            // HITAB:4:4:3+0+M:1:::::::::::mT?:MFN1:********0340'
            // HITAB:5:4:3+0+M:2:::::::::::Unregistriert 1::01514/654321::::::+M:1:::::::::::Handy:*********4321:::::::
            // HITAB:4:4:3+0+M:1:::::::::::mT?:MFN1:********0340+G:1:SO?:iPhone:00:::::::::SO?:iPhone''

            List<string> result = new List<string>();

            var segments = SplitEncryptedSegments(BankCode);
            foreach (var rawSegment in segments)
            {
                var segment = Parse_Segment(rawSegment);
                if (segment.Name == "HITAB")
                {
                    for (int i = 1; i < segment.DataElements.Count; i++)
                    {
                        var deg = segment.DataElements[i];
                        if (!deg.IsDataElementGroup)
                        {
                            continue;
                        }
                        string tanMediumName = null;
                        if (segment.Version == 4)
                        {
                            if (deg.DataElements.Count < 12)
                            {
                                continue;
                            }
                            tanMediumName = deg.DataElements[11].Value;
                        }
                        else if (deg.DataElements.Count >= 12)
                        {
                            tanMediumName = deg.DataElements[11].Value;
                        }
                        if (!string.IsNullOrEmpty(tanMediumName))
                        {
                            result.Add(tanMediumName);
                        }
                    }
                }
            }

            // For easier matching, replace '?:' by some special character
            //BankCode = BankCode.Replace("?:", @"\");

            //foreach (Match match in Regex.Matches(BankCode, @"\+[AGMS]:[012]:(?<Kartennummer>[^:]*):(?<Kartenfolgenummer>[^:]*):+(?<Bezeichnung>[^+:]+)"))
            //{
            //    result.Add(match.Groups["Bezeichnung"].Value.Replace(@"\", "?:"));
            //}

            return result;
        }

        /// <summary>
        /// Parse a single bank result message.
        /// </summary>
        /// <param name="BankCodeMessage"></param>
        /// <returns></returns>
        public static HBCIBankMessage Parse_BankCode_Message(DataElement singleMessage)
        {
            string code;
            string refElement = null;
            string messageText;
            string[] paramList = null;

            if (singleMessage.DataElements.Count >= 4)
            {
                code = singleMessage.DataElements[0].Value;
                refElement = singleMessage.DataElements[1].Value;
                messageText = singleMessage.DataElements[2].EscapedValue;
                paramList = singleMessage.DataElements.Skip(3).Select(d => d.EscapedValue).ToArray();
            }
            else if (singleMessage.DataElements.Count == 3)
            {
                code = singleMessage.DataElements[0].Value;
                refElement = singleMessage.DataElements[1].Value;
                messageText = singleMessage.DataElements[2].EscapedValue;
            }
            else
            {
                return null;
            }

            return new HBCIBankMessage(code, string.IsNullOrWhiteSpace(refElement) ? null : refElement, messageText, paramList);
        }

        /// <summary>
        /// Parse bank error codes
        /// </summary>
        /// <param name="BankCode"></param>
        /// <returns>Banks messages with "??" as seperator.</returns>
        public static List<HBCIBankMessage> Parse_BankCode(string BankCode)
        {
            List<HBCIBankMessage> result = new List<HBCIBankMessage>();

            var rawSegments = SplitEncryptedSegments(BankCode);
            List<Segment> segments = new List<Segment>();
            foreach (var item in rawSegments)
            {
                Segment segment = Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            foreach (var segment in segments)
            {
                if (segment.Name == "HIRMG" || segment.Name == "HIRMS")
                {
                    //// HIRMS:4:2:3+9210::*?'Ausführung bis?' muss nach ?'Ausführung ab?' liegen.+9210::*Die BIC wurde angepasst.+0900::Freigabe erfolgreich
                    foreach (var message in segment.DataElements)
                    {
                        var bankMessage = Parse_BankCode_Message(message);
                        if (bankMessage != null)
                            result.Add(bankMessage);
                    }
                }
            }

            return result;
        }

        public static HIVPP Parse_Vop(string BankCode)
        {
            var rawSegments = SplitEncryptedSegments(BankCode);
            List<Segment> segments = new List<Segment>();
            foreach (var item in rawSegments)
            {
                Segment segment = Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            var hivpp = segments.FirstOrDefault(s => s.Name == "HIVPP");
            if (hivpp != null)
            {
                var hivppSegment = hivpp as HIVPP;
                return hivppSegment;
            }

            return null;
        }

        /// <summary>
        /// Make filename valid
        /// </summary>
        public static string MakeFilenameValid(string value)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value.Replace(" ", "_");
        }

        private static string GetBPDDir()
        {
            var dir = FinTsGlobals.ProgramBaseDir;
            return Path.Combine(dir, "BPD");
        }

        private static string GetBPDFile(string dir, int BLZ)
        {
            return Path.Combine(dir, "280_" + BLZ + ".bpd");
        }

        private static string GetUPDDir()
        {
            var dir = FinTsGlobals.ProgramBaseDir;
            return Path.Combine(dir, "UPD");
        }

        private static string GetUPDFile(string dir, int BLZ, string UserID)
        {
            return Path.Combine(dir, "280_" + BLZ + "_" + MakeFilenameValid(UserID) + ".upd");
        }

        public static void SaveUPD(int BLZ, string UserID, string upd)
        {
            string dir = GetUPDDir();
            Directory.CreateDirectory(dir);
            var file = GetUPDFile(dir, BLZ, UserID);
            Log.Write($"Saving UPD to '{file}' ...");
            if (!File.Exists(file))
            {
                using (File.Create(file)) { }
                ;
            }
            File.WriteAllText(file, upd);
        }

        public static string GetUPD(int BLZ, string UserID)
        {
            var dir = GetUPDDir();
            var file = GetUPDFile(dir, BLZ, UserID);
            var content = File.Exists(file) ? File.ReadAllText(file) : string.Empty;

            return content;
        }

        public static void SaveBPD(int BLZ, string upd)
        {
            string dir = GetBPDDir();
            Directory.CreateDirectory(dir);
            var file = GetBPDFile(dir, BLZ);
            Log.Write($"Saving BPD to '{file}' ...");
            if (!File.Exists(file))
            {
                using (File.Create(file)) { }
                ;
            }
            File.WriteAllText(file, upd);
        }

        public static string GetBPD(int BLZ)
        {
            var dir = GetBPDDir();
            var file = GetBPDFile(dir, BLZ);
            var content = File.Exists(file) ? File.ReadAllText(file) : string.Empty;

            return content;
        }

        public static bool IsTANRequired(string gvName)
        {
            var HIPINS = BPD.HIPINS;
            return HIPINS != null && HIPINS.IsTanRequired(gvName);
        }

    }
}


