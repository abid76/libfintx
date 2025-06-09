using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Vop
{
    public class VopDialog
    {
        private Func<string, bool> _confirmVop;

        public VopDialog(Func<string, bool> confirmVop)
        {
            _confirmVop = confirmVop;
        }

        public bool ConfirmVop(string vopText)
        {
            return _confirmVop(vopText);
        }
    }
}
