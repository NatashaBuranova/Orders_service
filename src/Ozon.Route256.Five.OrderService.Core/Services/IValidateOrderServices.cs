using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Core.Services;

public interface IValidateOrderServices
{
    public bool IsCanCancelOrder(OrderState state);

    public bool IsOrderValid(Order order);
}
