using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.FinTS;
using Xunit;

namespace libfintx.Tests
{
    public class Test_UPD
    {
        [Fact]
        public void Test_UPD_1()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\BankMessage_4.txt");
            var message = File.ReadAllText(path);
            var upaMatch = Regex.Match(message, @"(HIUPA.+?)\b(HITAN|HNHBS|HIKIM)\b");
            Assert.True(upaMatch.Success);

            var upd = upaMatch.Groups[1].Value;

            UPD.ParseUpd(upd);
            Assert.NotEmpty(UPD.AccountList);
        }
    }
}
