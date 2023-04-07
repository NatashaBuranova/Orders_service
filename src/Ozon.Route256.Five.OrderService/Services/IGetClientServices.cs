using Ozon.Route256.Five.OrderService.Web.Api.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Services;

public interface IGetClientServices
{
    /// <summary>
    /// Получение всех клиентов из сервиса клиентов
    /// </summary>
    Task<List<ClientResponse>> GetClientsAsync(CancellationToken token);

    /// <summary>
    /// Получения информации о клиенте по ИД из сервиса клиентов
    /// </summary>
    Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token);
}
