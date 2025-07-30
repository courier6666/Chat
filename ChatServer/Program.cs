using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chat.ChatServer;
using Chat.TcpServer.Core;
using Chat.DatabaseAccess.SQLite;
using Microsoft.Data.Sqlite;

using var conn = new SqliteConnection("Data Source=Chat.db");
conn.Open();
var command = conn.CreateCommand();
command.CommandText = """
                      CREATE TABLE IF NOT EXISTS messages(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Data VARCHAR(4096) NOT NULL,
                        AuthorId CHAR(36) NOT NULL,
                        TimeUtc DATETIME NOT NULL);
                      """;
command.ExecuteNonQuery();

var serializationOptions = new JsonSerializerOptions()
{
    WriteIndented = false,
    AllowTrailingCommas = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
};

TcpServerCore tcpServerCore = new TcpServerCore(IPAddress.Loopback, 7070, new SQLiteMessageService("Data Source=Chat.db"), serializationOptions);
ChatHttpServer httpServer = new ChatHttpServer("http://localhost:7071/", serializationOptions);


Task.WaitAll([ httpServer.StartAsync(), tcpServerCore.RunAsync()]);
