using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS.Vop
{
    public class VopDialog
    {
        private Func<string, string, bool> _confirmVop;
        private Func<VopCheckResult, string, bool> _confirmVopWithResult;

        public VopDialog(Func<string, string, bool> confirmVop, Func<VopCheckResult, string, bool> confirmVopWithResult)
        {
            _confirmVop = confirmVop;
            _confirmVopWithResult = confirmVopWithResult;
        }

        public bool ConfirmVop(string vopText, string vopAdditionalInfo)
        {
            return _confirmVop(vopText, vopAdditionalInfo);
        }

        public bool ConfirmVop(VopCheckResult result, string vopAdditionalInfo)
        {
            return _confirmVopWithResult(result, vopAdditionalInfo);
        }
    }
}
