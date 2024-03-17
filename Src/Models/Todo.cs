using System.Text.Json.Serialization;
namespace TodoApi.Models;
public record Todo(long Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);


[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
