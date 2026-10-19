
using library.Models;
using Npgsql;

namespace library.Data;

public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    private static Note ReadNote(NpgsqlDataReader reader)
    {
        return new Note
        {
            NoteId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Body = reader.IsDBNull(2) ? null : reader.GetString(2),
            Date = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
        };
    }

    public async Task<List<Note>> GetAllAsync()
    {
        const string sql =
            "SELECT note_id, title, body, created_on " +
            "FROM lending.note ORDER BY title;";

        var notes = new List<Note>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(ReadNote(reader));
        }

        return notes;
    }

    public async Task<Note?> GetByIdAsync(long id)
    {
        const string sql =
            "SELECT note_id, title, body, created_on " +
            "FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? ReadNote(reader) : null;
    }

    public async Task AddAsync(Note note)
    {
        const string sql =
            "INSERT INTO lending.note (title, body, created_on) " +
            "VALUES (@title, @body, @created_on);";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "created_on", (object?)note.Date ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Note note)
    {
        const string sql =
            "UPDATE lending.note " +
            "SET title = @title, body = @body, created_on = @created_on " +
            "WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "created_on", (object?)note.Date ?? DBNull.Value);
        command.Parameters.AddWithValue("id", note.NoteId);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        Console.WriteLine($"Rows updated: {rowsAffected}");
    }

    public async Task DeleteAsync(long id)
    {
        const string sql =
            "DELETE FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}
