using Moq;
using MikkiNavarroWalletConsoleApp.Service;
using MikkiNavarroWalletConsoleApp.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace WalletConsoleTest
{
    public class UnitTest1
    {
        [Fact]
        public void Transfer_ShouldTransferSuccessfully_WhenCalledWithValidArguments()
        {
            // Arrange
            var mockTransaction = new Mock<ITransaction>();
            var command = new Mock<SqlCommand>();
            command.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1); // Simulate successful DB operation

            mockTransaction.Setup(db => db.CreateCommand(It.IsAny<string>(), It.IsAny<CommandType>())).Returns(command.Object);

            var transferService = new TransferFundService(mockTransaction.Object);

            // Act
            Action action = () => transferService.Transfer("123", "456", 100, 1, 2, new byte[0], new byte[0]);

            // Assert
            var exception = Record.Exception(action);
            Assert.Null(exception);
        }

        [Fact]
        public void Transfer_ShouldThrowInvalidOperationException_WhenConcurrencyConflictOccurs()
        {
            // Arrange
            var mockTransaction = new Mock<ITransaction>();
            var command = new Mock<SqlCommand>();

            // Simulate a SQL Exception which represents a concurrency conflict.
            var concurrencyConflictException = SqlExceptionHelper.CreateSqlException(50000, "Concurrency conflict detected");
            command.Setup(cmd => cmd.ExecuteNonQuery()).Throws(concurrencyConflictException);

            mockTransaction.Setup(db => db.CreateCommand(It.IsAny<string>(), It.IsAny<CommandType>())).Returns(command.Object);

            var transferService = new TransferFundService(mockTransaction.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => transferService.Transfer("123", "456", 100, 1, 2, new byte[0], new byte[0]));
        }

    }
}