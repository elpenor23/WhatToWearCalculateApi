namespace WhatToWearCalculateApi.Models;

public class ClothingItem
{
    public string? Id { get; set; }
    public decimal? MinTemp { get; set; }
    public decimal? MaxTemp { get; set; }
    public string? Title { get; set; }
    public string? Special { get; set; }
    public string? Conditions { get; set; }
}