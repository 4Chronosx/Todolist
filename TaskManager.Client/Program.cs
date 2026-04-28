using TaskManager.Client.Components;
using TaskManager.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var todoApiBaseUrl = builder.Configuration.GetValue<string>("TodoApi:BaseUrl");
if (string.IsNullOrWhiteSpace(todoApiBaseUrl))
{
    todoApiBaseUrl = "http://localhost:5193/";
}

builder.Services.AddScoped(sp => 
    new HttpClient
    {
        BaseAddress = new Uri(todoApiBaseUrl)
    });

builder.Services.AddScoped<TaskApiService>();

// 👇 Add these two lines
builder.Services.AddHttpClient();
builder.Services.AddHostedService<KeepAliveService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();