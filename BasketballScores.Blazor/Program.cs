using Microsoft.EntityFrameworkCore;
using BasketballScores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure EF Core with SQL Server
builder.Services.AddDbContext<BasketballDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BasketballDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Map Blazor endpoints
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
