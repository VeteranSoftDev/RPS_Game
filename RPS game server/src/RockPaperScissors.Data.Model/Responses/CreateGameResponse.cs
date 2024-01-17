namespace RockPaperScissors.Data.Model.Responses
{
    using System;

    public class CreateGameResponse
    {
        public Guid GameId { get; set; }

        public bool IsSuccessful { get; set; }

        public ResponseError Error { get; set; }
    }
}
