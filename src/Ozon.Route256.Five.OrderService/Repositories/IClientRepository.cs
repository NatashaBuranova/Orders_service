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
    public Task InsertAsync(Models.Client newClient, CancellationToken token);
}
