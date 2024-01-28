namespace WhatToWearCalculateApi.Utilities;

public static class Calculations
{
    public static bool isDay(DateTime currentTime, DateTime sunRiseTime, DateTime sunSetTime)
    {
        return sunRiseTime <= currentTime && currentTime <= sunSetTime;
    }

    public static bool isDawn(DateTime currentTime, DateTime sunRiseTime)
    {
        var thirtyMinutesBeforeSunrise = sunRiseTime.AddMinutes(-30);
        var thirtyMinutesAfterSunrise = sunRiseTime.AddMinutes(30);

        return thirtyMinutesBeforeSunrise <= currentTime && currentTime <= thirtyMinutesAfterSunrise;
    }

    public static bool isDusk(DateTime currentTime, DateTime sunSetTime)
    {
        var thirtyMinutesBeforeSunset = sunSetTime.AddMinutes(-30);
        var thirtyMinutesAfterSunset = sunSetTime.AddMinutes(30);

        return (thirtyMinutesBeforeSunset <= currentTime && currentTime <= thirtyMinutesAfterSunset);
    }

    public static bool isNight(DateTime currentTime, DateTime sunSetTime)
    {
        return currentTime >= sunSetTime;
    }
    public static DateTime ConvertEpochTimeToDateTime(Int64 timestamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        return dtDateTime.AddSeconds(timestamp).ToLocalTime();
    }
}
