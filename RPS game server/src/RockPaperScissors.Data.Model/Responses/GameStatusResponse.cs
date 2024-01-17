namespace RockPaperScissors.Data.Model.Responses
{
    using RockPaperScissors.Data.Model.Enums;

    public class GameStatusResponse
    {
        public bool IsSuccessful { get; set; }

        public GameStatus Status { get; set; }

        public ResponseError Error { get; set; }
    }
}
