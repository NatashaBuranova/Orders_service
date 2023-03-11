using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Services;

public interface IGetClientServices
{
    Task<List<ClientResponse>> GetClientsAsync(CancellationToken token);
    Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token);
}
