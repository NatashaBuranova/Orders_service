using Ozon.Route256.Five.OrderService.Models;
using static Dapper.SqlMapper;
using System.Data;
using System.Text.Json;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

class AdressObjectHandler : TypeHandler<Adress>
{
    private AdressObjectHandler() { }
    public static AdressObjectHandler Instance { get; } = new AdressObjectHandler();

    public override Adress Parse(object value)
    {
        var json = (string)value;
        return json == null ? null : JsonSerializer.Deserialize<Adress>(json);
    }

    public override void SetValue(IDbDataParameter parameter, Adress value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}