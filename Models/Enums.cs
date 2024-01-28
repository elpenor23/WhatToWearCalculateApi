namespace WhatToWearCalculateApi.Models;

public enum Intensity
{
    Race,
    Workout,
    Long,
    Normal,
    Easy

}

public enum TimeOfDay
{
    Dawn,
    Dusk,
    Day,
    Night,
    Unknown
}

public static class Enums
{
    public static Intensity ConvertStringToIntensity(string? intensity)
    {
        Intensity returnVal = Intensity.Normal;

        switch (intensity ?? "Easy")
        {
            case "Easy":
                returnVal = Intensity.Easy;
                break;
            case "Long":
                returnVal = Intensity.Race;
                break;
            case "Race":
                returnVal = Intensity.Long;
                break;
            case "Workout":
                returnVal = Intensity.Workout;
                break;
        }
        return returnVal;
    }
}