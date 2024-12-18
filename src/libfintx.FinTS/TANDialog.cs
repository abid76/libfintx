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

#if WINDOWS
using System.Windows.Forms;
#endif

namespace libfintx.FinTS
{
    /// <summary>
    /// Helper object needed for entering a TAN.
    /// </summary>
    public class TANDialog
    {
        public HBCIDialogResult DialogResult { get; internal set; }

        public byte[] MatrixImage { get; internal set; }

        /// <summary>
        /// Bei Verwendung des Decoupled-Verfahren (HKTAN#7) setzen.
        /// </summary>
        public bool IsDecoupled { get; set; }

        /// <summary>
        /// HKTAN#7: Kann gesetzt werden, um die automatisierte Statusabfrage für eine Freigabe zu unterbrechen.
        /// </summary>
        public bool IsCancelWaitForApproval { get; set; }

        /// <summary>
        /// Der Aufrufer kann sich hier registrieren, um darüber benachrichtigt zu werden, dass die Statusabfrage erteilt wurde.
        /// </summary>
        private readonly Func<bool, Task> _onTransactionEndAsync;

        private readonly Func<TANDialog, Task<string>> _waitForTanAsync;

        /// <summary>
        /// Enter a TAN without any visual components, e.g. pushTAN or mobileTAN.
        /// </summary>
        /// <param name="waitForTanAsync">Function which takes a </param>
        /// <param name="dialogResult"></param>
        /// <param name="matrixImage"></param>
        public TANDialog(Func<TANDialog, Task<string>> waitForTanAsync)
        {
            _waitForTanAsync = waitForTanAsync;
        }

        /// <summary>
        /// Enter a TAN without any visual components, e.g. pushTAN or mobileTAN.
        /// </summary>
        /// <param name="waitForTanAsync">Function which takes a </param>
        /// <param name="dialogResult"></param>
        /// <param name="matrixImage"></param>
        public TANDialog(Func<TANDialog, Task<string>> waitForTanAsync, Func<bool, Task> onTransactionEndAsync)
        {
            _waitForTanAsync = waitForTanAsync;
            _onTransactionEndAsync = onTransactionEndAsync;
        }

        /// <summary>
        /// Wait for the user to enter a TAN.
        /// </summary>
        /// <param name="dialogResult">The <code>HBCIDialogResult</code> from the bank which requests the TAN. Can be used to display bank messages in the dialog.</param>
        /// <returns></returns>
        internal async Task<string> WaitForTanAsync()
        {
            return await _waitForTanAsync.Invoke(this);
        }

        /// <summary>
        /// Wait for the user to enter a TAN.
        /// </summary>
        /// <param name="dialogResult">The <code>HBCIDialogResult</code> from the bank which requests the TAN. Can be used to display bank messages in the dialog.</param>
        /// <returns></returns>
        internal async Task OnTransactionEndAsync(bool success)
        {
            await (_onTransactionEndAsync != null ? _onTransactionEndAsync.Invoke(success) : Task.CompletedTask);
        }
    }
}
