# .NET Version Upgrade Plan

## Overview

**Target**: Basketball Scores Web Forms application
**Scope**: Single project (BasketballScores.csproj), ~1,059 LOC, upgrading from .NET Framework 4.8 to .NET 10

### Selected Strategy
**All-at-Once** — Single project makes this a straightforward atomic migration.
**Rationale**: Single Web Forms project with no inter-project dependencies. Side-by-side approach allows incremental Blazor migration while keeping the existing app functional.

## Tasks

### 01-prerequisites: Validate upgrade prerequisites

Verify the development environment can support .NET 10 and validate any global.json SDK constraints. This includes checking that the .NET 10 SDK is installed and that no global.json files in the solution directory or parent directories pin to an incompatible SDK version.

**Done when**: .NET 10 SDK is confirmed installed, no global.json conflicts exist, and the environment is ready for the upgrade.

---

### 02-sdk-conversion: Convert project to SDK-style format

Convert BasketballScores.csproj from the legacy .NET Framework project format to the modern SDK-style format while staying on net48. This is a structural change that simplifies the project file and enables modern .NET features. The conversion will automatically migrate packages.config to PackageReference format.

**Scope**: 
- Convert BasketballScores.csproj (legacy format → SDK-style)
- Migrate packages.config to PackageReference (handled by conversion tooling)
- Stay on net48 during this task (TFM upgrade comes later)

**Key concerns from assessment**:
- Project is currently non-SDK-style (Wap, Sdk Style = False)
- No NuGet packages currently referenced (minimal PackageReference migration)

**Done when**: Project file is SDK-style format, builds successfully on net48, all packages are PackageReference-based, and no functionality has regressed.

---

### 03-data-layer-migration: Migrate data access from ADO.NET to EF Core

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

---

### 04-scaffold-basketballscores-blazor: Scaffold new Blazor Server project with YARP proxy

Create a new Blazor Server project (BasketballScores.Blazor) targeting net10.0 alongside the existing Web Forms project. Configure YARP (Yet Another Reverse Proxy) to route requests between the old Web Forms app and the new Blazor app, enabling incremental migration. The YARP proxy allows the old Web Forms pages to continue serving requests while newly migrated pages use Blazor.

**Scope**:
- Create BasketballScores.Blazor project (Blazor Server, net10.0)
- Configure YARP reverse proxy with routing rules
- Set up shared data access (reference the migrated EF Core layer from task 03)
- Create minimal Blazor shell (App.razor, MainLayout, _Host.cshtml)
- Configure both apps to run simultaneously (different ports or path-based routing)

**YARP routing strategy**:
- Default: route all requests to old Web Forms app
- As pages migrate: update YARP config to route specific paths to Blazor
- Both apps stay live throughout migration

**Done when**: Blazor project builds, runs alongside Web Forms app, YARP correctly routes requests, stub home page returns 200, ready for incremental migration.

---

### 05-migrate-basketballscores-ui: Migrate Web Forms pages to Blazor components

Incrementally migrate all Web Forms pages (.aspx) to Blazor components (.razor), moving UI logic from code-behind to component code. Migrate data-bound controls (GridView, Repeater, etc.) to Blazor patterns. This task will be broken down into subtasks at execution time (one per page or feature area).

**Scope** (4 pages to migrate):
- Default.aspx → Home.razor (dashboard/welcome page)
- Players.aspx → Players.razor (player list management)
- Games.aspx → Games.razor (game schedule list)
- GameDetail.aspx → GameDetail.razor (game details with player stats)

**Assessment context**:
- 312 System.Web API issues (57.4% of total issues) — primarily Web Forms controls
- Top incompatible APIs: TextBox (54 instances), t (45), Label (22), Repeater (16), DropDownList (11), Button (10)
- Data binding patterns: GridView/Repeater with manual data population
- Postback event handlers: button clicks, form submissions

**Migration approach per page**:
- Create Blazor component with equivalent layout
- Migrate data loading from Page_Load → OnInitializedAsync
- Replace server controls with Blazor components (@bind, EditForm, etc.)
- Migrate postback handlers to Blazor event handlers
- Update YARP routing to point page path to Blazor app
- Validate page functionality before moving to next

**Dependencies**:
- App_Code/Models.cs → shared via project reference to Blazor project
- EF Core data access layer (from task 03)
- Site.Master layout → convert to MainLayout.razor

**Done when**: All 4 Web Forms pages are converted to Blazor components, all functionality verified, YARP routes all paths to Blazor app, old Web Forms project is no longer receiving requests (but remains deployable).

---

### 06-final-validation: Validate complete migration

Build the full solution, run all tests, verify the Blazor application functions correctly end-to-end, and document any deferred recommendations. Confirm that the old Web Forms project is cleanly isolated and document its removal as a post-upgrade step.

**Validation checklist**:
- Solution builds with 0 errors and 0 warnings
- All migrated pages function correctly in Blazor
- Data access layer works correctly (CRUD operations on all tables)
- Player management, game scheduling, and stats tracking all operational
- No references to System.Web remain in active code
- Configuration properly migrated (connection strings, app settings)

**Post-upgrade recommendations to document**:
- Remove old BasketballScores Web Forms project after production validation
- Remove YARP proxy configuration once fully migrated (optional simplification)
- Consider nullable reference types enablement (future enhancement)
- Plan Phase 3: Add Aspire orchestration

**Done when**: Full solution builds successfully, Blazor app passes end-to-end validation, migration complete, post-upgrade steps documented for user.
