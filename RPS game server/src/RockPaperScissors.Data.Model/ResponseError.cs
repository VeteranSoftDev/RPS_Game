namespace RockPaperScissors.Data.Model
{
    using System.Net;

    public class ResponseError
    {
        public HttpStatusCode ErrorCode { get; set; }

        public string Description { get; set; }
    }
}
