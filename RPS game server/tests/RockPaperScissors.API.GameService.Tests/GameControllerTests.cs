namespace RockPaperScissors.API.GameService.Tests
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using RockPaperScissors.API.GameService.Controllers;
    using RockPaperScissors.Core.Services;
    using RockPaperScissors.Data.Model.Requests;
    using RockPaperScissors.Data.Model.Responses;
    using Xunit;

    public class GameControllerTests
    {
        private readonly Mock<IGameService> gameService = new Mock<IGameService>();

        [Fact]
        public void CreateGame_Valid_ReturnsOK()
        {
            // Arrange
            this.gameService.Setup(service => service.CreateGame(It.IsAny<CreateGameRequest>()))
                .Returns(new CreateGameResponse { GameId = Guid.NewGuid(), IsSuccessful = true });

            var controller = new GamesController(this.gameService.Object);

            // Act
            var result = controller.Post("Test Game");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void CreateGame_Invalid_ReturnsBadRequest()
        {
            // Arrange
            this.gameService.Setup(service => service.CreateGame(It.IsAny<CreateGameRequest>()))
                .Returns(new CreateGameResponse { IsSuccessful = false });

            var controller = new GamesController(this.gameService.Object);

            // Act
            var result = controller.Post("Test Game");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
