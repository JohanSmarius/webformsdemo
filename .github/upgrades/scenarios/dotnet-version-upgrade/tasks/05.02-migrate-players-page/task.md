# 05.02-migrate-players-page: Migrate Players.aspx → Players.razor (CRUD)

## Objective
Migrate Players.aspx (player management) to Blazor Server component Pages/Players.razor with full CRUD functionality.

## Scope
- Create Pages/Players.razor with @page "/Players" and @page "/Players.aspx"
- Migrate player list Repeater → @foreach with action buttons
- Migrate add/edit form (name, jersey, position dropdown)
- Implement form state management (add mode vs edit mode)
- Migrate validation (required fields, range validation)
- Handle Save, Cancel, Edit, Delete actions
- Inject BasketballDbContext
- Update NavMenu.razor to add Players link
- Update YARP routing for /Players.aspx

## Controls → Blazor Components

**Repeater → @foreach with buttons**:
```razor
@foreach (var player in players)
{
    <div class="player-row">
        <span>@player.Name (#@player.JerseyNumber) - @player.Position</span>
        <button @onclick="() => EditPlayer(player.PlayerId)">Edit</button>
        <button @onclick="() => DeletePlayer(player.PlayerId)">Delete</button>
    </div>
}
```

**TextBox/DropDownList → InputText/InputSelect with EditForm**:
```razor
<EditForm Model="@currentPlayer" OnValidSubmit="SavePlayer">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <InputText @bind-Value="currentPlayer.Name" placeholder="Player Name" />
    <InputNumber @bind-Value="currentPlayer.JerseyNumber" placeholder="Jersey #" />
    <InputSelect @bind-Value="currentPlayer.Position">
        <option value="">Select Position</option>
        <option value="Guard">Guard</option>
        <option value="Forward">Forward</option>
        <option value="Center">Center</option>
    </InputSelect>
    
    <button type="submit">@(isEditMode ? "Update" : "Add") Player</button>
    @if (isEditMode)
    {
        <button type="button" @onclick="CancelEdit">Cancel</button>
    }
</EditForm>
```

## State Management
```csharp
@code {
    [Inject] private BasketballDbContext Db { get; set; } = default!;
    
    private List<PlayerSummary> players = new();
    private Player currentPlayer = new();
    private bool isEditMode = false;
    private string? message;
    private bool messageIsSuccess;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadPlayers();
    }
    
    private async Task LoadPlayers()
    {
        // Use Database.GetPlayerSummaries() or direct EF Core query
        players = Database.GetPlayerSummaries();
    }
    
    private void EditPlayer(int id)
    {
        var player = Db.Players.Find(id);
        if (player != null)
        {
            currentPlayer = player;
            isEditMode = true;
        }
    }
    
    private async Task SavePlayer()
    {
        Database.SavePlayer(currentPlayer);
        message = isEditMode ? "Player updated." : "Player added.";
        messageIsSuccess = true;
        CancelEdit();
        await LoadPlayers();
    }
    
    private async Task DeletePlayer(int id)
    {
        Database.DeletePlayer(id);
        message = "Player removed from roster.";
        messageIsSuccess = true;
        await LoadPlayers();
    }
    
    private void CancelEdit()
    {
        currentPlayer = new Player();
        isEditMode = false;
    }
}
```

## YARP Routing Update
```json
"players-page": {
  "ClusterId": "local-blazor",
  "Match": { "Path": "/Players.aspx" },
  "Order": 3
}
```

## Navigation Update
Update `Shared/NavMenu.razor`:
```razor
<NavLink href="players" Match="NavLinkMatch.All">
    <span class="oi oi-people" aria-hidden="true"></span> Players
</NavLink>
```

## Done When
- [ ] Players.razor displays player list with summaries (name, jersey, position, stats)
- [ ] Add player form works (name, jersey, position dropdown)
- [ ] Edit player loads existing data into form
- [ ] Update player saves changes
- [ ] Delete player removes from roster
- [ ] Cancel button resets form to add mode
- [ ] Validation enforces required fields and valid jersey number
- [ ] Success/error messages display correctly
- [ ] Empty state shows when no players exist
- [ ] NavMenu includes Players link
- [ ] Both /Players and /Players.aspx routes work
- [ ] Page builds and runs without errors
