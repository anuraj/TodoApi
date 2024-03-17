using Microsoft.AspNetCore.Mvc;
using TodoApi;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddTransient<IDataService, DataService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

DatabaseInit.Initialize(app.Configuration.GetConnectionString("TodoDbConnection")!);

var todosApi = app.MapGroup("/todos");

todosApi.MapGet("/", async ([FromServices] IDataService dataService) => await dataService.GetTodos() is { } todos
    ? Results.Ok(todos)
    : Results.NotFound());

todosApi.MapGet("/{id}",async (int id, [FromServices] IDataService dataService) =>
    await dataService.GetTodoById(id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

todosApi.MapPost("/", async (Todo todo, [FromServices] IDataService dataService) =>
{
    var newTodo = await dataService.AddTodo(todo);
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

todosApi.MapPut("/{id}", async (int id, Todo todo, [FromServices] IDataService dataService) =>
{
    var updatedTodo = await dataService.UpdateTodoById(id, todo);
    return updatedTodo is not null
        ? Results.Ok(updatedTodo)
        : Results.NotFound();
});

todosApi.MapDelete("/{id}", async (int id, [FromServices] IDataService dataService) =>
{
    var deleted = await dataService.DeleteTodoById(id);
    return deleted
        ? Results.NoContent()
        : Results.NotFound();
});

app.Run();
