namespace JadeMaui.Services;

public interface IRestService
{
    Task<T?> GetItem<T>(string route);
    Task<List<T>?> GetItems<T>(string route);
    // Task SaveItem<T>(T item);
    // Task UpdateItem<T>(T item);
    // Task DeleteItem(string id);
}
