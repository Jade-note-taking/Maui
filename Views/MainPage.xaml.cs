using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.Views;

public partial class MainPage : ContentPage
{
    private readonly HubConnection _connection;

    public MainPage(HubConnection connection)
    {
        InitializeComponent();
        _connection = connection;

        Task.Run(() =>
        {
            // Connecting to Hub
            Dispatcher.Dispatch(async () => await _connection.StartAsync());

            // Listening
            _connection.On<string>("Note.Create", (message) =>
            {
                Message.Text = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()} - there's new message coming!";
            });
        });
    }

    async void OnAddItemClicked(object sender, EventArgs e)
    {
        // var navigationParameter = new Dictionary<string, object>
        // {
        //     { nameof(TodoItem), new TodoItem { ID = Guid.NewGuid().ToString() } }
        // };
        // await Shell.Current.GoToAsync(nameof(TodoItemPage), navigationParameter);
    }

    private async void  CreateNewNote(object sender, EventArgs e)
    {
        await _connection.InvokeCoreAsync("Create", args: new object?[] { "my note name", "my start location" });
    }
}
