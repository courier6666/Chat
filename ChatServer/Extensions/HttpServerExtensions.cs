using Chat.ChatServer;
using Chat.DatabaseAccess.SQLite;
using ChatServer.Endpoints;

namespace ChatServer.Extensions;

public static class HttpServerExtensions
{
    public static void AddEndpoints(this ChatHttpServer server)
    {
        var messageService = new SQLiteMessageService("Data Source=Chat.db");
        server.AddEndpoint(new GetAllMessagesEndpoint(messageService));
    }
}