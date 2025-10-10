using System;
using System.Collections.Generic;
using System.Text;
using libfintx.FinTS.Data.Segment;
using libfintx.Sepa.pain_002_001_10;
using Xunit;
using static libfintx.FinTS.Data.Segment.VopCheckResult;

namespace libfintx.Tests
{
    public class HIVPPSegmentParserTest
    {
        [Fact]
        public void Test_HIVPP_1()
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

        [Fact]
        public void Test_HIVPP_2()
        {
            var rawSegment = @"HIVPP:6:1:3+@32@5318-10-05-12.14.31.699628309325+++++DE03100701240149355000::::RCVC";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HIVPPSegmentParser();
            var hivpp = (HIVPP)parser.ParseSegment(segment);
            Assert.NotNull(hivpp);
            Assert.Equal("5318-10-05-12.14.31.699628309325", hivpp.VopId);
            Assert.NotNull(hivpp.VopCheckResultSingleTransaction);
            Assert.Equal("DE03100701240149355000", hivpp.VopCheckResultSingleTransaction.Iban);
            Assert.Equal(VopCheckResultCode.RCVC, hivpp.VopCheckResultSingleTransaction.Result);
        }

        [Fact]
        public void Test_HIVPP_3()
        {
            var rawSegment = @"HIVPP:5:1:3+@36@cbcd6314-9386-4cab-835a-62b2ba9a5b69+++urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10+@2257@<?xml version='1.0' encoding='UTF-8'?><Document xmlns=""urn:iso:std:iso:20022:tech:xsd:pain.002.001.10"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:iso:std:iso:20022:tech:xsd:pain.002.001.10 pain.002.001.10.xsd""><CstmrPmtStsRpt><GrpHdr><MsgId>3054631a98564b8399b88d60d97a9693</MsgId><CreDtTm>2025-10-10T07:15:42.128958283Z</CreDtTm><DbtrAgt><FinInstnId><BICFI>DEUTDEDB</BICFI></FinInstnId></DbtrAgt></GrpHdr><OrgnlGrpInfAndSts><OrgnlMsgId>2025-10-10 09:15:22.080</OrgnlMsgId><OrgnlMsgNmId>pain.001</OrgnlMsgNmId><OrgnlNbOfTxs>1</OrgnlNbOfTxs><OrgnlCtrlSum>2.00</OrgnlCtrlSum><StsRsnInf><AddtlInf>RCVC - The IBAN matches the name of the payee.</AddtlInf><AddtlInf>RVNM - The IBAN does not match the name of the payee.</AddtlInf><AddtlInf>RVMC - The IBAN almost matches the name of the payee.</AddtlInf><AddtlInf>RVNA - The IBAN could not be matched with the name of the payee.</AddtlInf><AddtlInf>RVNM or RVMC or RVNA : If you authorize the order anyway, this may result in the money being transferred</AddtlInf><AddtlInf>RVNM or RVMC or RVNA : to a payment account whose owner is not the payee you specified.</AddtlInf></StsRsnInf><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RCVC</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RVNM</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RVMC</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>1</DtldNbOfTxs><DtldSts>RVNA</DtldSts></NbOfTxsPerSts></OrgnlGrpInfAndSts><OrgnlPmtInfAndSts><OrgnlPmtInfId>2025-10-10 09:15:22.080</OrgnlPmtInfId><OrgnlNbOfTxs>1</OrgnlNbOfTxs><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RCVC</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RVNM</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>0</DtldNbOfTxs><DtldSts>RVMC</DtldSts></NbOfTxsPerSts><NbOfTxsPerSts><DtldNbOfTxs>1</DtldNbOfTxs><DtldSts>RVNA</DtldSts></NbOfTxsPerSts><TxInfAndSts><OrgnlEndToEndId>NOTPROVIDED</OrgnlEndToEndId><TxSts>RVNA</TxSts><StsRsnInf><Rsn><Cd>AG11</Cd></Rsn></StsRsnInf><OrgnlTxRef><Cdtr><Pty><Nm>Abid</Nm></Pty></Cdtr><CdtrAcct><Id><IBAN>DE12100500000940223457</IBAN></Id></CdtrAcct></OrgnlTxRef></TxInfAndSts></OrgnlPmtInfAndSts></CstmrPmtStsRpt></Document>++Kontrollieren Sie das VOP-Prüfergebnis. Autorisieren Sie den Auftrag trotzdem, kann das Geld an eine falsche Person gehen.";
            var segment = new Segment(rawSegment);
            segment = new GenericSegmentParser().ParseSegment(segment);
            var parser = new HIVPPSegmentParser();
            var hivpp = (HIVPP)parser.ParseSegment(segment);
            Assert.NotNull(hivpp);
            Assert.Equal("cbcd6314-9386-4cab-835a-62b2ba9a5b69", hivpp.VopId);
            Assert.Equal("urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.002.001.10", hivpp.PaymentStatusReportDescriptor);
            Assert.Equal("Kontrollieren Sie das VOP-Prüfergebnis. Autorisieren Sie den Auftrag trotzdem, kann das Geld an eine falsche Person gehen.", hivpp.AdditionalInfo);
            var document = Pain00200110.Create(hivpp.PaymentStatusReport);
        }
    }
}
