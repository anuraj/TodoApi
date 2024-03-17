using TodoApi.Models;

namespace TodoApi.Services;

internal interface IDataService
{
    Task<Todo[]?> GetTodos();
    Task<Todo?> GetTodoById(int id);
    Task<Todo> AddTodo(Todo todo);
    Task<Todo?> UpdateTodoById(int id, Todo todo);
    Task<bool> DeleteTodoById(int id);
}
