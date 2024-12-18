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

using libfintx.FinTS.Data;
using libfintx.Logger.Log;
using libfintx.Sepa;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace libfintx.FinTS
{
    public partial class FinTsClient : IFinTsClient
    {
        public bool Anonymous { get; }
        public ConnectionDetails ConnectionDetails { get; }
        public AccountInformation activeAccount { get; set; }
        public string SystemId { get; internal set; }

        public string TanMedium { get; set; }
        public int? TanProcessCode { get; set; }
        internal List<int> AllowedTanProcesses { get; set; }

        public int HktanVersion { get; set; }

        internal int SegmentNumber { get; set; }

        internal string DialogId { get; set; }
        internal int MessageNumber { get; set; }
        internal int HksalVersion { get; set; }
        internal int HkkazVersion { get; set; }
        internal int HkcazVersion => 1;
        public string HkcazCamtScheme { get; set; }
        internal string HktanOrderRef { get; set; }
        internal int HispasVersion { get; set; }
        internal int SepaPainVersion { get; set; }
        internal bool SepaAccountNationalAllowed { get; set; }

        public FinTsClient(ConnectionDetails conn, bool anon = false)
        {
            ConnectionDetails = conn;
            Anonymous = anon;
            activeAccount = null;
        }

        internal async Task<HBCIDialogResult> InitializeConnection(string hkTanSegmentId = "HKIDN")
        {
            Log.Write("Initializing connection ...");

            HBCIDialogResult result;
            string BankCode;

            // Check if the user provided a SystemID
            if (ConnectionDetails.CustomerSystemId == null)
            {
                result = await Synchronization();
                if (!result.IsSuccess)
                {
                    Log.Write("Synchronisation failed.");
                    return result;
                }
            }
            else
            {
                SystemId = ConnectionDetails.CustomerSystemId;
            }
            BankCode = await Transaction.INI(this, hkTanSegmentId);

            var bankMessages = Helper.Parse_BankCode(BankCode);
            result = new HBCIDialogResult(bankMessages, BankCode);
            if (!result.IsSuccess)
                Log.Write("Initialisation failed: " + result);

            return result;
        }

        /// <summary>
        /// Synchronize bank connection
        /// </summary>
        /// <param name="conn">ConnectionDetails object must atleast contain the fields: Url, HBCIVersion, UserId, Pin, Blz</param>
        /// <returns>
        /// Customer System ID
        /// </returns>
        public async Task<HBCIDialogResult<string>> Synchronization()
        {
            string BankCode = await Transaction.HKSYN(this);

            var messages = Helper.Parse_BankCode(BankCode);

            return new HBCIDialogResult<string>(messages, BankCode, SystemId);
        }

        /// <summary>
        /// Retrieves the accounts for this client
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>Gets informations about the accounts</returns>
        public async Task<HBCIDialogResult<List<AccountInformation>>> Accounts(TANDialog tanDialog)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result.TypedResult<List<AccountInformation>>();

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result.TypedResult<List<AccountInformation>>();

            return new HBCIDialogResult<List<AccountInformation>>(result.Messages, UPD.Value, UPD.AccountList);
        }

        /// <summary>
        /// Account balance
        /// </summary>
        /// <param name="tanDialog">The TAN Dialog</param>
        /// <returns>The balance for this account</returns>
        public async Task<HBCIDialogResult<AccountBalance>> Balance(TANDialog tanDialog)
        {
            HBCIDialogResult result = await InitializeConnection();
            if (result.HasError)
                return result.TypedResult<AccountBalance>();

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            // Success
            string BankCode = await Transaction.HKSAL(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result.TypedResult<AccountBalance>();

            result = await ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<AccountBalance>();

            BankCode = result.RawData;
            AccountBalance balance = Helper.Parse_Balance(BankCode);
            return result.TypedResult(balance);
        }

        /// <summary>
        /// Rebook money from one to another account - General method
        /// </summary>
        /// <param name="receiverName">Name of the recipient</param>
        /// <param name="receiverIBAN">IBAN of the recipient</param>
        /// <param name="receiverBIC">BIC of the recipient</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="anonymous"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Rebooking(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC,
            decimal amount, string purpose)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            string BankCode = await Transaction.HKCUM(this, receiverName, receiverIBAN, receiverBIC, amount, purpose);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collect money from another account - General method
        /// </summary>
        /// <param name="payerName">Name of the payer</param>
        /// <param name="payerIBAN">IBAN of the payer</param>
        /// <param name="payerBIC">BIC of the payer</param>         
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>    
        /// <param name="settlementDate"></param>
        /// <param name="mandateNumber"></param>
        /// <param name="mandateDate"></param>
        /// <param name="creditorIdNumber"></param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Collect(TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC,
            decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            string BankCode = await Transaction.HKDSE(this, payerName, payerIBAN, payerBIC, amount, purpose, settlementDate, mandateNumber, mandateDate, creditorIdNumber);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Collective collect money from other accounts - General method
        /// </summary>
        /// <param name="settlementDate"></param>
        /// <param name="painData"></param>
        /// <param name="numberOfTransactions"></param>
        /// <param name="totalAmount"></param>      
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> CollectiveCollect(TANDialog tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData,
           string numberOfTransactions, decimal totalAmount)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            string BankCode = await Transaction.HKDME(this, settlementDate, painData, numberOfTransactions, totalAmount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Load mobile phone prepaid card - General method
        /// </summary>
        /// <param name="mobileServiceProvider"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="amount">Amount to transfer</param>  
        /// <returns>
        /// Bank return codes
        /// </returns>
        public async Task<HBCIDialogResult> Prepaid(TANDialog tanDialog, int mobileServiceProvider, string phoneNumber,
            int amount)
        {
            var result = await InitializeConnection();
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog, true);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            string BankCode = await Transaction.HKPPD(this, mobileServiceProvider, phoneNumber, amount);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (result.HasError)
                return result;

            result = await ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Process required SCA (strong customer authentication).
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tanDialog"></param>
        /// <param name="ini">Wenn die SCA direkt nach der Dialog-Initialisierung erforderlich ist.</param>
        /// <returns></returns>
        private async Task<HBCIDialogResult> ProcessSCA(HBCIDialogResult result, TANDialog tanDialog, bool ini = false)
        {
            if (!result.IsSCARequired)
            {
                return result;
            }

            tanDialog.DialogResult = result;
            if (result.IsTanRequired)
            {
                string tan = await Helper.WaitForTanAsync(this, result, tanDialog);
                if (tan == null)
                {
                    // Wenn der User keine TAN eingegeben hat, können wir nichts tun
                }
                else
                {
                    result = await TAN(tan);
                }
            }
            else if (result.IsApprovalRequired)
            {
                // Ohne automatisierte Statusabfrage:
                // await Helper.WaitForTanAsync(this, result, tanDialog);
                // result = await TAN(null);


                // Mit automatisierter Statusabfrage
                await tanDialog.WaitForTanAsync(); // Dem Benutzer signalisieren, dass auf die Freigabe gewartet wird

                const int Delay = 2000; // Der minimale Zeitraum zwischen zwei Statusabfragen steht in HITANS, wir nehmen einfach 2 Sek
                await Task.Delay(Delay);
                result = await TAN(null);
                while (!result.IsSuccess && !result.HasError && result.IsWaitingForApproval) // Freigabe wurde noch nicht erteilt
                {
                    await Task.Delay(Delay);
                    if (tanDialog.IsCancelWaitForApproval)
                    {
                        // Nichts tun
                    }
                    else
                    {
                        result = await TAN(null);
                    }
                }

                await tanDialog.OnTransactionEndAsync(result.IsSuccess); // Dem Benutzer signalisieren, dass die Transaktion beendet ist
            }

            if (result.IsSuccess && ini)
            {
                // Fand die SCA direkt nach der Initialisierung statt, ist in der Antwort BPD/UPD enthalten
                Helper.Parse_Segments(this, result.RawData);
            }

            return result;
        }
    }
}
