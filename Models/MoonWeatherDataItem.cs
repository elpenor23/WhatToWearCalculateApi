namespace WhatToWearCalculateApi.Models;

public class MoonWeatherDataItem
{
    public int? phaseIndex { get; set; }
    public string? phaseName { get; set; }
    public string? phaseIcon { get; set; }
    public DateTime? moonriseTime { get; set; }
    public string? moonriseTimeFormatted { get; set; }
    public DateTime? moonsetTime { get; set; }
    public string? moonsetTimeFormatted { get; set; }
}
