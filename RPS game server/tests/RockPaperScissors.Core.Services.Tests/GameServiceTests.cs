namespace RockPaperScissors.Core.Services.Tests
{
    using System;
    using System.Net;
    using RockPaperScissors.Data.Model.Enums;
    using RockPaperScissors.Data.Model.Requests;
    using Xunit;

    public class GameServiceTests
    {
        private readonly IGameService gameService;
        private readonly string gameName;

        public GameServiceTests()
        {
            this.gameService = new GameService();
            this.gameName = "Test Game";
        }

        [Fact]
        public void CreateGame_EmptyGameName_ReturnBadRequest()
        {
            // Arrange
            var createGameRequest = new CreateGameRequest
            {
                GameName = string.Empty
            };

            // Act
            var createGameResponse = this.gameService.CreateGame(createGameRequest);

            // Assert
            Assert.False(createGameResponse.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, createGameResponse.Error.ErrorCode);
            Assert.Equal(Guid.Empty, createGameResponse.GameId);
        }

        [Fact]
        public void CreateGame_DuplicateGameName_ReturnBadRequest()
        {
            // Arrange
            var createGameRequest = new CreateGameRequest
            {
                GameName = this.gameName
            };

            // Act
            var createGameResponseWorks = this.gameService.CreateGame(createGameRequest);
            var createGameResponseFails = this.gameService.CreateGame(createGameRequest);

            // Assert
            Assert.True(createGameResponseWorks.IsSuccessful);
            Assert.NotEqual(Guid.Empty, createGameResponseWorks.GameId);

            Assert.False(createGameResponseFails.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, createGameResponseFails.Error.ErrorCode);
            Assert.Equal(Guid.Empty, createGameResponseFails.GameId);
        }

        [Fact]
        public void CreateGame_Correct_ReturnOk()
        {
            // Arrange
            var createGameRequest = new CreateGameRequest
            {
                GameName = this.gameName
            };

            // Act
            var createGameResponse = this.gameService.CreateGame(createGameRequest);

            // Assert
            Assert.True(createGameResponse.IsSuccessful);
            Assert.NotEqual(Guid.Empty, createGameResponse.GameId);
        }

        [Fact]
        public void GetGameStatus_EmptyId_ReturnBadRequest()
        {
            // Arrange
            var gameStatusRequest = new GameStatusRequest
            {
                GameId = Guid.Empty
            };

            // Act
            var gameStatusResponse = this.gameService.CheckGameStatus(gameStatusRequest);

            // Assert
            Assert.False(gameStatusResponse.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, gameStatusResponse.Error.ErrorCode);
        }

        [Fact]
        public void GetGameStatus_InvalidId_ReturnBadRequest()
        {
            // Arrange
            var createGameRequest = new CreateGameRequest
            {
                GameName = this.gameName
            };
            var createGameResponse = this.gameService.CreateGame(createGameRequest);
            var gameStatusRequest = new GameStatusRequest
            {
                GameId = createGameResponse.GameId
            };

            // Act
            var gameStatusResponse = this.gameService.CheckGameStatus(gameStatusRequest);

            // Assert
            Assert.True(gameStatusResponse.IsSuccessful);
            Assert.Equal(GameStatus.Created, gameStatusResponse.Status);
        }

        [Fact]
        public void GetGameStatus_CorrectId_ReturnOk()
        {
            // Arrange
            var gameStatusRequest = new GameStatusRequest
            {
                GameId = Guid.NewGuid()
            };

            // Act
            var gameStatusResponse = this.gameService.CheckGameStatus(gameStatusRequest);

            // Assert
            Assert.False(gameStatusResponse.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, gameStatusResponse.Error.ErrorCode);
        }
    }
}
