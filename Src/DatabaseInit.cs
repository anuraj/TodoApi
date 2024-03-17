using Microsoft.Data.Sqlite;

namespace TodoApi;

public static class DatabaseInit
{
    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Todo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                DueBy TEXT NOT NULL,
                IsComplete INTEGER NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    }
}