﻿namespace Ozon.Route256.Five.OrderService.ClientBalancing;

public interface IDbStore
{
    /// <summary>
    ///  Обновление конечных точек
    /// </summary>
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    /// <summary>
    /// Получение следующей конечной точки с помощью перебора по круговому циклу
    /// </summary>
    Task<DbEndpoint> GetNextEndpointAsync();
}
