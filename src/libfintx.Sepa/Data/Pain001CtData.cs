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

namespace libfintx.Sepa
{
    public class Pain001CtData
    {
        public string Initiator { get; set; }
        public int NumberOfTransactions { get; set; }
        public decimal? ControlSum { get; set; }
        public List<PaymentInfo> Payments { get; set; }

        public class PaymentInfo
        {
            public DateTime RequestedExecutionDate { get; set; }
            public string Debtor { get; set; }
            /// <summary>
            /// IBAN
            /// </summary>
            public string DebtorAccount { get; set; }
            /// <summary>
            /// BIC
            /// </summary>
            public string DebtorAgent { get; set; }
            public List<CreditTransferTransactionInfo> CreditTxInfos { get; set; }
        }

        public class CreditTransferTransactionInfo
        {
            public decimal Amount { get; set; }
            public string Creditor { get; set; }
            /// <summary>
            /// BIC
            /// </summary>
            public string CreditorAgent { get; set; }
            /// <summary>
            /// IBAN
            /// </summary>
            public string CreditorAccount { get; set; }
            /// <summary>
            /// Verwendungszweck
            /// </summary>
            public string RemittanceInformation { get; set; }
        }
    }
}
