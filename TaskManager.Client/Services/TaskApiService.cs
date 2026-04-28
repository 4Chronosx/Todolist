
namespace TaskManager.Client.Services;
using System.Net;
using System.Net.Http.Headers;
using TaskManager.Shared;

public class TaskApiService(HttpClient http)
{
    // Fire-and-forget: keeps Render's connection alive during cold start (up to 120s)
    public async Task WakeUpAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            await http.GetAsync("health", cts.Token);
        }
        catch { }
    }

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

        var response = await http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        using (response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Todo>>(cancellationToken: cancellationToken);
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