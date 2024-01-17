namespace RockPaperScissors.Data.Model.Tests
{
    using System;
    using RockPaperScissors.Data.Model.Enums;
    using RockPaperScissors.Data.Model.Requests;
    using Xunit;

    public class RequestTests
    {
        private readonly Player testPlayer;
        private readonly Guid playerGuid;
        private readonly Guid gameGuid;
        private readonly string gameName;
        private readonly string playerName;
        private readonly Move nextMove;

        public RequestTests()
        {
            this.nextMove = Move.Empty;
            this.gameName = "Test Game";
            this.playerName = "Test Player";
            this.playerGuid = Guid.NewGuid();
            this.gameGuid = Guid.NewGuid();
            this.testPlayer = new Player
            {
                Id = this.playerGuid,
                Move = Move.Empty,
                Name = "Test Player One"
            };
        }

        [Fact]
        public void PlayGameRequest_Correct_ObjectCreated()
        {
            // Arrange
            var playGameRequest = new PlayGameRequest
            {
                GameName = this.gameName,
                PlayerName = this.playerName,
                NextMove = this.nextMove
            };

            // Act
            // Assert
            Assert.Equal(this.gameName, playGameRequest.GameName);
            Assert.Equal(this.playerName, playGameRequest.PlayerName);
            Assert.Equal(this.nextMove, playGameRequest.NextMove);
        }

        [Fact]
        public void JoinGameRequest_Correct_ObjectCreated()
        {
            // Arrange
            var joinGameRequest = new JoinGameRequest()
            {
                Player = this.testPlayer,
                GameName = this.gameName
            };

            // Act
            // Assert
            Assert.Equal(this.gameName, joinGameRequest.GameName);
            Assert.Equal(this.testPlayer, joinGameRequest.Player);
        }

        [Fact]
        public void CreateGameRequest_Correct_ObjectCreated()
        {
            // Arrange
            var createGameRequest = new CreateGameRequest
            {
                GameName = this.gameName
            };

            // Act
            // Assert
            Assert.Equal(this.gameName, createGameRequest.GameName);
        }

        [Fact]
        public void GameStatusRequest_Correct_ObjectCreated()
        {
            // Arrange
            var gameStatusRequest = new GameStatusRequest
            {
                GameId = this.gameGuid
            };

            // Act
            // Assert
            Assert.Equal(this.gameGuid, gameStatusRequest.GameId);
        }
    }
}
