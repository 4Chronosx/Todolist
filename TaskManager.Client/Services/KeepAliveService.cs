

public class KeepAliveService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KeepAliveService> _logger;
    private readonly string _url;
    private readonly TimeSpan _interval;

    public KeepAliveService(
        IHttpClientFactory httpClientFactory,
        ILogger<KeepAliveService> logger,
        IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _url = config["KeepAlive:Url"] ?? throw new ArgumentNullException("KeepAlive:Url missing in appsettings.json");
        _interval = TimeSpan.FromSeconds(config.GetValue<int>("KeepAlive:IntervalSeconds", 30));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[KeepAlive] Service started. Pinging {Url} every {Seconds}s", _url, _interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            await PingBackendAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task PingBackendAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_url);
            _logger.LogInformation("[KeepAlive] {Time} — Backend responded with {StatusCode}",
                DateTime.UtcNow.ToString("o"), (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError("[KeepAlive] {Time} — Failed to reach backend: {Message}",
                DateTime.UtcNow.ToString("o"), ex.Message);
        }
    }
}