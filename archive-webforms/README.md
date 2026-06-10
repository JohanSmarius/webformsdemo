# Archived Web Forms Project

This folder contains the original ASP.NET Web Forms project files that have been migrated to Blazor Server.

## Migration Date
December 2024

## What was migrated

### Pages Migrated to Blazor
- `Default.aspx` → `BasketballScores.Blazor/Pages/Index.razor`
- `Players.aspx` → `BasketballScores.Blazor/Pages/Players.razor`
- `Games.aspx` → `BasketballScores.Blazor/Pages/Games.razor`
- `GameDetail.aspx` → `BasketballScores.Blazor/Pages/GameDetail.razor`

### Master Page
- `Site.Master` → Replaced by `BasketballScores.Blazor/Shared/MainLayout.razor`

### Data Layer
The shared data layer (`App_Code/Database.cs` and `App_Code/Models.cs`) was migrated to EF Core and is now used by the Blazor project.

## Archived Files
This folder contains:
- All `.aspx` markup files and their code-behind (`.aspx.cs`) and designer files (`.aspx.designer.cs`)
- Master page (`Site.Master`) and its code-behind
- `Web.config` (Web Forms configuration)

## Blazor Project Location
The new Blazor Server project is in: `BasketballScores.Blazor/`

## Old Project File
The old Web Forms project file remains at the solution root: `BasketballScores.csproj` (targeting .NET Framework 4.8)

## Notes
- All Web Forms pages have been successfully migrated to Blazor
- The YARP reverse proxy has been removed (no longer needed)
- The Blazor project now handles all routes including legacy `.aspx` URLs
- The shared data layer uses EF Core instead of ADO.NET

## Rollback
If you need to rollback:
1. Move files from this archive back to the solution root
2. Re-add the Web Forms project to the solution
3. Restore YARP configuration in `BasketballScores.Blazor/appsettings.json`
4. Re-add YARP package and configuration in `BasketballScores.Blazor/Program.cs`
