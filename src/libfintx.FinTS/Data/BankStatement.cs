using System;
using System.Collections.Generic;
using System.Text;

namespace libfintx.FinTS.Data
{
    public class BankStatement
    {
        public int StatementNumber { get; internal set; }
        public string AcknowledgementCode { get; internal set; }
        public bool? PickupPossible { get; internal set; }
        public int? Year { get; internal set; }
        public DateTime? CreationDate { get; internal set; }
        public TimeSpan? CreationTime { get; internal set; }
        public string CreationType { get; internal set; }
    }
}
