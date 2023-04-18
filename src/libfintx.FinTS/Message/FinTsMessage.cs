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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using libfintx.FinTS.Version;
using libfintx.Logger.Log;
using libfintx.Logger.Trace;

namespace libfintx.FinTS.Message
{
    public abstract class FinTSMessage
    {

        /// <summary>
        /// Create FinTS message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="Segments"></param>
        /// <param name="SegmentNum"></param>
        /// <returns></returns>

        public static string CreateSync(FinTsClient client, string Segments)
        {
            return Create(client, 1, "0", Segments, null, "0");
        }
        /// <summary>
        /// Create FinTS message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="MsgNum"></param>
        /// /// <param name="DialogID"></param>
        /// <param name="Segments"></param>
        /// /// <param name="HIRMS_TAN"></param>
        /// <param name="SegmentNum"></param>
        /// /// <param name="SystemID"></param>
        /// <returns></returns>
        ///
        /// (iwen65) First redesign to make things easier and more readable. All important params were values that had been stored as properties of the FinTsClient
        /// If this is connected as close as this it is better to pass the client as ref parameter, that makes the hole method much more flexible and extensible
        /// without breaking changes.
        /// ConnectionDetails are a part of the client too. We can handle the new added ServiceProtocolType inside the common connectioninfo without breaking changes too.
        /// Mostly it will be TLS12 but who knows.
        /// I'm pretty sure the method can be simplified even more. 
        /// 
        public static string Create(FinTsClient client, int MsgNum, string DialogID, string Segments, string HIRMS_TAN, string SystemID = null)
        {

            int Version = client.ConnectionDetails.HbciVersion;
            int BLZ = client.ConnectionDetails.BlzPrimary;
            string UserID = client.ConnectionDetails.UserIdEscaped;
            string PIN = client.ConnectionDetails.Pin;
            int SegmentNum = client.SEGNUM;

            if (SystemID == null)
                SystemID = client.SystemId; 

            if (MsgNum == 0)
                MsgNum = 1;

            DialogID += "";

            var HEAD_LEN = 29;
            var TRAIL_LEN = 11;

            Random Rnd = new Random();
            int RndNr = Rnd.Next();

            var encHead = string.Empty;
            var sigHead = string.Empty;
            var sigTrail = string.Empty;

            var secRef = Math.Round(Convert.ToDecimal(RndNr.ToString().Replace("-", "")) * 999999 + 1000000);

            string date = Convert.ToString(DateTime.Now.Year) + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd");
            string time = Convert.ToString(DateTime.Now.TimeOfDay).Substring(0, 8).Replace(":", "");

            string TAN_ = string.Empty;

            if (HIRMS_TAN != null)
            {
                if (HIRMS_TAN.Length >= 10)
                {
                    var split = HIRMS_TAN.Split(':');
                    if (split.Length == 2)
                    {
                        HIRMS_TAN = split[0];
                        TAN_ = ":" + split[1];
                    }
                }
            }

            if (Version == Convert.ToInt16(HBCI.v220))
            {
                encHead = "HNVSK:" + Enc.SECFUNC_ENC_PLAIN + ":2+" + Enc.SECFUNC_ENC_PLAIN + "+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@8@00000000:5:1+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":V:0:0+0'";

                Log.Write(encHead.Replace(UserID, "XXXXXX"));

                sigHead = string.Empty;

                if (HIRMS_TAN == null)
                {
                    sigHead = "HNSHK:2:3+" + Sig.SECFUNC_SIG_PT_2STEP_MIN + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:" + Sig.SIGMODE_RETAIL_MAC + ":1 +6:10:16+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX"));
                }

                else
                {
                    sigHead = "HNSHK:2:3+" + HIRMS_TAN + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:" + Sig.SIGMODE_RETAIL_MAC + ":1+6:10:16+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX"));
                }

                if (String.IsNullOrEmpty(TAN_))
                {
                    sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":1+" + secRef + "++" + PIN + "'";

                    Log.Write("HNSHA:" + Convert.ToString(SegmentNum + 1) + ":1+" + secRef + "++" + "XXXXXX" + "'");
                }

                else
                {
                    sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":1+" + secRef + "++" + PIN + TAN_ + "'";

                    Log.Write("HNSHA:" + Convert.ToString(SegmentNum + 1) + ":1+" + secRef + "++" + "XXXXXX" + "XXXXXX" + "'");
                }
            }
            else if (Version == Convert.ToInt16(HBCI.v300))
            {
                if (HIRMS_TAN == null)
                    encHead = "HNVSK:" + Enc.SECFUNC_ENC_PLAIN + ":3+PIN:1+" + Enc.SECFUNC_ENC_PLAIN + "+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@8@00000000:5:1+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":V:0:0+0'";
                else
                    encHead = "HNVSK:" + Enc.SECFUNC_ENC_PLAIN + ":3+PIN:2+" + Enc.SECFUNC_ENC_PLAIN + "+1+1::" + SystemID + "+1:" + date + ":" + time + "+2:2:13:@8@00000000:5:1+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":V:0:0+0'";

                Log.Write(encHead.Replace(UserID, "XXXXXX"));

                if (HIRMS_TAN == null)
                {
                    sigHead = "HNSHK:2:4+PIN:1+" + Sig.SECFUNC_SIG_PT_1STEP + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:" + Sig.SIGMODE_RETAIL_MAC + ":1+6:10:16+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX"));
                }
                else
                {
                    var SECFUNC = HIRMS_TAN.Equals("999") ? "1" : "2";

                    sigHead = "HNSHK:2:4+PIN:" + SECFUNC + "+" + HIRMS_TAN + "+" + secRef + "+1+1+1::" + SystemID + "+1+1:" + date + ":" + time + "+1:" + Sig.SIGMODE_RETAIL_MAC + ":1+6:10:16+" + SEG_Country.Germany + ":" + BLZ + ":" + UserID + ":S:0:0'";

                    Log.Write(sigHead.Replace(UserID, "XXXXXX"));
                }

                if (String.IsNullOrEmpty(TAN_))
                {
                    sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "++" + PIN + "'";

                    Log.Write("HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "++" + "XXXXXX" + "'");
                }

                else
                {
                    sigTrail = "HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "++" + PIN + TAN_ + "'";

                    Log.Write("HNSHA:" + Convert.ToString(SegmentNum + 1) + ":2+" + secRef + "++" + "XXXXXX" + "XXXXXX" + "'");
                }
            }
            else
            {
                Log.Write("HBCI version not supported");

                return string.Empty;
            }

            Segments = sigHead + Segments + sigTrail;

            var payload = Helper.Encrypt(Segments);

            if (HIRMS_TAN == null)
                Log.Write(payload.Replace(UserID, "XXXXXX").Replace(PIN, "XXXXXX"));
            else if (!String.IsNullOrEmpty(TAN_))
                Log.Write(payload.Replace(UserID, "XXXXXX").Replace(PIN, "XXXXXX").Replace(TAN_, "XXXXXX"));

            var msgLen = HEAD_LEN + TRAIL_LEN + ($"{MsgNum}".Length * 2) + DialogID.Length + payload.Length + encHead.Length;

            var paddedLen = ("000000000000").Substring(0, 12 - Convert.ToString(msgLen).Length) + Convert.ToString(msgLen);

            var msgHead = string.Empty;

            if (Version == Convert.ToInt16(HBCI.v220))
            {
                msgHead = "HNHBK:1:3+" + paddedLen + "+" + (HBCI.v220) + "+" + DialogID + "+" + MsgNum + "'";

                Log.Write(msgHead);
            }
            else if (Version == Convert.ToInt16(HBCI.v300))
            {
                msgHead = "HNHBK:1:3+" + paddedLen + "+" + (HBCI.v300) + "+" + DialogID + "+" + MsgNum + "'";

                Log.Write(msgHead);
            }
            else
            {
                Log.Write("HBCI version not supported");

                return string.Empty;
            }

            var msgEnd = "HNHBS:" + Convert.ToString(SegmentNum + 2) + ":1+" + MsgNum + "'";

            Log.Write(msgEnd);

            return msgHead + encHead + payload + msgEnd;
        }

        /// <summary>
        /// Send FinTS message
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static async Task<string> Send(FinTsClient client, string Message)
        {
            Log.Write("Connect to FinTS Server");
            Log.Write("Url: " + client.ConnectionDetails.Url);

            // Warning:
            // This writes plain message incl. PIN, UserID and TAN human readable into a textfile!
            if (Trace.Enabled)
            {
                if (Trace.MaskCredentials)
                    Trace.Write(Message, client.ConnectionDetails.UserIdEscaped, client.ConnectionDetails.Pin);
                else
                    Trace.Write(Message);
            }

            return await SendAsync(client, Message);
        }

        /// <summary>
        /// Send FinTS message async
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        private static async Task<string> SendAsync(FinTsClient client, string Message)
        {
            try
            {
                string FinTSMessage = string.Empty;
                ServicePointManager.SecurityProtocol = client.ConnectionDetails.SecurityProtocol;
                var req = WebRequest.Create(client.ConnectionDetails.Url) as HttpWebRequest;

                byte[] data = Encoding.ASCII.GetBytes(Helper.EncodeTo64(Message));

                req.Method = "POST";
                req.Timeout = 10000;
                req.ContentType = "application/octet-stream";
                req.ContentLength = data.Length;
                req.KeepAlive = false;

                using (var reqStream = await req.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                    await reqStream.FlushAsync();
                }

                using (var res = (HttpWebResponse) await req.GetResponseAsync())
                {
                    using (var resStream = res.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            FinTSMessage = Helper.DecodeFrom64EncodingDefault(streamReader.ReadToEnd());
                        }
                    }
                }

                // Warning:
                // This writes plain message incl. PIN, UserID and TAN human readable into a textfile!
                if (Trace.Enabled)
                {
                    if (Trace.MaskCredentials)
                        Trace.Write(FinTSMessage, client.ConnectionDetails.UserIdEscaped, client.ConnectionDetails.Pin);
                    else
                        Trace.Write(FinTSMessage);
                }

                return FinTSMessage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fehler beim Versenden der HBCI-Nachricht.", ex);
            }
        }
    }
}
