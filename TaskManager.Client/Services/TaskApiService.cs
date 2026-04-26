
namespace TaskManager.Client.Services;
using System.Net;
using TaskManager.Shared;

public class TaskApiService(HttpClient http)
{
    public Task<List<Todo>?> GetAllAsync() =>
        http.GetFromJsonAsync<List<Todo>>("todoitems");

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