namespace Ozon.Route256.Five.OrderService.Services;

public interface IGetClientServices
{
    Task<Customers.GetCustomersResponse> GetClientsAsync(CancellationToken token);
    Task<Customers.Customer> GetClientAsync(int customerId, CancellationToken token);
}
