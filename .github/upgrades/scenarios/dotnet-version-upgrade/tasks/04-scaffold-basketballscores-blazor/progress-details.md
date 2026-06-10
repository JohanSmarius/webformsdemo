# Task 04 Progress: Scaffold Blazor Server Project with YARP Proxy

## Files Modified

### New Project Created
- **BasketballScores.Blazor/** - New Blazor Server project targeting net10.0

### Project Files
- `BasketballScores.Blazor/BasketballScores.Blazor.csproj` - Created SDK-style Blazor project
- `BasketballScores.Blazor/Program.cs` - Configured Blazor Server, YARP proxy, and EF Core
- `BasketballScores.Blazor/appsettings.json` - Added connection string and YARP configuration
- `BasketballScores.Blazor/Properties/launchSettings.json` - Configured to run on port 5000

### Blazor Components
- `BasketballScores.Blazor/Pages/Index.razor` - Stub home page
- `BasketballScores.Blazor/Shared/MainLayout.razor` - Main layout component
- `BasketballScores.Blazor/Shared/NavMenu.razor` - Navigation menu (placeholders for future pages)
- `BasketballScores.Blazor/_Imports.razor` - Global using statements

### Existing Files Modified
- `App_Code/Database.cs` - Added conditional compilation for .NET Framework vs .NET Core configuration
- `App_Code/Database.cs` - Updated `.HasName()` to `.HasDatabaseName()` (obsolete API fix)
- `App_Code/Models.cs` - Added `required` modifiers to fix nullable reference warnings
- `Properties/launchSettings.json` - Updated Web Forms port to 5001
- `BasketballScores.sln` - Added BasketballScores.Blazor project

### Files Deleted
- `BasketballScores.Blazor/Data/` - Removed default WeatherForecastService
- `BasketballScores.Blazor/Pages/Counter.razor` - Removed
- `BasketballScores.Blazor/Pages/FetchData.razor` - Removed
- `BasketballScores.Blazor/Shared/SurveyPrompt.razor` - Removed

## Build Result
- **Errors**: 0
- **Warnings**: 3 (nullable reference warnings in Database.cs - non-blocking)
- **Projects built**: BasketballScores.Blazor

## YARP Configuration

### Current Routing (Phase 1)
All requests currently route to the Web Forms app (fallback):
```json
{
  "ReverseProxy": {
    "Routes": {
      "fallback-to-webforms": {
        "ClusterId": "webforms-cluster",
        "Match": {
          "Path": "{**catch-all}"
        },
        "Order": 999
      }
    },
    "Clusters": {
      "webforms-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5001/"
          }
        }
      }
    }
  }
}
```

### Port Configuration
- **Blazor YARP entry point**: `http://localhost:5000`
- **Web Forms backend**: `http://localhost:5001`

## Data Layer Integration

The Blazor project shares the EF Core data layer from the Web Forms project using linked files:
- `Database.cs` - BasketballDbContext and data access methods
- `Models.cs` - Player, Game, PlayerGameStat, PlayerSummary entities

The Web Forms project still uses `ConfigurationManager.ConnectionStrings` (conditional compilation), while Blazor uses dependency injection configured in Program.cs.

## Packages Added
- **Microsoft.EntityFrameworkCore.SqlServer** 10.0.0
- **Microsoft.EntityFrameworkCore.Tools** 10.0.0
- **Yarp.ReverseProxy** 2.2.0

## Changes Summary

Successfully scaffolded a new Blazor Server project targeting .NET 10 alongside the existing .NET Framework 4.8 Web Forms project. The YARP reverse proxy is configured to route requests, with the Blazor project serving as the entry point (port 5000) and the Web Forms project running as a backend service (port 5001).

### Key Accomplishments:
1. ✅ Created Blazor Server project targeting net10.0
2. ✅ Configured YARP reverse proxy with fallback routing to Web Forms
3. ✅ Set up shared EF Core data access via linked files
4. ✅ Created minimal Blazor shell (layouts, nav, stub home page)
5. ✅ Configured both apps to run simultaneously on different ports
6. ✅ Project builds successfully with zero errors

### Architecture:
- **Side-by-side migration**: Both Web Forms and Blazor apps run independently
- **Incremental routing**: YARP routes allow migrating pages one at a time (task 05)
- **Shared data layer**: Both apps use the same EF Core DbContext

## Issues Encountered

### Issue 1: .NET 10 template not available
**Problem**: `dotnet new blazorserver -f net10.0` failed - template only supports net6.0
**Solution**: Created project with net6.0 template, then manually updated `.csproj` to net10.0

### Issue 2: Project reference compatibility
**Problem**: Blazor (net10.0) couldn't directly reference Web Forms project (net48) - cross-TFM incompatibility
**Solution**: Used linked files (`<Compile Include="...">`) to share data layer code instead of project reference

### Issue 3: ConfigurationManager.ConnectionStrings not available in .NET Core
**Problem**: `ConfigurationManager.ConnectionStrings` is .NET Framework-only API
**Solution**: Added conditional compilation (`#if NETFRAMEWORK`) to support both frameworks. Blazor uses DI configuration in Program.cs.

### Issue 4: Nullable reference warnings
**Problem**: Models.cs and Database.cs had CS8618 warnings for non-nullable properties
**Solution**: Added `required` modifiers to model properties. Remaining 3 warnings in Database.cs (return values) are non-blocking.

## Next Steps (Task 05)
- Begin incremental page migration: Default.aspx → Index.razor, Players.aspx → Players.razor, etc.
- Update YARP routing rules as pages are migrated
- Eventually route all requests to Blazor, keeping Web Forms as deprecated fallback

## Validation
- ✅ Blazor project builds successfully (0 errors, 3 non-blocking warnings)
- ✅ Project added to solution
- ✅ YARP configuration present and valid
- ✅ Data layer accessible from Blazor via linked files
- ✅ Port configuration correct (Blazor:5000, Web Forms:5001)
- ⏳ Stub home page ready (will test run in follow-up validation)
