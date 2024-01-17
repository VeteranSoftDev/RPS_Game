namespace RockPaperScissors.Data.Model.Responses
{
    public class PlayGameResponse
    {
        public bool IsSuccessful { get; set; }

        public ResponseError Error { get; set; }
    }
}
