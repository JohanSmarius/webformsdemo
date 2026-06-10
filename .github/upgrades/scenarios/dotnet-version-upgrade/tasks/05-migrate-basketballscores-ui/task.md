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

---

## Research Findings

### Pages to Migrate (4 total)

#### 1. Default.aspx → Index.razor (Home/Dashboard)
- **Complexity**: LOW - Read-only dashboard, simple data display
- **Controls**: 
  - Literals (litGames, litWins, litLosses, litDraws, litPlayers) - 5 stats
  - 2 Repeaters (rptRecentGames for 5 recent games, rptTopScorers for top 10 scorers)
  - 2 Labels for empty states (lblNoGames, lblNoPlayers)
- **Data access**: 
  - Read-only queries: GetGames(), GetPlayers(), GetPlayerSummaries()
  - Aggregation: Count wins/losses/draws from games
- **Migration effort**: ~1 hour
- **Subtask**: 05.01-migrate-home-page

#### 2. Players.aspx → Players.razor (Player Management)
- **Complexity**: MEDIUM - Full CRUD with inline edit/add form, validation
- **Controls**: 
  - 1 Repeater (rptPlayers) with Edit/Delete buttons per row
  - Add/Edit form: TextBox (name, jersey), DropDownList (position), HiddenField (editId)
  - Buttons: Save, Cancel
  - Labels for messages and empty state
  - Page validation (RequiredFieldValidator, RangeValidator)
- **Data access**: 
  - Read: GetPlayerSummaries()
  - Write: SavePlayer(), DeletePlayer(), GetPlayer()
- **Event handlers**: btnSave_Click, btnCancel_Click, rptPlayers_ItemCommand (Edit/Delete)
- **State management**: Form reset, edit mode toggle, message display
- **Migration effort**: ~2 hours
- **Subtask**: 05.02-migrate-players-page

#### 3. Games.aspx → Games.razor (Game Schedule)
- **Complexity**: MEDIUM-HIGH - CRUD with conditional fields, redirect after add
- **Controls**: 
  - 1 Repeater (rptGames) with Edit/Delete/View buttons per row
  - Add/Edit form: Multiple TextBoxes (date, opponent, location, scores, notes), DropDownList (status), HiddenField (editId)
  - Buttons: Save, Cancel
  - Labels for messages and empty state
  - Page validation (RequiredFieldValidator, CompareValidator for dates)
- **Data access**: 
  - Read: GetGames(), GetGame()
  - Write: SaveGame() (returns ID for redirect), DeleteGame()
- **Event handlers**: btnSave_Click (with conditional redirect), btnCancel_Click, rptGames_ItemCommand
- **Special logic**: After creating new game, redirect to GameDetail with ID
- **Migration effort**: ~2-3 hours
- **Subtask**: 05.03-migrate-games-page

#### 4. GameDetail.aspx → GameDetail.razor (Game Details with Stats)
- **Complexity**: HIGH - Query string routing, nested forms, bulk stat update, complex ViewModel
- **Controls**: 
  - Game edit form (similar to Games.aspx)
  - Stats Repeater (rptStats) with dynamic TextBox rows for each active player
  - Each stat row: HiddenField (playerId) + 5 TextBoxes (points, errors, assists, rebounds, minutes)
  - 2 update buttons (btnUpdateGame, btnSaveStats)
  - Panels for visibility control (pnlStats, pnlNoPlayers)
  - Labels for messages and page header (litTitle, litSubtitle)
- **Data access**: 
  - Read: GetGame(), GetPlayers(activeOnly: true), GetStatsForGame()
  - Write: SaveGame(), SaveStat() (called in loop)
- **Special logic**: 
  - Query string validation (id parameter, redirect if missing)
  - Welcome message on ?new=1
  - Merge logic: active players + existing stats → StatRow ViewModel
  - Bulk save: iterate Repeater.Items, extract TextBox values, save each stat
- **Event handlers**: Page_Load with query string, btnUpdateGame_Click, btnSaveStats_Click
- **Migration effort**: ~3-4 hours
- **Subtask**: 05.04-migrate-gamedetail-page

### Migration Approach

This task requires decomposition into subtasks - one per page. The task will assess each page, then call `break_down_task` with the subtask breakdown. Each subtask will:

1. Analyze the Web Forms page (markup + code-behind)
2. Create equivalent Blazor component
3. Migrate Page_Load → OnInitializedAsync
4. Replace Web Forms controls → Blazor components
5. Migrate event handlers → Blazor event handlers
6. Update YARP routing for the page
7. Test the migrated page

### Recommended Subtask Order
1. **05.01-migrate-home-page** (Default.aspx) - Likely simplest, dashboard/welcome
2. **05.02-migrate-players-page** (Players.aspx) - CRUD with form inputs
3. **05.03-migrate-games-page** (Games.aspx) - List view, likely simpler than GameDetail
4. **05.04-migrate-gamedetail-page** (GameDetail.aspx) - Most complex, nested data (Game + Stats)

### YARP Routing Updates

As pages are migrated, update `appsettings.json` in Blazor project to route specific paths to Blazor instead of Web Forms backend.

**Initial state** (all to Web Forms):
```json
"Routes": {
  "fallback-to-webforms": {
    "ClusterId": "webforms-cluster",
    "Match": { "Path": "{**catch-all}" },
    "Order": 999
  }
}
```

**After migrating Default.aspx**:
```json
"Routes": {
  "blazor-home": {
    "ClusterId": "local-blazor",
    "Match": { "Path": "/" },
    "Order": 1
  },
  "fallback-to-webforms": { ... }
}
```

_(Pattern continues for each page)_

### Next Steps

**This task MUST be decomposed** before execution. The orchestrator will:
1. Read each .aspx/.aspx.cs file to assess complexity
2. Call `break_down_task` with 4 subtasks (one per page)
3. Execute each subtask in order, validating before proceeding
