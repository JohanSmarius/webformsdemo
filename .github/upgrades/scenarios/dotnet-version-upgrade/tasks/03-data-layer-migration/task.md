# 03-data-layer-migration: Migrate data access from ADO.NET to EF Core

Replace the existing ADO.NET data access layer (Database.cs) with Entity Framework Core. The current implementation uses SqlConnection/SqlCommand with manual SQL queries. This task creates EF Core DbContext, entities with proper relationships, and migrates all data access methods.

**Scope**:
- Create BasketballDbContext with DbSet<Player>, DbSet<Game>, DbSet<PlayerGameStat>
- Configure entity relationships and table mappings
- Replace all Database.cs methods with EF Core queries
- Update connection string handling (Web.config → appsettings.json approach for future)
- Maintain existing data schema (Players, Games, PlayerGameStats tables)

**Assessment context**:
- 242 System.Data.SqlClient API issues (source incompatible) — migrate to Microsoft.Data.SqlClient, then to EF Core
- Database initialization logic in Database.cs with manual table creation SQL
- CRUD operations across 3 tables with foreign key relationships

**Done when**: All data access goes through EF Core, no ADO.NET code remains, existing functionality preserved, project builds and tests pass.

## Research Findings

### Files to Modify
- **App_Code/Database.cs** (369 lines) — Complete replacement with EF Core implementation
- **App_Code/Models.cs** (96 lines) — Already EF-ready with navigation properties, minimal changes needed
- **BasketballScores.csproj** — Add EF Core NuGet packages
- **Web.config** — Connection strings remain compatible

### Current Data Access Methods (13 total)
**Players:**
- GetPlayers(bool activeOnly) — Filter by IsActive
- GetPlayer(int id) — Single player lookup
- SavePlayer(Player p) — Insert or Update
- DeletePlayer(int id) — Soft delete (set IsActive=false)

**Games:**
- GetGames() — All games ordered by date
- GetGame(int id) — Single game lookup
- SaveGame(Game g) — Insert or Update, returns new ID
- DeleteGame(int id) — Hard delete with cascade to stats

**PlayerGameStats:**
- GetStatsForGame(int gameId) — All stats for a game with player details
- SaveStat(PlayerGameStat s) — Insert or Update stat entry

**Aggregates:**
- GetPlayerSummaries() — Complex JOIN with AVG/SUM calculations

**Schema:**
- Initialize() — Create tables if not exist

### Database Schema
**Players:**
- Id (PK, IDENTITY)
- Name (NVARCHAR(200), NOT NULL)
- JerseyNumber (INT, NOT NULL)
- Position (NVARCHAR(50))
- IsActive (BIT, NOT NULL, DEFAULT 1)

**Games:**
- Id (PK, IDENTITY)
- GameDate (DATE, NOT NULL)
- OpponentTeam (NVARCHAR(200), NOT NULL)
- Location (NVARCHAR(200))
- OurScore (INT, nullable)
- OpponentScore (INT, nullable)
- IsCompleted (BIT, NOT NULL, DEFAULT 0)
- Notes (NVARCHAR(MAX))

**PlayerGameStats:**
- Id (PK, IDENTITY)
- GameId (FK → Games.Id, NOT NULL)
- PlayerId (FK → Players.Id, NOT NULL)
- Points, Errors, Assists, Rebounds, MinutesPlayed (INT, NOT NULL, DEFAULT 0)
- UNIQUE constraint on (GameId, PlayerId)

### Packages to Add
- Microsoft.EntityFrameworkCore (8.0.x for net48 compatibility)
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools (for migrations if needed later)

### EF Core Migration Strategy
1. Add EF Core NuGet packages
2. Create BasketballDbContext with DbSet properties
3. Configure entity mappings (Fluent API in OnModelCreating)
4. Implement EnsureCreated() or migration-based initialization
5. Replace Database.cs static methods with instance DbContext usage OR keep static wrapper with internal DbContext
6. Update Global.asax.cs Application_Start to call initialization
7. Verify connection string format in Web.config

### Dependencies & Risks
- Web Forms pages heavily use Database.cs static methods (all 4 pages)
- Keep static API surface to minimize page changes
- EF Core 8.0 supports .NET Framework 4.6.2+, compatible with net48
- Connection string format should be compatible (no breaking changes expected)

### Decisions Made
- Keep Database class as static facade with internal DbContext usage
- Use EF Core 3.1.32 (latest version supporting .NET Framework 4.8)
- Use EnsureCreated() for database initialization (matches current behavior)
- Preserve exact SQL schema and constraints
- No changes to Models.cs (already has navigation properties)
- Used `HasName` instead of `HasDatabaseName` for index naming (EF Core 3.1 API)

## Research Findings

### Current Data Access Layer Analysis

**Database.cs** (369 lines):
- Static utility class with ADO.NET operations
- Connection: ConfigurationManager.ConnectionStrings["BasketballDB"]
- Initialize(): Creates 3 tables if they don't exist (Players, Games, PlayerGameStats)

**CRUD Methods Inventory** (14 methods to migrate):
1. `GetPlayers(bool activeOnly)` — List all/active players
2. `GetPlayer(int id)` — Single player by ID
3. `AddPlayer(Player)` — Insert new player
4. `UpdatePlayer(Player)` — Update existing player
5. `DeletePlayer(int id)` — Delete player by ID
6. `GetGames()` — List all games ordered by date
7. `GetGame(int id)` — Single game by ID
8. `AddGame(Game)` — Insert new game
9. `UpdateGame(Game)` — Update existing game
10. `DeleteGame(int id)` — Delete game by ID
11. `GetPlayerGameStats(int gameId)` — Stats for a specific game with player details
12. `GetPlayerSummaries()` — Aggregated player statistics
13. `SavePlayerGameStats(int gameId, List<PlayerGameStat>)` — Batch save/update stats
14. `Initialize()` — Database schema creation

### Entity Model Review

**Models.cs** (78 lines):
- `Player` — Id, Name, JerseyNumber, Position, IsActive
- `Game` — Id, GameDate, OpponentTeam, Location, OurScore, OpponentScore, IsCompleted, Notes
  - Computed: Result (Win/Loss/Draw/Scheduled), ScoreDisplay
- `PlayerGameStat` — Id, GameId, PlayerId, Points, Errors, Assists, Rebounds, MinutesPlayed
  - Denormalized: PlayerName, JerseyNumber (from joins)
- `PlayerSummary` — DTO for aggregated stats (not a table)

### Database Schema
```sql
Players (Id PK, Name, JerseyNumber, Position, IsActive)
Games (Id PK, GameDate, OpponentTeam, Location, OurScore, OpponentScore, IsCompleted, Notes)
PlayerGameStats (Id PK, GameId FK, PlayerId FK, Points, Errors, Assists, Rebounds, MinutesPlayed)
  - UNIQUE(GameId, PlayerId)
```

### Files to Modify
1. **BasketballScores.csproj** — Add EF Core NuGet packages
2. **App_Code/Models.cs** — Add navigation properties for EF relationships
3. **App_Code/Database.cs** — Replace with BasketballDbContext
4. **Global.asax.cs** — Update Initialize() call to use EF migrations
5. **Default.aspx.cs** — Update data access calls
6. **Players.aspx.cs** — Update player CRUD calls
7. **Games.aspx.cs** — Update game CRUD calls
8. **GameDetail.aspx.cs** — Update stats queries

### Packages to Add
| Package | Version | Notes |
|---------|---------|-------|
| Microsoft.EntityFrameworkCore | Latest for net48 | Core EF package |
| Microsoft.EntityFrameworkCore.SqlServer | Latest for net48 | SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | Latest for net48 | Design-time tools |

### Migration Strategy
1. Add EF Core NuGet packages (net48 compatible versions)
2. Enhance Models.cs with navigation properties
3. Create BasketballDbContext with OnModelCreating configuration
4. Replace Database.cs static methods with DbContext extension methods
5. Update all 4 .aspx.cs files to use new DbContext API
6. Update Global.asax.cs to use EF Database.EnsureCreated()
7. Build and fix any compilation errors
8. Test basic CRUD operations

### Dependencies & Risks
- **Net48 compatibility**: Must use EF Core versions compatible with .NET Framework 4.8 (likely 3.1.x or lower)
- **Connection string**: Keep in Web.config for now (appsettings.json is ASP.NET Core only)
- **Static method pattern**: Current pages expect Database.GetPlayers() — need compatible replacement
- **PlayerGameStat denormalization**: Current queries join Player data into stats — preserve this pattern

### Decisions Made
- Use EF Core 3.1 (last version fully supporting net48)
- Keep static helper pattern with DbContext per-request lifecycle
- Preserve denormalized PlayerGameStat DTO pattern
- Use Database.EnsureCreated() instead of manual schema DDL
