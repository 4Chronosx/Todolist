using TaskManager.Shared;
using System.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TaskTracker"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? ["https://localhost:7001"];

builder.Services.AddCors(options =>
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
    ));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => {
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi(config => {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.UseCors("BlazorClient");

app.MapGet("/health", () => Results.Ok());

app.MapGet("/todoitems", async(TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/completed", async(TodoDb db) =>
    await db.Todos.Where(t => t.IsCompleted).ToListAsync());

app.MapGet("/todoitems/{id}", async(int id, TodoDb db) => {
    var todo = await db.Todos.FindAsync(id);
    return todo == null ? Results.NotFound() : Results.Ok(todo);
});

app.MapPost("/todoitems", async(Todo todo, TodoDb db) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapDelete("/todoitems/{id}", async(int id, TodoDb db) => {
    if (await db.Todos.FindAsync(id) is Todo todo) {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    } 

    return Results.NotFound();
});

app.MapPut("/todoitems/{id}", async(int id, Todo inputTodo, TodoDb db) => {
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();

    todo.Title = inputTodo.Title;
    todo.Description = inputTodo.Description;
    todo.IsCompleted = inputTodo.IsCompleted;
    todo.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();