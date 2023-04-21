using static Dapper.SqlMapper;
using System.Data;
using System.Text.Json;
using Ozon.Route256.Five.OrderService.Core.Models;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection;

class AddressObjectHandler : TypeHandler<Address>
{
    private AddressObjectHandler() { }
    public static AddressObjectHandler Instance { get; } = new AddressObjectHandler();

    public override Address Parse(object value)
    {
        var json = (string)value;
        return json == null ? null : JsonSerializer.Deserialize<Address>(json);
    }

    public override void SetValue(IDbDataParameter parameter, Address value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}