using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;

namespace libfintx.FinTS.Vop
{
    public class VopDialog
    {
        private Func<string, bool> _confirmVop;
        private Func<VopCheckResult, bool> _confirmVopWithResult;

        public VopDialog(Func<string, bool> confirmVop, Func<VopCheckResult, bool> confirmVopWithResult)
        {
            _confirmVop = confirmVop;
            _confirmVopWithResult = confirmVopWithResult;
        }

        public bool ConfirmVop(string vopText)
        {
            return _confirmVop(vopText);
        }

        public bool ConfirmVop(VopCheckResult result)
        {
            return _confirmVopWithResult(result);
        }
    }
}
