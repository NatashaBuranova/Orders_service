using Microsoft.AspNetCore.Mvc;
using Moq;
using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Web.Api;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Regions;

namespace Ozon.Route256.Five.OrderService.Tests.Controllers;

public class RegionsControllerTests
{
    private readonly Mock<IRegionRepository> _regionRepositoryMock;
    private readonly RegionsController _controller;

    public RegionsControllerTests()
    {
        _regionRepositoryMock = new Mock<IRegionRepository>();
        _controller = new RegionsController(_regionRepositoryMock.Object);
    }

    [Fact]
    public async Task GetRegionsListAsync_ReturnsOkResult_WithRegionsList()
    {
        // Arrange
        var expectedRegions = new Region[] { new Region { Id = 1, Name = "Region 1" } };
        _regionRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRegions);

        // Act
        var result = await _controller.GetRegionsListAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualRegions = Assert.IsAssignableFrom<IEnumerable<RegionResponse>>(okResult.Value);
        Assert.Equal(expectedRegions.Length, actualRegions.Count());
        Assert.Equal(expectedRegions[0].Id, actualRegions.First().Id);
        Assert.Equal(expectedRegions[0].Name, actualRegions.First().Name);
    }
}