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
using System.Threading.Tasks;
using libfintx.FinTS.Camt;
using libfintx.FinTS.Data;
using libfintx.FinTS.Swift;
using libfintx.FinTS.Vop;
using libfintx.Sepa;

namespace libfintx.FinTS
{
    public interface IFinTsClient
    {
        AccountInformation activeAccount { get; set; }
        bool Anonymous { get; }
        ConnectionDetails ConnectionDetails { get; }
        int? TanProcessCode { get; set; }
        string TanMedium { get; set; }
        int HktanVersion { get; set; }
        string SystemId { get; }

        Task<HBCIDialogResult<List<AccountInformation>>> Accounts(TANDialog tanDialog);
        Task<HBCIDialogResult<AccountBalance>> Balance(TANDialog tanDialog);
        Task<HBCIDialogResult> Collect(TANDialog tanDialog, string payerName, string payerIBAN, string payerBIC, decimal amount, string purpose, DateTime settlementDate, string mandateNumber, DateTime mandateDate, string creditorIdNumber);
        Task<HBCIDialogResult> CollectiveCollect(TANDialog tanDialog, DateTime settlementDate, List<Pain00800202CcData> painData, string numberOfTransactions, decimal totalAmount);
        Task<HBCIDialogResult> CollectiveTransfer(TANDialog tanDialog, VopDialog vopDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount);
        Task<HBCIDialogResult> CollectiveTransfer_Terminated(TANDialog tanDialog, VopDialog vopDialog, List<Pain00100203CtData> painData, string numberOfTransactions, decimal totalAmount, DateTime executionDay);
        Task<HBCIDialogResult> DeleteBankersOrder(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay);
        Task<HBCIDialogResult> DeleteTerminatedTransfer(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay);
        Task<HBCIDialogResult<List<BankersOrder>>> GetBankersOrders(TANDialog tanDialog);
        Task<HBCIDialogResult<List<TerminatedTransfer>>> GetTerminatedTransfers(TANDialog tanDialog);
        Task<HBCIDialogResult> ModifyBankersOrder(TANDialog tanDialog, VopDialog vopDialog, string OrderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay);
        Task<HBCIDialogResult> ModifyTerminatedTransfer(TANDialog tanDialog, VopDialog vopDialog, string orderId, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string usage, DateTime executionDay);
        Task<HBCIDialogResult> Prepaid(TANDialog tanDialog, int mobileServiceProvider, string phoneNumber, int amount);
        Task<HBCIDialogResult> Rebooking(TANDialog tanDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose);
        Task<HBCIDialogResult<List<string>>> RequestTANMediumName();
        Task<HBCIDialogResult> SubmitBankersOrder(TANDialog tanDialog, VopDialog vopDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, HKCDE.TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay);
        Task<HBCIDialogResult<string>> Synchronization();
        Task<HBCIDialogResult> TAN(string TAN);
        Task<HBCIDialogResult> TAN4(string TAN, string MediumName);
        Task<HBCIDialogResult<List<SwiftStatement>>> Transactions(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null, bool saveMt940File = false);
        Task<HBCIDialogResult<List<AccountTransaction>>> TransactionsSimple(TANDialog tanDialog, DateTime? startDate = null, DateTime? endDate = null);
        Task<HBCIDialogResult<List<CamtStatement>>> Transactions_camt(TANDialog tanDialog, CamtVersion camtVers, DateTime? startDate = null, DateTime? endDate = null, bool saveCamtFile = false);
        Task<HBCIDialogResult> Transfer(TANDialog tanDialog, VopDialog vopDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose);
        Task<HBCIDialogResult> Transfer_Terminated(TANDialog tanDialog, VopDialog vopDialog, string receiverName, string receiverIBAN, string receiverBIC, decimal amount, string purpose, DateTime executionDay);
    }
}
