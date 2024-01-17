namespace RockPaperScissors.Core.Services
{
    using RockPaperScissors.Data.Model.Requests;
    using RockPaperScissors.Data.Model.Responses;

    public interface IGameService
    {
        CreateGameResponse CreateGame(CreateGameRequest request);

        JoinGameResponse JoinGame(JoinGameRequest request);

        PlayGameResponse PlayGame(PlayGameRequest request);

        GameStatusResponse CheckGameStatus(GameStatusRequest request);
    }
}