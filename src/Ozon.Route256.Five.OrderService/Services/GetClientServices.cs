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
            .Customers.Select(x => new ClientResponse()
            {
                Id = x.Id,
                LastName = x.LastName,
                FirstName = x.FirstName,
                Email = x.Email,
                Telephone = x.MobileNumber,
                DefaultAdresses = x.Addresses.Select(x => new AdressResponse()
                {
                    City = x.City,
                    Region = x.Region,
                    Apartment = x.Apartment,
                    Building = x.Building,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Street = x.Street
                })
            .ToList(),
                DefaultAdress = new AdressResponse()
                {
                    City = x.DefaultAddress.City,
                    Region = x.DefaultAddress.Region,
                    Apartment = x.DefaultAddress.Apartment,
                    Building = x.DefaultAddress.Building,
                    Latitude = x.DefaultAddress.Latitude,
                    Longitude = x.DefaultAddress.Longitude,
                    Street = x.DefaultAddress.Street
                }
            })
            .ToList();

        return result;
    }

    public async Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token)
    {
        var result = (await _client.GetCustomerAsync(
            new Customers.GetCustomerByIdRequest { Id = customerId }, cancellationToken: token));

        return new ClientResponse()
        {
            Id = result.Id,
            LastName = result.LastName,
            FirstName = result.FirstName,
            Email = result.Email,
            Telephone = result.MobileNumber,
            DefaultAdresses = result.Addresses.Select(x => new AdressResponse()
            {
                City = x.City,
                Region = x.Region,
                Apartment = x.Apartment,
                Building = x.Building,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Street = x.Street
            })
            .ToList(),
            DefaultAdress = new AdressResponse()
            {
                City = result.DefaultAddress.City,
                Region = result.DefaultAddress.Region,
                Apartment = result.DefaultAddress.Apartment,
                Building = result.DefaultAddress.Building,
                Latitude = result.DefaultAddress.Latitude,
                Longitude = result.DefaultAddress.Longitude,
                Street = result.DefaultAddress.Street
            }
        };
    }
}
