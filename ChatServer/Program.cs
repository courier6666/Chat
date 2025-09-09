using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chat.ChatServer;
using Chat.TcpServer.Core;
using Chat.DatabaseAccess.SQLite;
using ChatServer;
using ChatServer.Extensions;
using Microsoft.Data.Sqlite;
using TcpServer.Core.Builder;
using Chat.Message;
using System.IO;
using Chat.DatabaseAccess;
using TcpServer.Core.Pipeline.Components;
using TcpServer.Core.MessageComponents;
using Microsoft.Extensions.DependencyInjection;
using TcpServer.Core.MessageComponents.Services.Interfaces;
using TcpServer.Core.MessageComponents.Services;

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
await command.ExecuteNonQueryAsync();

DapperExtensions.DapperStartup();

var builder = TcpServerBuilder.Create();
builder.Port(7070)
    .IpAddress(IPAddress.Loopback)
    .OnClientConnectedMessageSend((services) =>
    {
        var idMessage = MessageFactory.CreateIdentificationMessage();
        var jsonSerializerOptions = services.GetService<JsonSerializerOptions>();
        return idMessage.ToBytes(jsonSerializerOptions);
    })
    .OnServerStart((services) =>
    {
        var broadcastService = services.GetService<IBroadcastService>()!;
        broadcastService.Start();
    })
    .Pipeline(pipeline =>
    {
        pipeline.AddComponent<BytesReadFromStreamComponent>()
            .AddComponent<ErrorCatchMessageComponent>()
            .AddComponent<ParseChatMessageComponent>()
            .AddComponent<AddChatMessageComponent>()
            .AddComponent<NotifyAllChatBroadcastComponent>();
    });

builder.Services.AddScoped<IMessageService, SQLiteMessageService>(_ => new SQLiteMessageService("Data Source=Chat.db"));
builder.Services.AddSingleton(JsonOptions.Instance);
builder.Services.AddSingleton<IBroadcastService, BroadcastService>();

var tcpServer = builder.Build();

ChatHttpServer httpServer = new ChatHttpServer("http://localhost:7071/", JsonOptions.Instance);
httpServer.AddEndpoints();

await Task.WhenAll(httpServer.StartAsync(), tcpServer.RunAsync());
