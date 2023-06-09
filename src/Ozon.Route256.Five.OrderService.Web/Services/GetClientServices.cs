﻿using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Distributed;
using Ozon.Route256.Five.OrderService.CustomersGrpsServise;
using Ozon.Route256.Five.OrderService.Services;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Clients;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Regions;
using System.Text.Json;
using static Ozon.Route256.Five.OrderService.CustomersGrpsServise.Customers;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class GetClientServices : IGetClientServices
{
    private readonly Customers.CustomersClient _client;
    private IDistributedCache _distributedCache;
    private const string CLIENTS_CACHE_NAME = "AllClientsInfo";
    private const string CLIENT_CACHE_NAME = "ClientsInfo";

    public GetClientServices(CustomersClient client, IDistributedCache distributedCache)
    {
        _client = client;
        _distributedCache = distributedCache;
    }

    public async Task<List<ClientResponse>> GetClientsAsync(CancellationToken token)
    {

        var stringCahce = await _distributedCache.GetStringAsync(CLIENTS_CACHE_NAME, token: token);

        if (!string.IsNullOrEmpty(stringCahce))
            return JsonSerializer.Deserialize<List<ClientResponse>>(stringCahce);

        var result = (await _client.GetCustomersAsync(request: new Empty(), cancellationToken: token))
            .Customers.Select(GetClientResponse)
            .ToList();

        await _distributedCache.SetStringAsync(CLIENTS_CACHE_NAME, JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            },
            token: token);

        return result;
    }

    public async Task<ClientResponse> GetClientAsync(int customerId, CancellationToken token)
    {
        var stringCahce = await _distributedCache.GetStringAsync($"{CLIENT_CACHE_NAME}:{customerId}", token: token);

        if (!string.IsNullOrEmpty(stringCahce))
            return JsonSerializer.Deserialize<ClientResponse>(stringCahce);

        var result = await _client.GetCustomerAsync(
            new GetCustomerByIdRequest { Id = customerId }, cancellationToken: token);

        if (result == null)
            throw new Exception($"Customer with id={customerId} not found");

        var clientResponse = GetClientResponse(result);

        await _distributedCache.SetStringAsync($"{CLIENT_CACHE_NAME}:{customerId}", JsonSerializer.Serialize(clientResponse),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            },
            token: token);

        return clientResponse;
    }

    private static ClientResponse GetClientResponse(Customer customer)
    {
        return new ClientResponse()
        {
            Id = customer.Id,
            LastName = customer.LastName,
            FirstName = customer.FirstName,
            Email = customer.Email,
            Telephone = customer.MobileNumber,
            Addresses = customer.Addresses.Select(GetAddressResponse).ToList(),
            DefaultAddress = GetAddressResponse(customer.DefaultAddress)
        };
    }

    private static AddressResponse GetAddressResponse(Address address)
    {
        return new AddressResponse()
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
