using System.Diagnostics;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

public partial class JadeViewModel : ObservableObject
{
    private readonly CosmosService _cosmosService = ServiceHelper.GetService<CosmosService>();
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();
    private readonly DebounceService _debounceService = ServiceHelper.GetService<DebounceService>();

    [ObservableProperty] private Note? _note = null;
    [ObservableProperty] private string? _content = null;
    [ObservableProperty] private Timer? _noteUpdateTimer;
    [ObservableProperty] private List<string> _locations;
    [ObservableProperty] private string _selectedLocation;

    public async void OnLoaded()
    {
        if (Note == null) // magical code, mvvm toolkit is somehow crazy
        {
            Note = new Note();
            Note = null;
        }

        Locations = ["./"];
        var notes =  await _noteService.GetNotes();
        var locationsList = new List<string>();
        locationsList.Add("./");
        foreach (var note in notes)
        {
            if (note.location != null) locationsList.Add(note.location);
        }
        locationsList.Sort();
        Locations = locationsList;

        SelectedLocation = (Note == null ? "./" : Note.location) ?? "./";
    }

    public void ApplyQueryAttributes(IDictionary<string, object?> query)
    {
        query.TryGetValue("Note", out var value);

        if (value == null)
        {
            Content = null;
            Note = null;
            SelectedLocation = "./";
        }
        else
        {
            Note = value as Note;
            SelectedLocation = Note.location;
            StartSignalRConnection();
        }
    }

    private async void StartSignalRConnection()
    {
        if (Note == null) return;
        var connection = await _cosmosService.GetConnection();
        connection.On<string>($"Note.UpdateContent.{Note.cosmosId}", (s) => Content = s);
        Content = await _noteService.GetNote(Note.id);
    }

    public void NoteContentUpdate(TextChangedEventArgs e) => _debounceService.Debounce(500, async () =>
    {
        if (Note == null || e.OldTextValue == e.NewTextValue) return;
        var connection = await _cosmosService.GetConnection();
        await connection.InvokeCoreAsync("UpdateContent", args: new object?[] { Note.cosmosId, Content });
    });

    public void NoteNameUpdate() => _debounceService.Debounce(500, async () =>
    {
        if (Note == null) return;
        var connection = await _cosmosService.GetConnection();
        await connection.InvokeCoreAsync("Update", args: new object?[] { Note.id, Note.name, Note.location });
    });

    public void NoteLocationUpdate() => _debounceService.Debounce(500, async () =>
    {
        if (Note == null) return;
        var connection = await _cosmosService.GetConnection();
        Note.location = SelectedLocation == "./" ? null : SelectedLocation;
        await connection.InvokeCoreAsync("Update", args: new object?[] { Note.id, Note.name, Note.location });
    });

    [RelayCommand]
    private async Task NotesPage()
    {
        await Shell.Current.GoToAsync(Routes.NotesPage);
    }
}