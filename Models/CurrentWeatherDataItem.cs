using WhatToWearCalculateApi.Utilities;

namespace WhatToWearCalculateApi.Models;
public class CurrentWeatherDataItem
{
    public DateTime? weatherTime { get; set; }
    public string? weatherTimeFormatted { get; set; }
    public int? temp { get; set; }
    public string? tempFormatted { get; set; }
    public int? dewPoint { get; set; }
    public int? feelsLike { get; set; }
    public string? feelsLikeFormatted { get; set; }
    public int? windSpeed { get; set; }
    public string? iconId { get; set; }
    public string? main { get; set; }
    public DateTime? sunriseTime { get; set; }
    public string? sunriseTimeFormatted { get; set; }
    public DateTime? sunsetTime { get; set; }
    public string? sunsetTimeFormatted { get; set; }
    public TimeSpan? dayLengthTimeSpan { get; set; }
    public string? dayLengthFormatted { get; set; }
    public int? timeOfDay { get; set; }
    public TimeOfDay TimeOfDay
    {
        get
        {
            return GetTimeOfDay();
        }
    }

    private TimeOfDay GetTimeOfDay()
    {
        if (this.weatherTime == null || this.sunriseTime == null || this.sunsetTime == null)
        {
            return TimeOfDay.Unknown;
        }

        if (Calculations.isDawn(this.weatherTime.Value, this.sunriseTime.Value))
        {
            return TimeOfDay.Day;
        }
        else if (Calculations.isDusk(this.weatherTime.Value, this.sunsetTime.Value))
        {
            return TimeOfDay.Dusk;
        }
        else if (Calculations.isDay(this.weatherTime.Value, this.sunriseTime.Value, this.sunsetTime.Value))
        {
            return TimeOfDay.Day;
        }
        else if (Calculations.isNight(this.weatherTime.Value, this.sunsetTime.Value))
        {
            return TimeOfDay.Night;
        }
        else
        {
            return TimeOfDay.Unknown;
        }
    }
}