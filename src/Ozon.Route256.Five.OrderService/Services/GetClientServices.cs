using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class GetClientServices : IGetClientServices
{
    private readonly Customers.Customers.CustomersClient _client;

    public GetClientServices(Customers.Customers.CustomersClient client)
    {
        _client = client;
    }

    public async Task<Customers.GetCustomersResponse> GetClientsAsync(CancellationToken token)
    {
        var result = await _client.GetCustomersAsync(request: new Empty(), cancellationToken: token);

        return result;
    }

    public async Task<Customers.Customer> GetClientAsync(int customerId, CancellationToken token)
    {
        var result = await _client.GetCustomerAsync(new Customers.GetCustomerByIdRequest { Id = customerId }, cancellationToken: token);

        return result;
    }
}
