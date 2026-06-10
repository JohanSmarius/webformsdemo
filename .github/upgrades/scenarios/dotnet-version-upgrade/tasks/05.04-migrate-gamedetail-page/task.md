# 05.04-migrate-gamedetail-page: Migrate GameDetail.aspx → GameDetail.razor (stats entry)

## Objective
Migrate GameDetail.aspx (game details with player stats entry) to Blazor Server component Pages/GameDetail.razor with query string routing and bulk stat updates.

## Scope
- Create Pages/GameDetail.razor with @page "/GameDetail" and @page "/GameDetail.aspx" (with query string support)
- Handle query string parameters: id (required), new (optional welcome message)
- Migrate game details display and edit form
- Migrate player stats Repeater → dynamic form with one row per active player
- Handle ViewModel merge: active players + existing stats → display rows
- Implement bulk stat save (iterate all rows, save each)
- Two separate save buttons: Update Game Info, Save Player Stats
- Validate query string, redirect to /Games if missing/invalid
- Inject BasketballDbContext and NavigationManager
- Update YARP routing for /GameDetail.aspx

## Query String Handling
```razor
@page "/GameDetail"
@page "/GameDetail.aspx"
@using Microsoft.AspNetCore.WebUtilities

@code {
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private BasketballDbContext Db { get; set; } = default!;
    
    private int gameId;
    private bool showWelcome;
    
    protected override async Task OnInitializedAsync()
    {
        var uri = new Uri(Nav.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);
        
        if (!query.TryGetValue("id", out var idStr) || !int.TryParse(idStr, out gameId) || gameId <= 0)
        {
            Nav.NavigateTo("/Games");
            return;
        }
        
        showWelcome = query.ContainsKey("new") && query["new"] == "1";
        
        await LoadGame();
        await LoadStats();
    }
}
```

## ViewModel for Stats Grid
```csharp
private class StatRow
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
    public int JerseyNumber { get; set; }
    public string Position { get; set; } = "";
    public int Points { get; set; }
    public int Errors { get; set; }
    public int Assists { get; set; }
    public int Rebounds { get; set; }
    public int MinutesPlayed { get; set; }
}

private List<StatRow> statRows = new();

private async Task LoadStats()
{
    var players = Database.GetPlayers(activeOnly: true);
    var existingStats = Database.GetStatsForGame(gameId);
    var statMap = existingStats.ToDictionary(s => s.PlayerId);
    
    statRows = players.Select(p => new StatRow
    {
        PlayerId = p.Id,
        PlayerName = p.Name,
        JerseyNumber = p.JerseyNumber,
        Position = p.Position,
        Points = statMap.ContainsKey(p.Id) ? statMap[p.Id].Points : 0,
        Errors = statMap.ContainsKey(p.Id) ? statMap[p.Id].Errors : 0,
        Assists = statMap.ContainsKey(p.Id) ? statMap[p.Id].Assists : 0,
        Rebounds = statMap.ContainsKey(p.Id) ? statMap[p.Id].Rebounds : 0,
        MinutesPlayed = statMap.ContainsKey(p.Id) ? statMap[p.Id].MinutesPlayed : 0
    }).ToList();
}
```

## Bulk Stats Save
```razor
<!-- Player Stats Table -->
@if (statRows.Count == 0)
{
    <p>No active players. Add players before entering stats.</p>
}
else
{
    <table>
        <thead>
            <tr>
                <th>Player</th>
                <th>Jersey</th>
                <th>Position</th>
                <th>Points</th>
                <th>Errors</th>
                <th>Assists</th>
                <th>Rebounds</th>
                <th>Minutes</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var row in statRows)
            {
                <tr>
                    <td>@row.PlayerName</td>
                    <td>@row.JerseyNumber</td>
                    <td>@row.Position</td>
                    <td><InputNumber @bind-Value="row.Points" /></td>
                    <td><InputNumber @bind-Value="row.Errors" /></td>
                    <td><InputNumber @bind-Value="row.Assists" /></td>
                    <td><InputNumber @bind-Value="row.Rebounds" /></td>
                    <td><InputNumber @bind-Value="row.MinutesPlayed" /></td>
                </tr>
            }
        </tbody>
    </table>
    <button @onclick="SaveAllStats">Save Player Stats</button>
}

@code {
    private async Task SaveAllStats()
    {
        foreach (var row in statRows)
        {
            var stat = new PlayerGameStat
            {
                GameId = gameId,
                PlayerId = row.PlayerId,
                Points = Math.Max(0, row.Points),
                Errors = Math.Max(0, row.Errors),
                Assists = Math.Max(0, row.Assists),
                Rebounds = Math.Max(0, row.Rebounds),
                MinutesPlayed = Math.Max(0, row.MinutesPlayed)
            };
            Database.SaveStat(stat);
        }
        
        message = "Player stats saved.";
        messageIsSuccess = true;
        await LoadStats();  // Reload to show saved values
    }
}
```

## Two Separate Update Buttons
```razor
<!-- Game Info Form -->
<EditForm Model="@currentGame" OnValidSubmit="UpdateGameInfo">
    <!-- game fields: date, opponent, location, scores, status, notes -->
    <button type="submit">Update Game Info</button>
</EditForm>

<!-- Player Stats Table (above) -->
<!-- Has its own button: Save Player Stats -->
```

## YARP Routing Update
```json
"gamedetail-page": {
  "ClusterId": "local-blazor",
  "Match": { "Path": "/GameDetail.aspx" },
  "Order": 5
}
```

## Done When
- [ ] GameDetail.razor loads game details from query string ?id=X
- [ ] Redirects to /Games if id is missing or invalid
- [ ] Displays welcome message when ?new=1 is present
- [ ] Game info form displays and updates correctly (date, opponent, location, scores, status, notes)
- [ ] Player stats table shows all active players with existing stats (or zeros)
- [ ] Update Game Info button saves game changes
- [ ] Save Player Stats button saves all stat rows in bulk
- [ ] Empty state displays when no active players exist
- [ ] Page header shows "vs. {OpponentTeam}" and game date/location
- [ ] Both /GameDetail?id=X and /GameDetail.aspx?id=X routes work
- [ ] Navigation from Games page (after add or view) works correctly
- [ ] Page builds and runs without errors
