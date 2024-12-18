﻿using System.Threading.Tasks;
using libfintx.FinTS;
using libfintx.FinTS.Message;
using Xunit;

namespace libfintx.Tests.Message
{
    public class FinTSMessageTests
    {
        [Fact]
        public void CreateSync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            string Segments = null;

            // Act
            var result = FinTSMessage.CreateSync(
                client,
                Segments);

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            int MsgNum = 0;
            string DialogID = null;
            string Segments = null;
            int? TanProcessCode = null;
            string SystemID = null;

            // Act
            var result = FinTSMessage.Create(
                client,
                MsgNum,
                DialogID,
                Segments,
                TanProcessCode,
                SystemID);

            // Assert
            Assert.True(false);
        }

        [Fact]
        public async Task Send_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            FinTsClient client = null;
            string Message = null;

            // Act
            var result = await FinTSMessage.Send(
                client,
                Message);

            // Assert
            Assert.True(false);
        }
    }
}
