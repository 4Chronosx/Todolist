
namespace TaskManager.Client.Services;
using System.Net;
using System.Net.Http.Headers;
using TaskManager.Shared;

public class TaskApiService(HttpClient http)
{
    public async Task<List<Todo>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "todoitems");
        request.Headers.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true,
            MustRevalidate = true
        };
        request.Headers.Pragma.ParseAdd("no-cache");

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
        catch when (!cancellationToken.IsCancellationRequested)
        {
            // Server is unreachable (connection refused, DNS failure, etc.) — fire wake ping and rethrow.
            _ = TryWakeBackendAsync(CancellationToken.None);
            throw;
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                await TryWakeBackendAsync(cancellationToken);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Todo>>(cancellationToken: cancellationToken);
        }
    }

    private async Task TryWakeBackendAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var wakeRequest = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            wakeRequest.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };
            wakeRequest.Headers.Pragma.ParseAdd("no-cache");

            using var _ = await http.SendAsync(
                wakeRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
        catch
        {
            // Best-effort wake-up ping. Caller handles user-facing retries.
        }
    }

    public Task<Todo?> GetByIdAsync(int id) =>
        http.GetFromJsonAsync<Todo>($"todoitems/{id}");

    public async Task<Todo?> CreateAsync(Todo todo)
    {
        var response = await http.PostAsJsonAsync("todoitems", todo);
        return await response.Content.ReadFromJsonAsync<Todo>();
    }

    public async Task<Todo?> UpdateAsync(Todo todo)
    {
        int id = todo.Id;
        var response = await http.PutAsJsonAsync($"todoitems/{id}", todo);

        response.EnsureSuccessStatusCode();

        if (response.StatusCode == HttpStatusCode.NoContent ||
            response.Content.Headers.ContentLength == 0)
        {
            return todo;
        }

        return await response.Content.ReadFromJsonAsync<Todo>();
    } 

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"todoitems/{id}");
        response.EnsureSuccessStatusCode();
    }
}