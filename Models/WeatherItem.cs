namespace WhatToWearCalculateApi.Models;

public class WeatherItem
{
    public string? id { get; set; }
    public decimal? lat { get; set; }
    public decimal? lon { get; set; }
    public DateTime? weatherTime { get; set; }
    public string? weatherTimeFormatted { get; set; }
    public MoonWeatherDataItem? moon { get; set; }
    public CurrentWeatherDataItem? current { get; set; }
    public DayWeatherDataItem? today { get; set; }
    public DayWeatherDataItem? tomorrow { get; set; }
}
