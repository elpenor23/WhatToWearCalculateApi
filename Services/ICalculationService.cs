using WhatToWearCalculateApi.Models;

namespace WhatToWearCalculateApi.Services;

public interface ICalculationService
{
    Task<List<IntensityClothing>> CalculateClothingAsync(double lat, double lon, int tempAdjust);
}
