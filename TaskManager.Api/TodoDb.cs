

using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using TaskManager.Shared;

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) {}
    public DbSet<Todo> Todos {get; set;}
}