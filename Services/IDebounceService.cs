namespace JadeMaui.Services;

public interface IDebounceService
{
    void Debounce(int interval, Func<Task> action);
}