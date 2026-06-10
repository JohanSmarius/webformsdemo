using System;
using System.Collections.Generic;

namespace BasketballScores
{
    public class Player
    {
        public Player()
        {
            IsActive = true;
            PlayerGameStats = new List<PlayerGameStat>();
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public int JerseyNumber { get; set; }
        public required string Position { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public virtual ICollection<PlayerGameStat> PlayerGameStats { get; set; }
    }

    public class Game
    {
        public Game()
        {
            PlayerGameStats = new List<PlayerGameStat>();
        }

        public int Id { get; set; }
        public DateTime GameDate { get; set; }
        public required string OpponentTeam { get; set; }
        public required string Location { get; set; }
        public int? OurScore { get; set; }
        public int? OpponentScore { get; set; }
        public bool IsCompleted { get; set; }
        public required string Notes { get; set; }

        // Navigation property
        public virtual ICollection<PlayerGameStat> PlayerGameStats { get; set; }

        public string Result
        {
            get
            {
                if (!IsCompleted || OurScore == null) return "Scheduled";
                if (OurScore > OpponentScore) return "Win";
                if (OurScore < OpponentScore) return "Loss";
                return "Draw";
            }
        }

        public string ScoreDisplay
        {
            get
            {
                if (!IsCompleted || OurScore == null) return "-";
                return OurScore + " - " + OpponentScore;
            }
        }
    }

    public class PlayerGameStat
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int Points { get; set; }
        public int Errors { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int MinutesPlayed { get; set; }

        // Navigation properties
        public required virtual Game Game { get; set; }
        public required virtual Player Player { get; set; }

        // Denormalized properties for query convenience (not mapped to DB)
        public required string PlayerName { get; set; }
        public int JerseyNumber { get; set; }
    }

    public class PlayerSummary
    {
        public int PlayerId { get; set; }
        public required string Name { get; set; }
        public int JerseyNumber { get; set; }
        public required string Position { get; set; }
        public int GamesPlayed { get; set; }
        public double AvgPoints { get; set; }
        public double AvgErrors { get; set; }
        public int TotalPoints { get; set; }
        public int TotalErrors { get; set; }
    }
}
