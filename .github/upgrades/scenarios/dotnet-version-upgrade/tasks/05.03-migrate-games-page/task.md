# 05.03-migrate-games-page: Migrate Games.aspx → Games.razor (schedule CRUD)

## Objective
Migrate Games.aspx (game schedule management) to Blazor Server component Pages/Games.razor with full CRUD and navigation to GameDetail.

## Scope
- Create Pages/Games.razor with @page "/Games" and @page "/Games.aspx"
- Migrate game list Repeater → @foreach with action buttons
- Migrate add/edit form (date, opponent, location, scores, status, notes)
- Handle conditional fields: scores only if completed
- Implement Save with conditional navigation (redirect to GameDetail after add)
- Handle Edit, Delete, View Detail actions
- Inject BasketballDbContext and NavigationManager
- Update NavMenu.razor to add Games link
- Update YARP routing for /Games.aspx

## Controls → Blazor Components

**Form with conditional fields**:
```razor
<EditForm Model="@currentGame" OnValidSubmit="SaveGame">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <InputDate @bind-Value="currentGame.GameDate" />
    <InputText @bind-Value="currentGame.OpponentTeam" placeholder="Opponent Team" />
    <InputText @bind-Value="currentGame.Location" placeholder="Location" />
    <InputSelect @bind-Value="currentGame.IsCompleted">
        <option value="false">Scheduled</option>
        <option value="true">Completed</option>
    </InputSelect>
    
    @if (currentGame.IsCompleted)
    {
        <InputNumber @bind-Value="currentGame.OurScore" placeholder="Our Score" />
        <InputNumber @bind-Value="currentGame.OpponentScore" placeholder="Opponent Score" />
    }
    
    <InputTextArea @bind-Value="currentGame.Notes" placeholder="Notes" />
    
    <button type="submit">@(isEditMode ? "Update" : "Add") Game</button>
    @if (isEditMode)
    {
        <button type="button" @onclick="CancelEdit">Cancel</button>
    }
</EditForm>
```

**Game list with multiple actions**:
```razor
@foreach (var game in games)
{
    <div class="game-row">
        <span>@game.GameDate.ToString("MM/dd") vs @game.OpponentTeam (@game.Location)</span>
        <span>@(game.IsCompleted ? $"{game.Result} {game.OurScore}-{game.OpponentScore}" : "Scheduled")</span>
        <button @onclick="() => ViewGameDetail(game.Id)">View</button>
        <button @onclick="() => EditGame(game.Id)">Edit</button>
        <button @onclick="() => DeleteGame(game.Id)">Delete</button>
    </div>
}
```

## Navigation Logic
```csharp
@code {
    [Inject] private BasketballDbContext Db { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    
    private async Task SaveGame()
    {
        int savedId = Database.SaveGame(currentGame);
        message = isEditMode ? "Game updated." : "Game added.";
        messageIsSuccess = true;
        
        if (!isEditMode)
        {
            // Navigate to GameDetail for new game (matches Web Forms redirect)
            Nav.NavigateTo($"/GameDetail?id={savedId}&new=1");
        }
        else
        {
            CancelEdit();
            await LoadGames();
        }
    }
    
    private void ViewGameDetail(int id)
    {
        Nav.NavigateTo($"/GameDetail?id={id}");
    }
}
```

## YARP Routing Update
```json
"games-page": {
  "ClusterId": "local-blazor",
  "Match": { "Path": "/Games.aspx" },
  "Order": 4
}
```

## Navigation Update
Update `Shared/NavMenu.razor`:
```razor
<NavLink href="games" Match="NavLinkMatch.All">
    <span class="oi oi-calendar" aria-hidden="true"></span> Games
</NavLink>
```

## Done When
- [ ] Games.razor displays game schedule with all details
- [ ] Add game form works with all fields
- [ ] Score fields show/hide based on IsCompleted toggle
- [ ] Edit game loads existing data
- [ ] Update game saves changes
- [ ] Delete game removes from database
- [ ] Cancel button resets form to add mode
- [ ] After adding new game, navigates to GameDetail with ?new=1
- [ ] View button navigates to GameDetail for existing games
- [ ] Validation enforces required fields and valid dates
- [ ] NavMenu includes Games link
- [ ] Both /Games and /Games.aspx routes work
- [ ] Page builds and runs without errors
