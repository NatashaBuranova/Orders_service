using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class GetClientServices : IGetClientServices
{
    private readonly Customers.Customers.CustomersClient _client;

    public GetClientServices(Customers.Customers.CustomersClient client)
    {
        _client = client;
    }

    public async Task<List<ClientResponse>> GetClientsAsync(CancellationToken token)
    {
        var result = (await _client.GetCustomersAsync(request: new Empty(), cancellationToken: token))
            .Customers.Select(x => GetClientResponse(x))
            .ToList();

        return result;
    }

    public async Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token)
    {
        var result = (await _client.GetCustomerAsync(
            new Customers.GetCustomerByIdRequest { Id = customerId }, cancellationToken: token));

        return GetClientResponse(result);
    }

    private static ClientResponse GetClientResponse(Customers.Customer customer)
    {
        return new ClientResponse()
        {
            Id = customer.Id,
            LastName = customer.LastName,
            FirstName = customer.FirstName,
            Email = customer.Email,
            Telephone = customer.MobileNumber,
            DefaultAdresses = customer.Addresses.Select(x => GetAdressResponse(x))
            .ToList(),
            DefaultAdress = GetAdressResponse(customer.DefaultAddress)
        };
    }

    private static AdressResponse GetAdressResponse(Customers.Address address)
    {
        return new AdressResponse()
        {
            City = address.City,
            Region = address.Region,
            Apartment = address.Apartment,
            Building = address.Building,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            Street = address.Street
        };
    }
}
