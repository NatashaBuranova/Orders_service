using Microsoft.AspNetCore.Mvc;
using Moq;
using Ozon.Route256.Five.OrderService.Controllers;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IClientRepository> _clientRepositoryMock = new();
    private readonly Mock<ICancelOrderServices> _canceledOrderServicesMock = new();
    private readonly Mock<IRegionRepository> _regionRepositoryMock = new();

    private readonly OrdersController _ordersController;

    public OrdersControllerTests()
    {
        _ordersController = new OrdersController(
            _orderRepositoryMock.Object,
            _canceledOrderServicesMock.Object,
            _regionRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task CancelOrderByIdAsync_WhenOrderNotFound_ReturnsNotFound()
    {
        // Arrange
        const int NOT_EXIST_ID = 99;
        var token = CancellationToken.None;
        _orderRepositoryMock.Setup(x => x.FindAsync(NOT_EXIST_ID, token)).ReturnsAsync((Order)null);

        // Act
        var result = await _ordersController.CancelOrderByIdAsync(NOT_EXIST_ID, token);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Order with id: {NOT_EXIST_ID} not found", ((NotFoundObjectResult)result).Value);
    }

    [Fact]
    public async Task CancelOrderByIdAsync_WhenOrderStateIsForbiddenToCancel_ReturnsBadRequest()
    {
        // Arrange
        const int CANCELED_ORDER_ID = 1;
        var token = CancellationToken.None;
        var order = new Order { Id = CANCELED_ORDER_ID, State = OrderState.Cancelled };
        _orderRepositoryMock.Setup(x => x.FindAsync(CANCELED_ORDER_ID, token)).ReturnsAsync(order);

        // Act
        var result = await _ordersController.CancelOrderByIdAsync(CANCELED_ORDER_ID, token);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Cannot cancel order with id:{CANCELED_ORDER_ID} in state {order.State}",
                    ((BadRequestObjectResult)result).Value);
    }

    [Fact]
    public async Task CancelOrderByIdAsync_WhenOrderIsCanceled_ReturnsOk()
    {
        // Arrange
        const int ORDER_ID = 1;
        var token = CancellationToken.None;
        var order = new Order { Id = ORDER_ID, State = OrderState.SentToCustomer };
        _orderRepositoryMock.Setup(x => x.FindAsync(ORDER_ID, token)).ReturnsAsync(order);

        // Act
        var result = await _ordersController.CancelOrderByIdAsync(ORDER_ID, token);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(OrderState.Cancelled, order.State);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(order, token), Times.Once);
        _canceledOrderServicesMock.Verify(x => x.CancelOrderInLogisticsSimulator(ORDER_ID, token), Times.Once);
    }

    [Fact]
    public async Task GetOrderStatusByIdAsync_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        const int ORDER_ID = 1;
        _orderRepositoryMock.Setup(x => x.FindAsync(ORDER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        // Act
        var result = await _ordersController.GetOrderStatusByIdAsync(ORDER_ID, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Order with id: {ORDER_ID} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetOrderStatusByIdAsync_ExistOrder_ReturnsOkWithState()
    {
        // Arrange
        const int ORDER_ID = 1;
        var order = new Order { Id = ORDER_ID, State = OrderState.Created };
        _orderRepositoryMock.Setup(x => x.FindAsync(ORDER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _ordersController.GetOrderStatusByIdAsync(ORDER_ID, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order.State, okResult.Value);
    }

    [Fact]
    public async Task GetOrdersListWithFiltersAsync_WithNonExistingRegion_ReturnsNotFound()
    {
        // Arrange
        var request = new OrdersListWithFiltersRequest(new List<long> { 1, 2, 3 }, true, null, 10);
        var token = new CancellationToken();

        _regionRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), token))
            .ReturnsAsync(false);

        // Act
        var result = await _ordersController.GetOrdersListWithFiltersAsync(request, token);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Region with id: 1 not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetOrdersListWithFiltersAsync_WithFilters_ReturnsCorrectResponse()
    {
        // Arrange
        var request = new OrdersListWithFiltersRequest(new List<long> { 1 }, true, null, 10);
        var token = new CancellationToken();

        _regionRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), token))
            .ReturnsAsync(true);

        _clientRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), token)).ReturnsAsync(true);

        _orderRepositoryMock.Setup(x => x.GetOrdersListWithFiltersByPageAsync(It.IsAny<OrdersListWithFiltersRequest>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order[]
            {
                new Order
                {
                    Id = 1,
                    State = OrderState.Created,
                    DateCreate = DateTime.Now,
                    CountProduct = 2,
                    TotalSumm = 100,
                    TotalWeight = 2,
                    ClientId = 1,
                    Client= new Models.Client()
                    {
                        Id=1,
                        FirstName = "John",
                        LastName = "Doe"
                    }
                }
            });

        // Act
        var result = await _ordersController.GetOrdersListWithFiltersAsync(request, token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var orders = Assert.IsType<List<OrderResponse>>(okResult.Value);
        var order = Assert.Single(orders);

        Assert.Equal(1, order.Id);
        Assert.Equal(OrderState.Created, order.Status);
        Assert.Equal(2, order.CountProduct);
        Assert.Equal(100, order.TotalSumm);
        Assert.Equal(2, order.TotalWeight);
        Assert.Equal("John", order.Client?.FirstName);
        Assert.Equal("Doe", order.Client?.LastName);
    }

    [Fact]
    public async Task GetOrdersListByRegionsAndDateTimeAsync_WithNonExistingRegion_ReturnsNotFound()
    {
        // Arrange
        var request = new OrdersListByRegionsAndDateTimeRequest(DateTimeOffset.Now.AddMonths(-2), new List<long> { 1, 2, 3 });
        var token = new CancellationToken();

        _regionRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _ordersController.GetOrdersListByRegionsAndDateTimeAsync(request, token);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Region with id: 1 not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetOrdersListByRegionsAndDateTimeAsync_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new OrdersListByRegionsAndDateTimeRequest(DateTime.Now.AddDays(-1), new List<long> { 1, 2, 3 });
        var token = new CancellationToken();

        _regionRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _orderRepositoryMock.Setup(x => x.GetOrdersListByRegionsAndDateTimeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<List<long>>(), token))
            .ReturnsAsync(new Order[]
            {
                new Order
                {
                    Id = 1,
                    State = OrderState.Created,
                    TotalSumm = 100,
                    TotalWeight = 10,
                    Region = new Region
                        {
                            Id = 1,
                            Name = "Region 1"
                        },
                    DeliveryAddress= new Models.Address()
                    {
                        Region="Region 1"
                    },
                    RegionId = 1,
                    ClientId = 2,
                },
                new Order
                {
                    Id = 2,
                    State = OrderState.Created,
                    TotalSumm = 200,
                    TotalWeight = 20,
                    Region = new Region
                        {
                            Id = 2,
                            Name = "Region 2"
                        },
                    DeliveryAddress= new Models.Address()
                    {
                        Region="Region 2"
                    },
                    RegionId = 2,
                    ClientId = 4
                }
            });

        // Act
        var result = await _ordersController.GetOrdersListByRegionsAndDateTimeAsync(request, token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ordersInRegions = Assert.IsType<List<OrdersInRegionResponse>>(okResult.Value);
        Assert.Equal(2, ordersInRegions.Count);
        Assert.Equal("Region 1", ordersInRegions[0].RegionName);
        Assert.Equal(1, ordersInRegions[0].CountOrders);
        Assert.Equal(1, ordersInRegions[0].CountClients);
        Assert.Equal(100, ordersInRegions[0].TotalSumOrders);
        Assert.Equal(10, ordersInRegions[0].TotalWeight);
        Assert.Equal("Region 2", ordersInRegions[1].RegionName);
        Assert.Equal(1, ordersInRegions[1].CountOrders);
        Assert.Equal(1, ordersInRegions[1].CountClients);
        Assert.Equal(200, ordersInRegions[1].TotalSumOrders);
        Assert.Equal(20, ordersInRegions[1].TotalWeight);
    }

    [Fact]
    public async Task GetOrdersForClientByTimeAsync_ReturnsNotFound_WhenClientNotFound()
    {
        // Arrange
        var request = new OrdersForClientByTimeRequest(1, DateTimeOffset.UtcNow, 10);
        var token = CancellationToken.None;

        _clientRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), token)).ReturnsAsync(false);

        // Act
        var result = await _ordersController.GetOrdersForClientByTimeAsync(request, token);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Customer with id: {request.ClientId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetOrdersForClientByTimeAsync_ReturnsOk_WithOrders()
    {
        // Arrange
        var request = new OrdersForClientByTimeRequest(1, DateTimeOffset.UtcNow, 10);
        var token = CancellationToken.None;
        var client = new Models.Client { Telephone = "1234567890", FirstName = "John", LastName = "Doe" };
        var orders = new[]
        {
            new Order { Id = 1, State = OrderState.Delivered, DateCreate = DateTime.Now, CountProduct = 2, TotalSumm = 10, TotalWeight = 1, ClientId = 1, Client = client },
            new Order { Id = 2, State = OrderState.SentToCustomer, DateCreate = DateTime.Now.AddDays(-1), CountProduct = 1, TotalSumm = 5, ClientId = 1, Client = client }
        };

        _orderRepositoryMock.Setup(x => x.GetOrdersForClientByTimePerPageAsync(It.IsAny<OrdersForClientByTimeRequest>(), token)).ReturnsAsync(orders);
        _clientRepositoryMock.Setup(x => x.IsExistsAsync(It.IsAny<long>(), token)).ReturnsAsync(true);

        // Act
        var result = await _ordersController.GetOrdersForClientByTimeAsync(request, token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<OrderResponse>>(okResult.Value);
        Assert.Equal(orders.Length, response.Count);
        for (int i = 0; i < orders.Length; i++)
        {
            Assert.Equal(orders[i].Id, response[i].Id);
            Assert.Equal(orders[i].State, response[i].Status);
            Assert.Equal(orders[i].DateCreate, response[i].DateCreate);
            Assert.Equal(orders[i].CountProduct, response[i].CountProduct);
            Assert.Equal(client.Telephone, response[i].Telephone);
            Assert.Equal(orders[i].TotalSumm, response[i].TotalSumm);
            Assert.Equal(orders[i].TotalWeight, response[i].TotalWeight);
            Assert.Equal(orders[i].DeliveryAddress, response[i].DeliveryAddress);
            Assert.Equal(client.FirstName, response[i].Client?.FirstName);
            Assert.Equal(client.LastName, response[i].Client?.LastName);
        }
    }
}
