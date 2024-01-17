namespace RockPaperScissors.Data.Model.Requests
{
    using RockPaperScissors.Data.Model.Enums;

    public class PlayGameRequest
    {
        public Move NextMove { get; set; }

        public string GameName { get; set; }

        public string PlayerName { get; set; }
    }
}