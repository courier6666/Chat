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
                                             SELECT * FROM messages ORDER BY TimeUtc;
                                             """;
    
    private const string GetPagedMessagesFromBeforeSqlFormat = """
                                                         SELECT * FROM
                                                                      (SELECT * FROM messages
                                                                      WHERE TimeUtc < '{0}'
                                                                      ORDER BY TimeUtc DESC
                                                                      LIMIT {1})
                                                                  ORDER BY TimeUtc;
                                                         """;
    
    private string connectionString;

    private string GetPagedMessagesBeforeSql(DateTime timeUtc, int size)
    {
        return string.Format(GetPagedMessagesFromBeforeSqlFormat, timeUtc.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), size);
    }

    public SQLiteMessageService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public async Task<int> AddMessageAsync(BasicMessage<string> message)
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

    public async Task<ICollection<BasicMessage<string>>> GetAllMessagesAsync()
    {
        using var connection = CreateConnection();
        connection.Open();
        
        try
        {
            return (await connection.QueryAsync<BasicMessage<string>>(GetAllMessagesSql)).ToList();
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task<ICollection<BasicMessage<string>>> GetPagedMessagedFromBefore(DateTime before, int size)
    {
        using var connection = CreateConnection();
        connection.Open();

        try
        {
            return (await connection.QueryAsync<BasicMessage<string>>(GetPagedMessagesBeforeSql(before, size))).ToList();
        }
        finally
        {
            connection.Close();
        }
    }
}