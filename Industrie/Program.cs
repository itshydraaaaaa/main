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


// 2. Configuration d'EF Core avec DbContextFactory pour Blazor
builder.Services.AddDbContextFactory<ApplicationDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
});

// ── Authentification & Authorization ──
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationBuilder()
     .AddPolicy("Responsable", policy =>
        policy.RequireRole("MEDIS-NABEUL\\GMEDIS"));


// ── Services ──
builder.Services.AddScoped<ActiveDirectoryService>();


var app = builder.Build();

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
