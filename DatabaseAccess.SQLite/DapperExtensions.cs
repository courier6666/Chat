using Chat.DatabaseAccess.SQLite.TypeHandlers;
using Dapper;

namespace Chat.DatabaseAccess.SQLite;

public static class DapperExtensions
{
    public static void DapperStartup()
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
    }
}