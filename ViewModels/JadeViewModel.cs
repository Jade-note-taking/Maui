using System.Diagnostics;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

[QueryProperty(nameof(JadeViewModel), "Note")]
public partial class JadeViewModel : ObservableObject
{
    private readonly CosmosService _cosmosService = ServiceHelper.GetService<CosmosService>();
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();
    private readonly DebounceService _debounceService = ServiceHelper.GetService<DebounceService>();

    [ObservableProperty] private Note? note = null;

    [ObservableProperty] private string? content = null;

    [ObservableProperty] private bool loading = false;

    [ObservableProperty] private Timer? _noteUpdateTimer;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Loading = true;
        if (query.TryGetValue("Note", out var value))
        {
            Note = value as Note;
            StartContentSignalR();
        }

        Loading = false;
    }

    private async void StartContentSignalR()
    {
        var connection = await _cosmosService.GetConnection();
        connection.On<string>($"Note.UpdateContent.{Note.cosmosId}", (s) => Content = s);
        Content = await _noteService.GetNote(Note.id);
    }

    public void NoteContentUpdate() => _debounceService.Debounce(500, async () =>
    {
        var connection = await _cosmosService.GetConnection();
        await connection.InvokeCoreAsync("UpdateContent", args: new object?[] { Note.cosmosId, Content });
    });

    public void NoteUpdate() => _debounceService.Debounce(500, async () =>
    {
        var connection = await _cosmosService.GetConnection();
        await connection.InvokeCoreAsync("Update", args: new object?[] { Note.id, Note.name, Note.location });
    });

    [RelayCommand]
    private async Task NotesPage()
    {
        await Shell.Current.GoToAsync(Routes.NotesPage);
    }
}