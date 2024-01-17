namespace RockPaperScissors.API.GameService.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RockPaperScissors.Core.Services;
    using RockPaperScissors.Data.Model;
    using RockPaperScissors.Data.Model.Enums;
    using RockPaperScissors.Data.Model.Requests;

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameService;

        // Should be used for prod
        // private readonly ILogger logger;
        public GamesController(IGameService gameService) // , ILogger<GamesController> logger)
        {
            // this.logger = logger;
            this.gameService = gameService;
        }

        // Get Game Status
        // GET api/games/gameId
        [HttpGet("{gameId}")]
        public ActionResult<Game> Get(Guid gameId)
        {
            var gameStatusRequest = new GameStatusRequest
            {
                GameId = gameId
            };

            var gameStatusResponse = this.gameService.CheckGameStatus(gameStatusRequest);
            if (gameStatusResponse?.IsSuccessful == false)
            {
                return this.BadRequest(gameStatusResponse);
            }

            return this.Ok(gameStatusResponse);
        }

        // Create Game
        // Post api/games/gameName
        [HttpPost]
        [Route("{gameName}")]
        public IActionResult Post(string gameName)
        {
            var createGameRequest = new CreateGameRequest
            {
                GameName = gameName
            };

            var createGameResponse = this.gameService.CreateGame(createGameRequest);
            if (createGameResponse?.IsSuccessful == false)
            {
                return this.BadRequest(createGameResponse);
            }

            return this.Created("api/games/gameName", createGameResponse);
        }

        // Join Game
        // Post api/games/gameName/playerName
        [HttpPost]
        [Route("{gameName}/{playerName}")]
        public IActionResult Post(string gameName, string playerName)
        {
            var joinGameRequest = new JoinGameRequest
            {
                GameName = gameName,
                Player = new Player
                {
                    Id = Guid.NewGuid(),
                    Name = playerName,
                    Move = Move.Empty
                }
            };
            var joinGameResponse = this.gameService.JoinGame(joinGameRequest);
            if (joinGameResponse?.IsSuccessful == false)
            {
                return this.BadRequest(joinGameResponse);
            }

            return this.Created("api/games/gameName/playerName", joinGameResponse);
        }

        // Play Game
        // Post api/games/guid/playerId/nextMove
        [HttpPost]
        [Route("{gameName}/{playerName}/{nextMove:int}")]
        public IActionResult Post(string gameName, string playerName, Move nextMove)
        {
            var playGameRequest = new PlayGameRequest
            {
                GameName = gameName,
                PlayerName = playerName,
                NextMove = nextMove
            };
            var playGameResponse = this.gameService.PlayGame(playGameRequest);
            if (playGameResponse?.IsSuccessful == false)
            {
                return this.BadRequest(playGameResponse);
            }

            return this.Created("api/games/guid/playerId/nextMove", playGameResponse);
        }
    }
}