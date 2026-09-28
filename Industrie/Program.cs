using Industrie.Components;
using Industrie.Data;
using Industrie.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
// Ajouter SignalR
builder.Services.AddSignalR();
// 1. Enregistrement du service de chiffrement
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();


// les services de notification
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<IMachineService, MachineService>();

var hseConnectionString = builder.Configuration.GetConnectionString("HseDbConnection");


// 2. Configuration d'EF Core avec DbContextFactory pour Blazor (Support Supabase PostgreSQL & InMemory fallback)
builder.Services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connString = config.GetConnectionString("DefaultConnection");
    var provider = config["ConnectionStrings:Provider"]?.ToLowerInvariant() ?? "postgresql";

    bool isPostgres = provider == "postgresql" || provider == "supabase" || provider == "npgsql";
    bool hasValidCredentials = !string.IsNullOrWhiteSpace(connString) && !connString.Contains("YOUR_DB_PASSWORD");

    if (isPostgres && hasValidCredentials)
    {
        options.UseNpgsql(connString);
    }
    else if (provider == "sqlserver" && hasValidCredentials)
    {
        options.UseSqlServer(connString);
    }
    else
    {
        // Fallback transparent InMemory pour le développement local
        options.UseInMemoryDatabase("IndustrieDb");
    }
});

// ── Authentification & Authorization ──
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddCascadingAuthenticationState();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("Responsable", policy => policy.RequireAssertion(_ => true));
}
else
{
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("Responsable", policy =>
            policy.RequireRole("MEDIS-NABEUL\\GMEDIS"));
}


// ── Services ──
builder.Services.AddScoped<ActiveDirectoryService>();


var app = builder.Build();
await DbInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();



// ✅ Puis les composants Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
