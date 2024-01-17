namespace RockPaperScissors.Data.Model
{
    using System;
    using RockPaperScissors.Data.Model.Enums;

    public class Player
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Move Move { get; set; }
    }
}
