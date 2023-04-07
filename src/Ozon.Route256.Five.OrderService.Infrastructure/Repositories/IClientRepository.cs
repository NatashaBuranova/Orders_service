using Ozon.Route256.Five.OrderService.Core.Models;

namespace Ozon.Route256.Five.OrderService.Repositories;

public interface IClientRepository
{

    /// <summary>
    ///  Проверка на существование клиента
    /// </summary>
    Task<bool> IsExistsAsync(long clientId, CancellationToken token);

    /// <summary>
    ///  Создание клиента
    /// </summary>
    public Task InsertAsync(Client newClient, long regionId, CancellationToken token);
}
