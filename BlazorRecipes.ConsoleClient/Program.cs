using CsvHelper;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorRecipes.Shared.Models;

const string baseUrl = "https://localhost:5026";
const string defaultEmail = "normaluser@example.com";
const string defaultPassword = "testUser123!";

Console.WriteLine("BlazorRecipes.ConsoleClient");

using HttpClient client = new()
{
    BaseAddress = new Uri(baseUrl)
};

string? apiResponse;

apiResponse = await GetTokenAsync(defaultEmail, defaultPassword);
Console.WriteLine(apiResponse);

if (apiResponse == null)
{
    Console.WriteLine("[ERROR] Did not receive token from server");
    return;
}

client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiResponse}");

if (args.Length > 0)
{
    string csvFilePath = args[0]; // assuming the CSV file path is the first argument

    using var reader = new StreamReader(csvFilePath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    csv.Read();
    csv.ReadHeader();

    while (csv.Read())
    {
        var record = csv.GetRecord<dynamic>();
        var jsonRecord = JsonSerializer.Serialize(record);
        Console.WriteLine("Posted record: " + jsonRecord);
        await PostRecordAsync(jsonRecord);
    }
}
else
{
    apiResponse = await GetEntityAllAsync();
    Console.WriteLine(apiResponse);
}

async Task<string?> GetTokenAsync(
    string email,
    string password)
{
    var response = await client.PostAsJsonAsync("/identity/login", new
    {
        email,
        password
    });

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

    return content?.AccessToken;
}

async Task<string> GetEntityAllAsync()
{
    var response = await client.GetStringAsync("/api/reviews");

    return response;
}

async Task PostRecordAsync(string record)
{
    bool succeeded = false;

    while (!succeeded)
    {
        var response = await client.PostAsync("/api/reviews", new StringContent(record, new MediaTypeHeaderValue("application/json")));

        if (response.StatusCode == HttpStatusCode.TooManyRequests && response.Headers.RetryAfter?.Delta != null)
        {
            Console.WriteLine($"[INFO] Rate limited. Retrying after {response.Headers.RetryAfter.Delta.Value}...");

            await Task.Delay(response.Headers.RetryAfter.Delta.Value);
        }
        else if (response.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine("[WARNING] There is already a record with this key value. Skipping...");

            succeeded = true;
        }
        else
        {
            response.EnsureSuccessStatusCode();

            succeeded = true;
        }
    }
}
