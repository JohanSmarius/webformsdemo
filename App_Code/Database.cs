using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace BasketballScores
{
    public static class Database
    {
        private static SqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["BasketballDB"].ConnectionString;
            return new SqlConnection(connStr);
        }

        public static void Initialize()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                Execute(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Players')
                    BEGIN
                        CREATE TABLE Players (
                            Id           INT IDENTITY(1,1) PRIMARY KEY,
                            Name         NVARCHAR(200)     NOT NULL,
                            JerseyNumber INT               NOT NULL,
                            Position     NVARCHAR(50),
                            IsActive     BIT               NOT NULL DEFAULT 1
                        )
                    END");

                Execute(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Games')
                    BEGIN
                        CREATE TABLE Games (
                            Id            INT IDENTITY(1,1) PRIMARY KEY,
                            GameDate      DATE              NOT NULL,
                            OpponentTeam  NVARCHAR(200)     NOT NULL,
                            Location      NVARCHAR(200),
                            OurScore      INT,
                            OpponentScore INT,
                            IsCompleted   BIT               NOT NULL DEFAULT 0,
                            Notes         NVARCHAR(MAX)
                        )
                    END");

                Execute(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PlayerGameStats')
                    BEGIN
                        CREATE TABLE PlayerGameStats (
                            Id            INT IDENTITY(1,1) PRIMARY KEY,
                            GameId        INT NOT NULL,
                            PlayerId      INT NOT NULL,
                            Points        INT NOT NULL DEFAULT 0,
                            Errors        INT NOT NULL DEFAULT 0,
                            Assists       INT NOT NULL DEFAULT 0,
                            Rebounds      INT NOT NULL DEFAULT 0,
                            MinutesPlayed INT NOT NULL DEFAULT 0,
                            CONSTRAINT FK_PGS_Games   FOREIGN KEY (GameId)   REFERENCES Games(Id),
                            CONSTRAINT FK_PGS_Players FOREIGN KEY (PlayerId) REFERENCES Players(Id),
                            CONSTRAINT UC_PlayerGameStats UNIQUE (GameId, PlayerId)
                        )
                    END");
            }
        }

        private static void Execute(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        // ── Players ──────────────────────────────────────────────────────────

        public static List<Player> GetPlayers(bool activeOnly = false)
        {
            var list = new List<Player>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    activeOnly
                        ? "SELECT * FROM Players WHERE IsActive=1 ORDER BY JerseyNumber"
                        : "SELECT * FROM Players ORDER BY JerseyNumber",
                    conn);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(MapPlayer(r));
            }
            return list;
        }

        public static Player GetPlayer(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Players WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    if (r.Read()) return MapPlayer(r);
            }
            return null;
        }

        public static void SavePlayer(Player p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("", conn);
                if (p.Id == 0)
                {
                    cmd.CommandText = @"INSERT INTO Players (Name,JerseyNumber,Position,IsActive)
                                        VALUES (@name,@jersey,@pos,@active)";
                }
                else
                {
                    cmd.CommandText = @"UPDATE Players SET Name=@name,JerseyNumber=@jersey,
                                        Position=@pos,IsActive=@active WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", p.Id);
                }
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@jersey", p.JerseyNumber);
                cmd.Parameters.AddWithValue("@pos", p.Position ?? "");
                cmd.Parameters.AddWithValue("@active", p.IsActive);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeletePlayer(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Players SET IsActive=0 WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ── Games ─────────────────────────────────────────────────────────────

        public static List<Game> GetGames()
        {
            var list = new List<Game>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Games ORDER BY GameDate DESC", conn);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(MapGame(r));
            }
            return list;
        }

        public static Game GetGame(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Games WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    if (r.Read()) return MapGame(r);
            }
            return null;
        }

        public static int SaveGame(Game g)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("", conn);
                if (g.Id == 0)
                {
                    cmd.CommandText = @"INSERT INTO Games (GameDate,OpponentTeam,Location,OurScore,OpponentScore,IsCompleted,Notes)
                                        VALUES (@date,@opp,@loc,@us,@them,@done,@notes);
                                        SELECT SCOPE_IDENTITY();";
                    cmd.Parameters.AddWithValue("@date", g.GameDate);
                    cmd.Parameters.AddWithValue("@opp", g.OpponentTeam);
                    cmd.Parameters.AddWithValue("@loc", g.Location ?? "");
                    cmd.Parameters.AddWithValue("@us", g.OurScore.HasValue ? (object)g.OurScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@them", g.OpponentScore.HasValue ? (object)g.OpponentScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@done", g.IsCompleted);
                    cmd.Parameters.AddWithValue("@notes", g.Notes ?? "");
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    cmd.CommandText = @"UPDATE Games SET GameDate=@date,OpponentTeam=@opp,Location=@loc,
                                        OurScore=@us,OpponentScore=@them,IsCompleted=@done,Notes=@notes
                                        WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", g.Id);
                    cmd.Parameters.AddWithValue("@date", g.GameDate);
                    cmd.Parameters.AddWithValue("@opp", g.OpponentTeam);
                    cmd.Parameters.AddWithValue("@loc", g.Location ?? "");
                    cmd.Parameters.AddWithValue("@us", g.OurScore.HasValue ? (object)g.OurScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@them", g.OpponentScore.HasValue ? (object)g.OpponentScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@done", g.IsCompleted);
                    cmd.Parameters.AddWithValue("@notes", g.Notes ?? "");
                    cmd.ExecuteNonQuery();
                    return g.Id;
                }
            }
        }

        public static void DeleteGame(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    var cmd = new SqlCommand("", conn, tx);
                    cmd.CommandText = "DELETE FROM PlayerGameStats WHERE GameId=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE FROM Games WHERE Id=@id";
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                }
            }
        }

        // ── Player Game Stats ─────────────────────────────────────────────────

        public static List<PlayerGameStat> GetStatsForGame(int gameId)
        {
            var list = new List<PlayerGameStat>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT s.*, p.Name, p.JerseyNumber
                    FROM PlayerGameStats s
                    JOIN Players p ON s.PlayerId = p.Id
                    WHERE s.GameId=@gid
                    ORDER BY p.JerseyNumber", conn);
                cmd.Parameters.AddWithValue("@gid", gameId);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(MapStat(r));
            }
            return list;
        }

        public static void SaveStat(PlayerGameStat s)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    MERGE PlayerGameStats AS target
                    USING (VALUES (@gid, @pid, @pts, @err, @ast, @reb, @min))
                        AS source (GameId, PlayerId, Points, Errors, Assists, Rebounds, MinutesPlayed)
                    ON target.GameId = source.GameId AND target.PlayerId = source.PlayerId
                    WHEN MATCHED THEN
                        UPDATE SET Points=source.Points, Errors=source.Errors, Assists=source.Assists,
                                   Rebounds=source.Rebounds, MinutesPlayed=source.MinutesPlayed
                    WHEN NOT MATCHED THEN
                        INSERT (GameId, PlayerId, Points, Errors, Assists, Rebounds, MinutesPlayed)
                        VALUES (source.GameId, source.PlayerId, source.Points, source.Errors,
                                source.Assists, source.Rebounds, source.MinutesPlayed);", conn);
                cmd.Parameters.AddWithValue("@gid", s.GameId);
                cmd.Parameters.AddWithValue("@pid", s.PlayerId);
                cmd.Parameters.AddWithValue("@pts", s.Points);
                cmd.Parameters.AddWithValue("@err", s.Errors);
                cmd.Parameters.AddWithValue("@ast", s.Assists);
                cmd.Parameters.AddWithValue("@reb", s.Rebounds);
                cmd.Parameters.AddWithValue("@min", s.MinutesPlayed);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<PlayerSummary> GetPlayerSummaries()
        {
            var list = new List<PlayerSummary>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT p.Id, p.Name, p.JerseyNumber, p.Position,
                           COUNT(s.Id) AS GamesPlayed,
                           COALESCE(SUM(s.Points),0) AS TotalPoints,
                           COALESCE(SUM(s.Errors),0) AS TotalErrors
                    FROM Players p
                    LEFT JOIN PlayerGameStats s ON p.Id = s.PlayerId
                    WHERE p.IsActive = 1
                    GROUP BY p.Id, p.Name, p.JerseyNumber, p.Position
                    ORDER BY p.JerseyNumber", conn);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int gp = r.GetInt32(r.GetOrdinal("GamesPlayed"));
                        int tp = r.GetInt32(r.GetOrdinal("TotalPoints"));
                        int te = r.GetInt32(r.GetOrdinal("TotalErrors"));
                        list.Add(new PlayerSummary
                        {
                            PlayerId = r.GetInt32(0),
                            Name = r.GetString(1),
                            JerseyNumber = r.GetInt32(2),
                            Position = r.IsDBNull(3) ? "" : r.GetString(3),
                            GamesPlayed = gp,
                            TotalPoints = tp,
                            TotalErrors = te,
                            AvgPoints = gp > 0 ? Math.Round((double)tp / gp, 1) : 0,
                            AvgErrors = gp > 0 ? Math.Round((double)te / gp, 1) : 0
                        });
                    }
            }
            return list;
        }

        private static Player MapPlayer(IDataReader r)
        {
            return new Player
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Name = r.GetString(r.GetOrdinal("Name")),
                JerseyNumber = r.GetInt32(r.GetOrdinal("JerseyNumber")),
                Position = r.IsDBNull(r.GetOrdinal("Position")) ? "" : r.GetString(r.GetOrdinal("Position")),
                IsActive = r.GetBoolean(r.GetOrdinal("IsActive"))
            };
        }

        private static Game MapGame(IDataReader r)
        {
            var g = new Game
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                GameDate = r.GetDateTime(r.GetOrdinal("GameDate")),
                OpponentTeam = r.GetString(r.GetOrdinal("OpponentTeam")),
                Location = r.IsDBNull(r.GetOrdinal("Location")) ? "" : r.GetString(r.GetOrdinal("Location")),
                IsCompleted = r.GetBoolean(r.GetOrdinal("IsCompleted")),
                Notes = r.IsDBNull(r.GetOrdinal("Notes")) ? "" : r.GetString(r.GetOrdinal("Notes"))
            };
            int usCol = r.GetOrdinal("OurScore");
            int themCol = r.GetOrdinal("OpponentScore");
            if (!r.IsDBNull(usCol)) g.OurScore = r.GetInt32(usCol);
            if (!r.IsDBNull(themCol)) g.OpponentScore = r.GetInt32(themCol);
            return g;
        }

        private static PlayerGameStat MapStat(IDataReader r)
        {
            return new PlayerGameStat
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                GameId = r.GetInt32(r.GetOrdinal("GameId")),
                PlayerId = r.GetInt32(r.GetOrdinal("PlayerId")),
                PlayerName = r.GetString(r.GetOrdinal("Name")),
                JerseyNumber = r.GetInt32(r.GetOrdinal("JerseyNumber")),
                Points = r.GetInt32(r.GetOrdinal("Points")),
                Errors = r.GetInt32(r.GetOrdinal("Errors")),
                Assists = r.GetInt32(r.GetOrdinal("Assists")),
                Rebounds = r.GetInt32(r.GetOrdinal("Rebounds")),
                MinutesPlayed = r.GetInt32(r.GetOrdinal("MinutesPlayed"))
            };
        }
    }
}
