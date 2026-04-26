using TaskManager.Client.Components;
using TaskManager.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var todoApiBaseUrl = builder.Configuration.GetValue<string>("TodoApi:BaseUrl");
if (string.IsNullOrWhiteSpace(todoApiBaseUrl))
{
    throw new InvalidOperationException("Missing configuration value for TodoApi:BaseUrl.");
}

builder.Services.AddScoped(sp => 
    new HttpClient
    {
        BaseAddress = new Uri(todoApiBaseUrl)
    });

builder.Services.AddScoped<TaskApiService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
