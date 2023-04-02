using Microsoft.AspNetCore.Mvc;
using Moq;
using Ozon.Route256.Five.OrderService.Controllers;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Tests.Controllers;

public class ClientsControllerTests
{
    private readonly Mock<IGetClientServices> _clientServicesMock;
    private readonly ClientsController _controller;

    public ClientsControllerTests()
    {
        _clientServicesMock = new Mock<IGetClientServices>();
        _controller = new ClientsController(_clientServicesMock.Object);
    }

    [Fact]
    public async Task GetClientsListAsync_ReturnsOkResult_WithClientsList()
    {
        // Arrange
        var expectedClients = new List<ClientResponse> {
            new ClientResponse
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test"
            }
        };
        _clientServicesMock.Setup(x => x.GetClientsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedClients);

        // Act
        var result = await _controller.GetClientsListAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualClients = Assert.IsAssignableFrom<List<ClientResponse>>(okResult.Value);
        Assert.Equal(expectedClients, actualClients);
    }
}