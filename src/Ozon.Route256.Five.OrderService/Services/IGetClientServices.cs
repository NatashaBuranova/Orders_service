using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Services;

public interface IGetClientServices
{
    /// <summary>
    /// Получение всех клиентом из сервиса клиентов
    /// </summary>
    Task<List<ClientResponse>> GetClientsAsync(CancellationToken token);

    /// <summary>
    /// Получения информации о клиента по ИД из сервиса клиентов
    /// </summary>
    Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token);
}
