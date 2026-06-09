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
