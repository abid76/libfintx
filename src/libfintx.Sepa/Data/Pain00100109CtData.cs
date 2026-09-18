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
using System.Xml.Serialization;

namespace libfintx.Sepa
{
    public class Pain00100109CtData : Pain001CtData
    {
        public static Pain00100109CtData Create(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(pain_001_001_09.Document), new XmlRootAttribute
            {
                ElementName = "Document",
                Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.09",
            });
            using (TextReader reader = new StringReader(xml))
            {
                return Create((pain_001_001_09.Document) ser.Deserialize(reader));
            }
        }
        public static Pain00100109CtData Create(pain_001_001_09.Document xml)
        {
            var result = new Pain00100109CtData();
            result.Initiator = xml.CstmrCdtTrfInitn?.GrpHdr?.InitgPty?.Nm;
            result.NumberOfTransactions = Convert.ToInt32(xml.CstmrCdtTrfInitn?.GrpHdr?.NbOfTxs);
            result.ControlSum = xml.CstmrCdtTrfInitn?.GrpHdr?.CtrlSum;
            foreach (var pmtInf in xml.CstmrCdtTrfInitn.PmtInf)
            {
                if (result.Payments == null)
                    result.Payments = new List<PaymentInfo>();
                var paymentInfo = new PaymentInfo();
                paymentInfo.RequestedExecutionDate = pmtInf.ReqdExctnDt.Item;
                paymentInfo.Debtor = pmtInf.Dbtr?.Nm;
                paymentInfo.DebtorAccount = pmtInf.DbtrAcct?.Id?.Item?.ToString();
                paymentInfo.DebtorAgent = pmtInf.DbtrAgt?.FinInstnId?.BICFI;
                result.Payments.Add(paymentInfo);
                foreach (var cdtTrfTxInf in pmtInf.CdtTrfTxInf)
                {
                    if (paymentInfo.CreditTxInfos == null)
                        paymentInfo.CreditTxInfos = new List<CreditTransferTransactionInfo>();
                    var creditTxInfo = new CreditTransferTransactionInfo();
                    creditTxInfo.Amount = ((pain_001_001_09.ActiveOrHistoricCurrencyAndAmount) cdtTrfTxInf.Amt?.Item).Value;
                    creditTxInfo.Creditor = cdtTrfTxInf.Cdtr?.Nm;
                    creditTxInfo.CreditorAccount = cdtTrfTxInf.CdtrAcct?.Id?.Item?.ToString();
                    creditTxInfo.CreditorAgent = cdtTrfTxInf.CdtrAgt?.FinInstnId?.BICFI;
                    if (cdtTrfTxInf.RmtInf?.Ustrd != null)
                        creditTxInfo.RemittanceInformation = string.Join(", ", cdtTrfTxInf.RmtInf?.Ustrd);
                    paymentInfo.CreditTxInfos.Add(creditTxInfo);
                }
            }
            return result;
        }
    }
}
