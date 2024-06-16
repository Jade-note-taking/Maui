using System.Diagnostics;
using System.Text.Json;
using IdentityModel.Client;
using JadeMaui.Helpers;
using Microsoft.Extensions.Configuration;

namespace JadeMaui.Services;

public class RestService : IRestService
{
    private readonly HttpClient _client = new();
    private readonly IConfigurationManager _configuration = ServiceHelper.GetService<IConfigurationManager>();
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task<T?> GetItem<T>(string route="")
    {
        var uri = $"{_configuration["Api:BaseUrl"]!}{route}";

        _client.SetBearerToken(await SecureStorage.GetAsync("access_token"));
        var response = await _client.GetAsync(uri);

        if (!response.IsSuccessStatusCode) return default;

        var content = await response.Content.ReadAsStringAsync();
        if (typeof(T) == typeof(string)) return (T)(object)content;

        return JsonSerializer.Deserialize<T>(content, _serializerOptions);
    }

    public async Task<List<T>?> GetItems<T>(string route="")
    {
        var uri = $"{_configuration["Api:BaseUrl"]!}{route}";

        _client.SetBearerToken(await SecureStorage.GetAsync("access_token"));
        var response = await _client.GetAsync(uri);

        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<T>>(content, _serializerOptions);
    }

    // public async Task<List<Note>> GetItems()
    // {
    //     Items = new List<Note>();
    //
    //     Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
    //     try
    //     {
    //         HttpResponseMessage response = await _client.GetAsync(uri);
    //         if (response.IsSuccessStatusCode)
    //         {
    //             string content = await response.Content.ReadAsStringAsync();
    //             Items = JsonSerializer.Deserialize<List<Note>>(content, _serializerOptions);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.WriteLine(@"\tERROR {0}", ex.Message);
    //     }
    //
    //     return Items;
    // }
    //
    // public async Task SaveTodoItemAsync(Note item, bool isNewItem = false)
    // {
    //     Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
    //
    //     try
    //     {
    //         string json = JsonSerializer.Serialize<TodoItem>(item, _serializerOptions);
    //         StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
    //
    //         HttpResponseMessage response = null;
    //         if (isNewItem)
    //             response = await _client.PostAsync(uri, content);
    //         else
    //             response = await _client.PutAsync(uri, content);
    //
    //         if (response.IsSuccessStatusCode)
    //             Debug.WriteLine(@"\tTodoItem successfully saved.");
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.WriteLine(@"\tERROR {0}", ex.Message);
    //     }
    // }
    //
    // public async Task DeleteTodoItemAsync(string id)
    // {
    //     Uri uri = new Uri(string.Format(Constants.RestUrl, id));
    //
    //     try
    //     {
    //         HttpResponseMessage response = await _client.DeleteAsync(uri);
    //         if (response.IsSuccessStatusCode)
    //             Debug.WriteLine(@"\tTodoItem successfully deleted.");
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.WriteLine(@"\tERROR {0}", ex.Message);
    //     }
    // }
}
