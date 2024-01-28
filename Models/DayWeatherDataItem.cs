namespace WhatToWearCalculateApi.Models;

public class DayWeatherDataItem
{
    public int? minTemp { get; set; }
    public string? minTempFormatted { get; set; }
    public int? maxTemp { get; set; }
    public string? maxTempFormatted { get; set; }
    public string? description { get; set; }
    public string? summary { get; set; }
}
