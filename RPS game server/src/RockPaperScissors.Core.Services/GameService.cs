namespace RockPaperScissors.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using RockPaperScissors.Data.Model;
    using RockPaperScissors.Data.Model.Enums;
    using RockPaperScissors.Data.Model.Requests;
    using RockPaperScissors.Data.Model.Responses;

    public class GameService : IGameService
    {
        private readonly List<Game> games;

        public GameService()
        {
            this.games = new List<Game>();
        }

        /// <summary>
        /// Create a new game
        /// </summary>
        /// <param name="request">Create game request</param>
        /// <returns>Create Game Response</returns>
        public CreateGameResponse CreateGame(CreateGameRequest request)
        {
            // Validate request, you will never know
            if (string.IsNullOrEmpty(request?.GameName))
            {
                return new CreateGameResponse
                {
                    GameId = Guid.Empty,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game name is unique
            var isDuplicate = this.games.Exists(item => item.Name == request.GameName);
            if (isDuplicate)
            {
                return new CreateGameResponse
                {
                    GameId = Guid.Empty,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Just create now
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Name = request.GameName,
                Status = GameStatus.Created
            };
            this.games.Add(game);

            return new CreateGameResponse
            {
                GameId = game.Id,
                IsSuccessful = true
            };
        }

        public JoinGameResponse JoinGame(JoinGameRequest request)
        {
            // Validate request, you will never know again
            if (string.IsNullOrEmpty(request?.GameName) || string.IsNullOrEmpty(request?.Player?.Name))
            {
                return new JoinGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game exists
            var game = this.games.FirstOrDefault(item => item.Name == request.GameName);
            if (game == null)
            {
                return new JoinGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game isn't full or finished
            if (game.IsFull || game.IsFinished)
            {
                return new JoinGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // The real joining happens now
            // First player will just join
            if (game.FirstPlayer == null)
            {
                game.FirstPlayer = request.Player;
            }

            // Second player is joining
            else if (game.SecondPlayer == null)
            {
                game.SecondPlayer = request.Player;

                // Set flag for the future
                game.IsFull = true;
            }
            else
            {
                return new JoinGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            return new JoinGameResponse
            {
                IsSuccessful = true
            };
        }

        public PlayGameResponse PlayGame(PlayGameRequest request)
        {
            // Validate request, you will never know again and again
            if (string.IsNullOrEmpty(request?.GameName) || string.IsNullOrEmpty(request?.PlayerName) ||
                request?.NextMove == Move.Empty)
            {
                return new PlayGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game exists
            var game = this.games.FirstOrDefault(item => item.Name == request.GameName);
            if (game == null)
            {
                return new PlayGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game isn't finished
            if (game.IsFinished)
            {
                return new PlayGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate that player is allowed to play in this game
            if (game.FirstPlayer.Name != request.PlayerName && game.SecondPlayer.Name != request.PlayerName)
            {
                return new PlayGameResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Time to play
            // If player one move is pending
            if (game.FirstPlayer.Name == request.PlayerName)
            {
                switch (game.Status)
                {
                    case GameStatus.PlayerOneMovePending:
                        game.FirstPlayer.Move = request.NextMove;
                        game.IsFinished = true;
                        break;
                    case GameStatus.Created:
                        game.FirstPlayer.Move = request.NextMove;
                        game.Status = GameStatus.PlayerTwoMovePending;
                        break;
                    default:
                        return new PlayGameResponse
                        {
                            IsSuccessful = false,
                            Error = new ResponseError
                            {
                                ErrorCode = HttpStatusCode.BadRequest,
                                Description = HttpStatusCode.BadRequest.ToString()
                            }
                        };
                }
            }

            // If player two move is pending
            else if (game.SecondPlayer.Name == request.PlayerName)
            {
                switch (game.Status)
                {
                    case GameStatus.PlayerTwoMovePending:
                        game.SecondPlayer.Move = request.NextMove;
                        game.IsFinished = true;
                        break;
                    case GameStatus.Created:
                        game.SecondPlayer.Move = request.NextMove;
                        game.Status = GameStatus.PlayerOneMovePending;
                        break;
                    default:
                        return new PlayGameResponse
                        {
                            IsSuccessful = false,
                            Error = new ResponseError
                            {
                                ErrorCode = HttpStatusCode.BadRequest,
                                Description = HttpStatusCode.BadRequest.ToString()
                            }
                        };
                }
            }

            return new PlayGameResponse
            {
                IsSuccessful = true
            };
        }

        public GameStatusResponse CheckGameStatus(GameStatusRequest request)
        {
            // Validate request, you will never know again, again and again
            if (request.GameId == Guid.Empty)
            {
                return new GameStatusResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            // Validate Game exists
            var game = this.games.FirstOrDefault(item => item.Id == request.GameId);
            if (game == null)
            {
                return new GameStatusResponse
                {
                    IsSuccessful = false,
                    Error = new ResponseError
                    {
                        ErrorCode = HttpStatusCode.BadRequest,
                        Description = HttpStatusCode.BadRequest.ToString()
                    }
                };
            }

            return new GameStatusResponse
            {
                IsSuccessful = true,
                Status = game.IsFinished ? Battle(game.FirstPlayer.Move, game.SecondPlayer.Move) : game.Status
            };
        }

        private static GameStatus Battle(Move firstPlayerMove, Move secondPlayerMove)
        {
            switch (firstPlayerMove)
            {
                case Move.Paper when secondPlayerMove == Move.Rock:
                    return GameStatus.PlayerOneWon;
                case Move.Paper when secondPlayerMove == Move.Scissors:
                    return GameStatus.PlayerTwoWon;
                case Move.Scissors when secondPlayerMove == Move.Paper:
                    return GameStatus.PlayerOneWon;
                case Move.Scissors when secondPlayerMove == Move.Rock:
                    return GameStatus.PlayerTwoWon;
                case Move.Rock when secondPlayerMove == Move.Paper:
                    return GameStatus.PlayerTwoWon;
                case Move.Rock when secondPlayerMove == Move.Scissors:
                    return GameStatus.PlayerOneWon;
                default:
                    return GameStatus.Tie;
            }
        }
    }
}
