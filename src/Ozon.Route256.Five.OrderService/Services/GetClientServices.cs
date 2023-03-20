using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Distributed;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;
using Ozon.Route256.Five.OrderService.Services;
using System.Text.Json;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class GetClientServices : IGetClientServices
{
    private readonly Customers.Customers.CustomersClient _client;
    private IDistributedCache _distributedCache;
    private const string CLIENTS_CACHE_NAME = "AllClientsInfo";
    private const string CLIENT_CACHE_NAME = "ClientsInfo";

    public GetClientServices(Customers.Customers.CustomersClient client, IDistributedCache distributedCache)
    {
        _client = client;
        _distributedCache = distributedCache;
    }

    public async Task<List<ClientResponse>> GetClientsAsync(CancellationToken token)
    {

        var stringCahce = await _distributedCache.GetStringAsync(CLIENTS_CACHE_NAME, token: token);

        if (stringCahce != null) return JsonSerializer.Deserialize<List<ClientResponse>>(stringCahce);

        var result = (await _client.GetCustomersAsync(request: new Empty(), cancellationToken: token))
            .Customers.Select(x => GetClientResponse(x))
            .ToList();

        await _distributedCache.SetStringAsync(CLIENTS_CACHE_NAME, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        }, token: token);

        return result;
    }

    public async Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token)
    {
        var stringCahce = await _distributedCache.GetStringAsync($"{CLIENT_CACHE_NAME}:{customerId}", token: token);

        if (stringCahce != null) return JsonSerializer.Deserialize<ClientResponse>(stringCahce);

        var result = await _client.GetCustomerAsync(
            new Customers.GetCustomerByIdRequest { Id = customerId }, cancellationToken: token);

        var clientResponse = GetClientResponse(result);

        await _distributedCache.SetStringAsync($"{CLIENT_CACHE_NAME}:{customerId}", JsonSerializer.Serialize(clientResponse), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        }, token: token);

        return clientResponse;
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
            Adresses = customer.Addresses.Select(x => GetAdressResponse(x))
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
