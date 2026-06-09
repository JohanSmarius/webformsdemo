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
