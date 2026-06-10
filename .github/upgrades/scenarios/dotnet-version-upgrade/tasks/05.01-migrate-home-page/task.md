# 05.01-migrate-home-page: Migrate Default.aspx → Index.razor (dashboard)

## Objective
Migrate Default.aspx (home/dashboard page) to Blazor Server component Index.razor.

## Scope
- Create Index.razor component (already has stub, replace content)
- Migrate Page_Load → OnInitializedAsync
- Replace 5 Literal controls with Blazor binding (@litGames → @gamesCount)
- Replace 2 Repeaters (rptRecentGames, rptTopScorers) with @foreach loops
- Replace 2 Labels for empty states with @if visibility
- Inject BasketballDbContext for data access
- Update YARP routing: add route for "/" to Blazor app

## Data Access
- GetGames().Where(g => g.IsCompleted).ToList() → aggregate wins/losses/draws/total
- GetPlayers(activeOnly: true).Count → player count
- GetGames().Take(5) → recent games list
- GetPlayerSummaries().OrderByDescending(p => p.TotalPoints).Take(10) → top scorers

## Migration Pattern
**Code-behind → Component code block**:
```csharp
@code {
    [Inject] private BasketballDbContext Db { get; set; } = default!;
    private int gamesCount, winsCount, lossesCount, drawsCount, playersCount;
    private List<Game> recentGames = new();
    private List<PlayerSummary> topScorers = new();
    
    protected override async Task OnInitializedAsync()
    {
        var games = Db.Games.Where(g => g.IsCompleted).ToList();
        gamesCount = games.Count;
        winsCount = games.Count(g => g.Result == "Win");
        // ... etc
    }
}
```

**Repeater → @foreach**:
```razor
@if (recentGames.Count == 0)
{
    <p>No games yet.</p>
}
else
{
    @foreach (var game in recentGames)
    {
        <div>@game.OpponentTeam - @game.GameDate.ToString("d")</div>
    }
}
```

## YARP Routing Update
Update `BasketballScores.Blazor/appsettings.json`:
```json
"Routes": {
  "blazor-home": {
    "ClusterId": "local-blazor",
    "Match": { "Path": "/" },
    "Order": 1
  },
  "blazor-default": {
    "ClusterId": "local-blazor",
    "Match": { "Path": "/Default.aspx" },
    "Order": 2
  },
  "fallback-to-webforms": { /* existing fallback */ }
}
```

## Done When
- [ ] Index.razor displays dashboard stats (games, wins, losses, draws, players)
- [ ] Recent games list shows correctly (top 5)
- [ ] Top scorers list shows correctly (top 10)
- [ ] Empty states display when no data
- [ ] Blazor app builds successfully
- [ ] Navigate to http://localhost:5000/ shows the migrated Blazor dashboard
- [ ] Navigate to http://localhost:5000/Default.aspx also routes to Blazor dashboard
- [ ] No errors in browser console
