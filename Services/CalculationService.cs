using WhatToWearCalculateApi.Models;
using System.Text.Json;
using WhatToWearCalculateApi.Utilities;

namespace WhatToWearCalculateApi.Services;

public class CalculationService : ICalculationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public CalculationService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<List<IntensityClothing>> CalculateClothingAsync(double lat, double lon, int tempAdjust)
    {
        var weather = await GetWeatherAsync(lat, lon) ?? throw new Exception("Unable to get Weather. Unknown Error.");
        var intensities = _configuration.GetSection("Intensities")?.GetChildren()?.Select(x => x.Value)?.ToList() ?? [];
        var returnData = await CalculateClothing(weather, intensities, tempAdjust);
        return returnData;
    }

    private async Task<List<IntensityClothing>> CalculateClothing(WeatherItem weather, List<string?> intensities, int tempAdjust)
    {
        var calculatedClothing = new List<IntensityClothing>();

        if (weather == null || intensities == null || !intensities.Any()) { return calculatedClothing; }


        foreach (var intense in intensities)
        {
            var intensity = Enums.ConvertStringToIntensity(intense);

            var adjustedTemp = AdjustTemperature(weather, intensity, tempAdjust);

            var clothing = await GetClothingForAdjustedTemp(adjustedTemp, intensity, weather?.current?.main ?? "No Data", weather?.current?.TimeOfDay ?? TimeOfDay.Day);

            calculatedClothing.Add(clothing);
        }

        return calculatedClothing;
    }

    private async Task<IntensityClothing> GetClothingForAdjustedTemp(decimal adjustedTemp, Intensity intensity, string conditions, TimeOfDay timeOfDay)
    {

        var finalList = new IntensityClothing(intensity);

        var bodyParts = await GetBodyParts();

        foreach (var bp in bodyParts)
        {
            //no data then bail
            if (bp == null || bp.Id == null) { continue; }

            var clothingList = GetClothingList(bp.Id).Clothing;
            FindCorrectItemForBodyPart(adjustedTemp, intensity, conditions, timeOfDay, clothingList, ref finalList);
        }

        return finalList;
    }

    private static void FindCorrectItemForBodyPart(decimal adjustedTemp, Intensity intensity, string conditions, TimeOfDay timeOfDay, List<ClothingItem> clothingList, ref IntensityClothing finalList)
    {
        foreach (var item in clothingList)
        {
            //if we don't have the data we need bail
            if (item == null || item.MinTemp == null || item.MaxTemp == null) { continue; }

            if (item.MinTemp.Value <= adjustedTemp && adjustedTemp <= item.MaxTemp.Value)
            {
                var addItem = true;

                //check if we need to eliminate this sucker for any reason
                if (
                    (item.Conditions != null && !item.Conditions.Contains(conditions)) ||
                    (item.Special != null && RemoveForSpecialConditions(item.Special, conditions, timeOfDay, intensity)))
                {
                    addItem = false;
                    continue;
                }

                if (addItem)
                {
                    finalList.Clothes.Add(item);
                    break;
                }

            }
        }
    }

    private static bool RemoveForSpecialConditions(string specialCondition, string conditions, TimeOfDay timeOfDay, Intensity intensity)
    {

        if (specialCondition == "sunny")
        {
            //bail if it is not a sunny day
            if (conditions == null || !conditions.Contains("Clear") || timeOfDay != TimeOfDay.Day)
            {
                return false;
            }
        }

        if (specialCondition == "not_rain")
        {
            //bail if it is raining
            if (conditions == null || conditions.Contains("Rain"))
            {
                return false;
            }
        }

        if (specialCondition == "race")
        {
            //bail if it is not a race
            if (intensity != Intensity.Race)
            {
                return false;
            }
        }
        return false;
    }

    public async Task<List<ClothingList>> GetClothing()
    {
        var fullList = new List<ClothingList>();
        if (fullList.Count == 0)
        {
            fullList = await this.DefaultClothing();
        }

        return fullList;
    }

    private List<BodyPart> DefaultBodyParts()
    {
        var bodyParts = new List<BodyPart>();

        var defaultbodyPartsList = _configuration.GetSection("BodyParts")?.GetChildren()?.Select(x => x.Value)?.ToList();

        if (defaultbodyPartsList == null) { return bodyParts; }

        foreach (var id in defaultbodyPartsList)
        {
            var bp = new BodyPart();
            bp.Id = id;

            bodyParts.Add(bp);
        }

        return bodyParts;
    }
    private async Task<List<ClothingList>> DefaultClothing()
    {
        var bodyParts = await GetBodyParts();
        var fullList = new List<ClothingList>();

        foreach (var id in bodyParts)
        {
            if (id == null || id.Id == null) { continue; }

            var clothingItemList = GetClothingList(id.Id);
            fullList.Add(clothingItemList);
        }

        return fullList;
    }

    private ClothingList GetClothingList(string bodyPart)
    {
        var cl = new ClothingList
        {
            Id = bodyPart
        };

        var key = string.Format("Clothing:{0}", bodyPart);
        var clothingItemSubList = _configuration.GetSection(key)?.GetChildren()?.Select(x => x)?.ToList();

        if (clothingItemSubList == null) { return cl; }

        foreach (var item in clothingItemSubList)
        {
            var ci = new ClothingItem
            {
                Id = item["id"],
                MinTemp = decimal.Parse(item["min_temp"] ?? "0"),
                MaxTemp = decimal.Parse(item["max_temp"] ?? "100"),
                Title = item["title"],
                Special = item["special"]
            };
            cl.Clothing.Add(ci);
        }

        return cl;
    }

    public async Task<IEnumerable<BodyPart>> GetBodyParts()
    {
        var bodyParts = new List<BodyPart>();

        if (bodyParts.Count == 0)
        {
            bodyParts = await Task.Run(() => DefaultBodyParts());
        }

        return bodyParts;
    }
    private decimal AdjustTemperature(WeatherItem weather, Intensity intensity, int tempAdjust)
    {
        decimal intensityAdjustment = IntensityAdjustment(intensity);

        decimal finalAdjustment = weather?.current?.feelsLike ?? 0 +
                                intensityAdjustment +
                                tempAdjust;

        return finalAdjustment;
    }

    private decimal IntensityAdjustment(Intensity intensity)
    {
        decimal intensityAdjustment = 0;

        switch (intensity)
        {
            case Intensity.Race:
                intensityAdjustment = decimal.Parse(_configuration["Adjustments:intensity:race"] ?? "0");
                break;
            case Intensity.Workout:
                intensityAdjustment = decimal.Parse(_configuration["Adjustments:intensity:hard_workout"] ?? "0");
                break;
            case Intensity.Long:
                intensityAdjustment = decimal.Parse(_configuration["Adjustments:intensity:long_run"] ?? "0");
                break;
        }

        return intensityAdjustment;
    }

    public async Task<WeatherItem?> GetWeatherAsync(double lat, double lon)
    {
        var openWeatherApiUri = string.Format("https://localhost:7194/weather?lat={0}&lon={1}", lat, lon);
        var weatherData = await ApiAccess.GetApiJsonData(openWeatherApiUri, _httpClientFactory);

        //TODO: Change this back to calling the api
        //var weatherData = "{\"id\":\"44.1765-70.6825\",\"lat\":44.1765,\"lon\":-70.6825,\"weatherTime\":\"2024-01-28T11:15:46-05:00\",\"weatherTimeFormatted\":\"01/28/2024 11:15:46\",\"moon\":{\"phaseIndex\":5,\"phaseName\":\"Waning Gibbous Moon\",\"phaseIcon\":\"waning-gibbous moon\",\"moonriseTime\":\"2024-01-28T19:46:00-05:00\",\"moonriseTimeFormatted\":\"7:46 PM\",\"moonsetTime\":\"2024-01-28T08:44:00-05:00\",\"moonsetTimeFormatted\":\"8:44 AM\"},\"current\":{\"weatherTime\":\"2024-01-28T11:15:46-05:00\",\"weatherTimeFormatted\":\"01/28/2024 11:15:46\",\"sunriseTime\":\"2024-01-28T07:05:17-05:00\",\"sunriseTimeFormatted\":\"7:05 AM\",\"sunsetTime\":\"2024-01-28T16:45:40-05:00\",\"sunsetTimeFormatted\":\"4:45 PM\",\"dayLengthTimeSpan\":\"09:40:23\",\"dayLengthFormatted\":\"9 hours, 40 minutes\",\"temp\":34,\"tempFormatted\":\"34°\",\"dewPoint\":33,\"feelsLike\":34,\"feelsLikeFormatted\":\"34°\",\"windSpeed\":2,\"iconId\":\"04d\",\"main\":\"Clouds\",\"timeOfDay\":2},\"today\":{\"minTemp\":26,\"minTempFormatted\":\"26°\",\"maxTemp\":34,\"maxTempFormatted\":\"34°\",\"description\":\"overcast clouds\",\"summary\":\"Tomorrow: overcast clouds\\nLow: 26° / High: 34°\"},\"tomorrow\":{\"minTemp\":15,\"minTempFormatted\":\"15°\",\"maxTemp\":33,\"maxTempFormatted\":\"33°\",\"description\":\"snow\",\"summary\":\"Tomorrow: snow\\nLow: 15° / High: 33°\"}}";
        var x = JsonSerializer.Deserialize<WeatherItem?>(weatherData);
        return x;
    }
}