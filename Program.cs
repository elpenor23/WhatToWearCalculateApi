using WhatToWearCalculateApi.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICalculationService, CalculationService>();
builder.Services.AddOutputCache();

var app = builder.Build();
app.Services.GetRequiredService<IConfiguration>();
app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseOutputCache();


app.MapGet("/calculate", async (double lat, double lon, IConfiguration configuration, ICalculationService calculationService, int tempAdjust = 0) =>
{
    try
    {
        var data = await calculationService.CalculateClothingAsync(lat, lon, tempAdjust);
        return data is null ? Results.NotFound() : Results.Ok(data);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.CacheOutput(x => x.Expire(TimeSpan.FromMinutes(5)))
.WithName("GetCalculatedClothing")
.WithOpenApi();

app.Run();

