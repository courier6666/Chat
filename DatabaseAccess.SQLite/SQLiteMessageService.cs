using Chat.DatabaseAccess;
using Chat.Message;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Chat.DatabaseAccess.SQLite;

public class SQLiteMessageService : IMessageService
{
    private const string CreateMessageSql = """
                                            INSERT INTO messages (Data, AuthorId, TimeUtc) VALUES (@Data, @AuthorId, @TimeUtc);
                                            SELECT last_insert_rowid();
                                            """;
    private const string GetAllMessagesSql = """
                                             SELECT * FROM messages;
                                             """;
    private string connectionString;

    public SQLiteMessageService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public async Task<int> AddMessageAsync(Message<string> message)
    {
        using var connection = CreateConnection();
        try
        {
            connection.Open();
            object param = new { Data = message.Data, AuthorId = message.AuthorId, TimeUtc = message.TimeUtc };
            return await connection.ExecuteScalarAsync<int>(CreateMessageSql, param);
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task<ICollection<Message<string>>> GetAllMessagesAsync()
    {
        using var connection = CreateConnection();
        connection.Open();
        
        try
        {
            return (await connection.QueryAsync<Message<string>>(GetAllMessagesSql)).ToList();
        }
        finally
        {
            connection.Close();
        }
    }
}