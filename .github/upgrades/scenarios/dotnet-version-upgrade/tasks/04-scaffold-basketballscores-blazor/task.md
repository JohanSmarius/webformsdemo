# 04-scaffold-basketballscores-blazor: Scaffold new Blazor Server project with YARP proxy

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

## Research Findings

### Current Solution Structure
- **BasketballScores.csproj**: SDK-style Web Forms project on net48
  - Pages: Default.aspx, Players.aspx, Games.aspx, GameDetail.aspx
  - Master page: Site.Master
  - Styles: Styles/site.css
  - EF Core DbContext: `BasketballDbContext` (migrated in task 03)
  - Entity models: Player, Game, PlayerGameStat
  - Connection string: Web.config → "BasketballDB"

### Projects to Create
1. **BasketballScores.Blazor** (Blazor Server, net10.0)
   - Entry point for the new Blazor app
   - Will host YARP reverse proxy
   - Will eventually replace Web Forms project

### Files to Create

#### BasketballScores.Blazor Project
- **BasketballScores.Blazor.csproj** - SDK-style Blazor Server project
- **Program.cs** - Configure Blazor Server, YARP, EF Core, and services
- **appsettings.json** / **appsettings.Development.json** - Connection string, logging
- **App.razor** - Blazor root component
- **_Imports.razor** - Global using statements
- **Pages/_Host.cshtml** - Server-side rendering host page
- **Pages/_Layout.cshtml** - HTML layout wrapper
- **Shared/MainLayout.razor** - Main layout component (replaces Site.Master)
- **Pages/Index.razor** - Stub home page (temporary, replaced by migrated Default.aspx)

### Packages Required

#### BasketballScores.Blazor
- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.x for net10.0)
- **Microsoft.EntityFrameworkCore.Tools** 
- **Yarp.ReverseProxy** (latest 2.x)

### YARP Configuration Strategy

**Phase 1 (this task)**: Default routing
```json
{
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "webforms-cluster",
        "Match": {
          "Path": "{**catch-all}"
        }
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

**Phase 2 (during task 05)**: As pages migrate, add specific routes
```json
{
  "ReverseProxy": {
    "Routes": {
      "blazor-home": {
        "ClusterId": "local-blazor",
        "Match": {
          "Path": "/"
        },
        "Order": 1
      },
      "fallback": {
        "ClusterId": "webforms-cluster",
        "Match": {
          "Path": "{**catch-all}"
        },
        "Order": 999
      }
    }
  }
}
```

### Port Configuration
- **Blazor app (YARP entry point)**: http://localhost:5000
- **Web Forms app (backend)**: http://localhost:5001 (will run independently)

### Shared Data Access
- **BasketballDbContext** will be referenced from BasketballScores.csproj
- Connection string moved to appsettings.json in Blazor project
- Dependency injection setup in Program.cs

### Risks & Considerations
- **Web Forms project must run independently**: Need to ensure BasketballScores.csproj can still run on its own on a different port
- **Static files**: Styles/Scripts may need to be copied or proxy configured to serve them
- **Session state**: Web Forms uses ViewState/Session - Blazor uses different state management (will migrate in task 05)

### Decisions Made
1. **Blazor Server over Blazor WebAssembly**: Better for incremental migration, server-side rendering matches Web Forms model
2. **YARP in Blazor project**: Centralized entry point, simpler than separate gateway project
3. **Shared DbContext reference**: Both projects reference the same EF Core context from Web Forms project until full migration
4. **Port-based routing**: Simpler than IIS-based routing, easier for development
