
using Microsoft.Data.Sqlite;
using TodoApi.Models;

namespace TodoApi.Services;

internal class DataService(IConfiguration configuration) : IDataService
{
    private readonly string _connectionString = configuration.GetConnectionString("TodoDbConnection")!;
    public async Task<Todo> AddTodo(Todo todo)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @" INSERT INTO Todo (Title, DueBy, IsComplete) VALUES ($title, $dueBy, $isComplete) RETURNING Id;";
        command.Parameters.AddWithValue("$title", todo.Title);
        command.Parameters.AddWithValue("$dueBy", todo.DueBy);
        command.Parameters.AddWithValue("$isComplete", todo.IsComplete);

        var id = (long)(await command.ExecuteScalarAsync())!;
        return new Todo(id!, todo.Title, todo.DueBy, todo.IsComplete);
    }

    public async Task<bool> DeleteTodoById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @" DELETE FROM Todo WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<Todo?> GetTodoById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @" SELECT * FROM Todo WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (reader.Read())
        {
            return new Todo(id, reader.GetString(1), DateOnly.FromDateTime(reader.GetDateTime(2)), reader.GetBoolean(3));
        }

        return null;
    }

    public async Task<Todo[]?> GetTodos()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @" SELECT * FROM Todo";

        using var reader = await command.ExecuteReaderAsync();
        var todos = new List<Todo>();
        while (reader.Read())
        {
            todos.Add(new Todo(reader.GetInt32(0), reader.GetString(1), DateOnly.FromDateTime(reader.GetDateTime(2)), reader.GetBoolean(3)));
        }

        return [.. todos];
    }

    public async Task<Todo?> UpdateTodoById(int id, Todo todo)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @" UPDATE Todo SET Title = $title, DueBy = $dueBy, IsComplete = $isComplete WHERE id = $id";
        command.Parameters.AddWithValue("$title", todo.Title);
        command.Parameters.AddWithValue("$dueBy", todo.DueBy);
        command.Parameters.AddWithValue("$isComplete", todo.IsComplete);
        command.Parameters.AddWithValue("$id", id);

        return await command.ExecuteNonQueryAsync() > 0
            ? new Todo(id, todo.Title, todo.DueBy, todo.IsComplete)
            : null;
    }
}