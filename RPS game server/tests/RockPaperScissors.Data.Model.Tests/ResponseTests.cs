namespace RockPaperScissors.Data.Model.Tests
{
    using System;
    using System.Net;
    using RockPaperScissors.Data.Model.Enums;
    using RockPaperScissors.Data.Model.Responses;
    using Xunit;

    public class ResponseTests
    {
        private readonly bool isSuccessful;
        private readonly Guid gameGuid;
        private readonly GameStatus gameStatus;
        private readonly ResponseError errorOk;
        private readonly ResponseError error404;

        public ResponseTests()
        {
            this.isSuccessful = false;
            this.gameStatus = GameStatus.Created;
            this.gameGuid = Guid.NewGuid();

            this.errorOk = new ResponseError
            {
                ErrorCode = HttpStatusCode.OK,
                Description = "OK"
            };

            this.error404 = new ResponseError
            {
                ErrorCode = HttpStatusCode.NotFound,
                Description = "Not Found"
            };
        }

        [Fact]
        public void PlayGameResponse_Correct_ObjectCreated()
        {
            // Arrange
            var playGameResponse = new PlayGameResponse
            {
                IsSuccessful = this.isSuccessful,
                Error = this.error404
            };

            // Act
            // Assert
            Assert.Equal(this.isSuccessful, playGameResponse.IsSuccessful);
            Assert.Equal(this.error404, playGameResponse.Error);
        }

        [Fact]
        public void JoinGameResponse_Correct_ObjectCreated()
        {
            // Arrange
            var joinGameResponse = new JoinGameResponse
            {
                IsSuccessful = this.isSuccessful,
                Error = this.error404
            };

            // Act
            // Assert
            Assert.Equal(this.isSuccessful, joinGameResponse.IsSuccessful);
            Assert.Equal(this.error404, joinGameResponse.Error);
        }

        [Fact]
        public void CreateGameResponse_Correct_ObjectCreated()
        {
            // Arrange
            var createGameResponse = new CreateGameResponse
            {
                GameId = this.gameGuid,
                IsSuccessful = this.isSuccessful,
                Error = this.errorOk
            };

            // Act
            // Assert
            Assert.Equal(this.gameGuid, createGameResponse.GameId);
            Assert.Equal(this.isSuccessful, createGameResponse.IsSuccessful);
            Assert.Equal(this.errorOk, createGameResponse.Error);
        }

        [Fact]
        public void GameStatusResponse_Correct_ObjectCreated()
        {
            // Arrange
            var gameStatusResponse = new GameStatusResponse
            {
                Status = this.gameStatus,
                Error = this.errorOk,
                IsSuccessful = this.isSuccessful
            };

            // Act
            // Assert
            Assert.Equal(this.gameStatus, gameStatusResponse.Status);
            Assert.Equal(this.errorOk, gameStatusResponse.Error);
            Assert.Equal(this.isSuccessful, gameStatusResponse.IsSuccessful);
        }
    }
}
