using System.Data;
using Dapper;

namespace Chat.DatabaseAccess.SQLite.TypeHandlers;

internal class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return  Guid.Parse(value.ToString()!);
    }
}