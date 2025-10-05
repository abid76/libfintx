using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;
using Xunit;
using static libfintx.FinTS.Data.Segment.VopCheckResult;

namespace libfintx.Tests
{
    public class HIVPPSegmentParserTest
    {
        [Fact]
        public void Test_HIVPP()
        {
            var rawSegment = @"HIVPP:6:1:3+@32@0911-10-05-10.08.42.730944909325+++++DE03100701240149355000::::RVNA:VOP503-Aufruf konnte nicht durchgeführt werden+Ohne die Beachtung des Ergebnisses der Empfängerüberprüfung könnte der Betrag auf ein Konto gelangen, dessen Inhaber nicht dem eingegebenen Empfänger entspricht.";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HIVPPSegmentParser();
            var hivpp = (HIVPP)parser.ParseSegment(segment);
            Assert.Equal("HIVPP", hivpp.Name);
            Assert.Equal("0911-10-05-10.08.42.730944909325", hivpp.VopId);
            Assert.Empty(hivpp.VopIdValidUntil);
            Assert.Empty(hivpp.PollingId);
            Assert.Empty(hivpp.PaymentStatusReportDescriptor);
            Assert.Empty(hivpp.PaymentStatusReport);
            Assert.NotNull(hivpp.VopCheckResultSingleTransaction);
            Assert.Equal("DE03100701240149355000", hivpp.VopCheckResultSingleTransaction.Iban);
            Assert.Equal(VopCheckResultCode.RVNA, hivpp.VopCheckResultSingleTransaction.Result);
            Assert.True(hivpp.VopCheckResultSingleTransaction.IsNotApplicable);
            Assert.Equal("VOP503-Aufruf konnte nicht durchgeführt werden", hivpp.VopCheckResultSingleTransaction.ReasonRvna);
            Assert.Equal("Ohne die Beachtung des Ergebnisses der Empfängerüberprüfung könnte der Betrag auf ein Konto gelangen, dessen Inhaber nicht dem eingegebenen Empfänger entspricht.", hivpp.AdditionalInfo);
            Assert.Null(hivpp.WaitUntilNextPolling);
        }
    }
}
