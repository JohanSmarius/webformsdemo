# 05-migrate-basketballscores-ui: Migrate Web Forms pages to Blazor components

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
