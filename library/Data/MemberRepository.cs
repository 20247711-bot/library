using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about books lives in this one class.
public class MemberRepository
{

    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Book.
    // The positions 0 to 3 match the column order in both SELECTs above.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),   // NULL becomes null
            MemberType = reader.IsDBNull(3) ? null : reader.GetString(3)
        };
    }

    // READ: every book, sorted by title.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT member_id , full_name , email , member_type  FROM lending.member ORDER BY full_name;";

        var member = new List<Member>();

        // The command borrows a connection; 'await using' gives it back when the method ends.
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())        // once for each row
        {
            member.Add(ReadMember(reader));
        }

        return member;
    }

    

}
