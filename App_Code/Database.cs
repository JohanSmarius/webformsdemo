using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Web;

namespace BasketballScores
{
    public static class Database
    {
        private static string GetDbPath()
        {
            string relativePath = ConfigurationManager.AppSettings["DatabasePath"];
            return HttpContext.Current.Server.MapPath(relativePath);
        }

        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={GetDbPath()};Version=3;");
        }

        public static void Initialize()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Players (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        JerseyNumber INTEGER NOT NULL,
                        Position TEXT,
                        IsActive INTEGER NOT NULL DEFAULT 1
                    );
                    CREATE TABLE IF NOT EXISTS Games (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameDate TEXT NOT NULL,
                        OpponentTeam TEXT NOT NULL,
                        Location TEXT,
                        OurScore INTEGER,
                        OpponentScore INTEGER,
                        IsCompleted INTEGER NOT NULL DEFAULT 0,
                        Notes TEXT
                    );
                    CREATE TABLE IF NOT EXISTS PlayerGameStats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameId INTEGER NOT NULL,
                        PlayerId INTEGER NOT NULL,
                        Points INTEGER NOT NULL DEFAULT 0,
                        Errors INTEGER NOT NULL DEFAULT 0,
                        Assists INTEGER NOT NULL DEFAULT 0,
                        Rebounds INTEGER NOT NULL DEFAULT 0,
                        MinutesPlayed INTEGER NOT NULL DEFAULT 0,
                        FOREIGN KEY (GameId) REFERENCES Games(Id),
                        FOREIGN KEY (PlayerId) REFERENCES Players(Id),
                        UNIQUE(GameId, PlayerId)
                    );";
                cmd.ExecuteNonQuery();
            }
        }

        // ── Players ──────────────────────────────────────────────────────────

        public static List<Player> GetPlayers(bool activeOnly = false)
        {
            var list = new List<Player>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = activeOnly
                    ? "SELECT * FROM Players WHERE IsActive=1 ORDER BY JerseyNumber"
                    : "SELECT * FROM Players ORDER BY JerseyNumber";
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Players WHERE Id=@id";
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
                var cmd = conn.CreateCommand();
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
                cmd.Parameters.AddWithValue("@active", p.IsActive ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeletePlayer(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Players SET IsActive=0 WHERE Id=@id";
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Games ORDER BY GameDate DESC";
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Games WHERE Id=@id";
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
                var cmd = conn.CreateCommand();
                if (g.Id == 0)
                {
                    cmd.CommandText = @"INSERT INTO Games (GameDate,OpponentTeam,Location,OurScore,OpponentScore,IsCompleted,Notes)
                                        VALUES (@date,@opp,@loc,@us,@them,@done,@notes);
                                        SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("@date", g.GameDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@opp", g.OpponentTeam);
                    cmd.Parameters.AddWithValue("@loc", g.Location ?? "");
                    cmd.Parameters.AddWithValue("@us", g.OurScore.HasValue ? (object)g.OurScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@them", g.OpponentScore.HasValue ? (object)g.OpponentScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@done", g.IsCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@notes", g.Notes ?? "");
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    cmd.CommandText = @"UPDATE Games SET GameDate=@date,OpponentTeam=@opp,Location=@loc,
                                        OurScore=@us,OpponentScore=@them,IsCompleted=@done,Notes=@notes
                                        WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", g.Id);
                    cmd.Parameters.AddWithValue("@date", g.GameDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@opp", g.OpponentTeam);
                    cmd.Parameters.AddWithValue("@loc", g.Location ?? "");
                    cmd.Parameters.AddWithValue("@us", g.OurScore.HasValue ? (object)g.OurScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@them", g.OpponentScore.HasValue ? (object)g.OpponentScore.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@done", g.IsCompleted ? 1 : 0);
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
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tx;
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT s.*, p.Name, p.JerseyNumber
                    FROM PlayerGameStats s
                    JOIN Players p ON s.PlayerId = p.Id
                    WHERE s.GameId=@gid
                    ORDER BY p.JerseyNumber";
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO PlayerGameStats (GameId,PlayerId,Points,Errors,Assists,Rebounds,MinutesPlayed)
                    VALUES (@gid,@pid,@pts,@err,@ast,@reb,@min)
                    ON CONFLICT(GameId,PlayerId) DO UPDATE SET
                        Points=excluded.Points,
                        Errors=excluded.Errors,
                        Assists=excluded.Assists,
                        Rebounds=excluded.Rebounds,
                        MinutesPlayed=excluded.MinutesPlayed";
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
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT p.Id, p.Name, p.JerseyNumber, p.Position,
                           COUNT(s.Id) AS GamesPlayed,
                           COALESCE(SUM(s.Points),0) AS TotalPoints,
                           COALESCE(SUM(s.Errors),0) AS TotalErrors
                    FROM Players p
                    LEFT JOIN PlayerGameStats s ON p.Id = s.PlayerId
                    WHERE p.IsActive = 1
                    GROUP BY p.Id
                    ORDER BY p.JerseyNumber";
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

        // ── Mappers ──────────────────────────────────────────────────────────

        private static Player MapPlayer(IDataReader r) => new Player
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            Name = r.GetString(r.GetOrdinal("Name")),
            JerseyNumber = r.GetInt32(r.GetOrdinal("JerseyNumber")),
            Position = r.IsDBNull(r.GetOrdinal("Position")) ? "" : r.GetString(r.GetOrdinal("Position")),
            IsActive = r.GetInt32(r.GetOrdinal("IsActive")) == 1
        };

        private static Game MapGame(IDataReader r)
        {
            var g = new Game
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                GameDate = DateTime.Parse(r.GetString(r.GetOrdinal("GameDate"))),
                OpponentTeam = r.GetString(r.GetOrdinal("OpponentTeam")),
                Location = r.IsDBNull(r.GetOrdinal("Location")) ? "" : r.GetString(r.GetOrdinal("Location")),
                IsCompleted = r.GetInt32(r.GetOrdinal("IsCompleted")) == 1,
                Notes = r.IsDBNull(r.GetOrdinal("Notes")) ? "" : r.GetString(r.GetOrdinal("Notes"))
            };
            int usCol = r.GetOrdinal("OurScore");
            int themCol = r.GetOrdinal("OpponentScore");
            if (!r.IsDBNull(usCol)) g.OurScore = r.GetInt32(usCol);
            if (!r.IsDBNull(themCol)) g.OpponentScore = r.GetInt32(themCol);
            return g;
        }

        private static PlayerGameStat MapStat(IDataReader r) => new PlayerGameStat
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
