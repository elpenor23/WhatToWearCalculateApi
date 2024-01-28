using System.Net.Http.Headers;

namespace WhatToWearCalculateApi.Utilities;
public class ApiAccess
{

    public static async Task<string?> GetApiJsonData(string uri, IHttpClientFactory httpClientFactory)
    {
        var result = "[{}]";

        using (var client = httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                var errorMessage = string.Format($"Error Getting data from {uri}. Status Code: {response.StatusCode}");
                throw new Exception(errorMessage);
            }
        }

        return result;
    }
}
