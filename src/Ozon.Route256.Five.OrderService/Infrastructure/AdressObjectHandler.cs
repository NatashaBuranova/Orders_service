using static Dapper.SqlMapper;
using System.Data;
using System.Text.Json;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

class AddressObjectHandler : TypeHandler<Models.Address>
{
    private AddressObjectHandler() { }
    public static AddressObjectHandler Instance { get; } = new AddressObjectHandler();

    public override Models.Address Parse(object value)
    {
        var json = (string)value;
        return json == null ? null : JsonSerializer.Deserialize<Models.Address>(json);
    }

    public override void SetValue(IDbDataParameter parameter, Models.Address value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}