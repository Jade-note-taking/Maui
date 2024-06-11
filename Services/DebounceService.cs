namespace JadeMaui.Services;

public class DebounceService
{
    private Timer? _timer;

    public void Debounce(int interval, Func<Task> action)
    {
        _timer?.Dispose();
        _timer = new Timer(async _ =>
        {
            await action();
            _timer?.Dispose();
        }, null, interval, Timeout.Infinite);
    }
}