using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

public partial class JadeViewModel : ObservableObject
{
    // private readonly HubConnection _connection;

    [ObservableProperty]
    private string id;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string content;

    public JadeViewModel()
    {
        // _connection = connection;
        //
        // Task.Run(() =>
        // {
        //     // Connecting to Hub
        //     Dispatcher.Dispatch(async () => await _connection.StartAsync());
        //
        //     // Listening
        //     // _connection.On<string>("Note.Create", (message) =>
        //     // {
        //     //     Message.Text = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()} - there's new message coming!";
        //     // });
        // });
    }

    async void OnAddItemClicked(object sender, EventArgs e)
    {
        // var navigationParameter = new Dictionary<string, object>
        // {
        //     { nameof(TodoItem), new TodoItem { ID = Guid.NewGuid().ToString() } }
        // };
        // await Shell.Current.GoToAsync(nameof(TodoItemPage), navigationParameter);
    }

    // private async void  CreateNewNote(object sender, EventArgs e)
    // {
    //     await _connection.InvokeCoreAsync("Create", args: new object?[] { "my note name", "my start location" });
    // }

    [RelayCommand]
    private async Task NotesPage()
    {
        await Shell.Current.GoToAsync(Routes.NotesPage);
    }
}