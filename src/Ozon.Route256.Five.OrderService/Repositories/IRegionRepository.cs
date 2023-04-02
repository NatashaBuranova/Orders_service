﻿using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories;

public interface IRegionRepository
{
    /// <summary>
    ///  Получение всех регионов
    /// </summary>
    Task<Region[]> GetAllAsync(CancellationToken token);

    /// <summary>
    ///  Проверка на существование региона
    /// </summary>
    Task<bool> IsExistsAsync(long regionId, CancellationToken token);
}
