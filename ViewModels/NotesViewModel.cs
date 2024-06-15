using System.Collections.ObjectModel;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly SignalRService _signalRService = ServiceHelper.GetService<SignalRService>();
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();

    [ObservableProperty]
    private List<Note>? _allNotes;

    private ObservableCollection<Note>? notes;
    public ObservableCollection<Note>? Notes
    {
        get => notes;
        set => SetProperty(ref notes, value);
    }

    public async Task OnAppearing()
    {
        Notes = new ObservableCollection<Note>(await _noteService.GetNotes());
        SortNotes();

        await StartSignalRConnection();
    }

    private async Task StartSignalRConnection()
    {
        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("Init", args: new object?[] {});
        foreach (var note in Notes)
        {
            connection.On<string, string?>($"Note.Update.{note.cosmosId}", (noteName, noteLocation) =>
            {
                UpdateNote(note.id, noteName, noteLocation);
            });
        }

        connection.On<Note>("Note.Create", (note) =>
        {
            var indexNotes = Notes.ToList().FindIndex(n => n.id == note.id);
            if (indexNotes != -1) return;

            Notes.Add(note);
            connection.On<string, string?>($"Note.Update.{note.cosmosId}", (noteName, noteLocation) => UpdateNote(note.id, noteName, noteLocation));
        });

        connection.On<string>("Note.Delete", (noteId) =>
        {
            var indexNotes = Notes.ToList().FindIndex(n => n.id == noteId);
            if (indexNotes != -1) Notes.RemoveAt(indexNotes);
            // var indexAllNotes = AllNotes.FindIndex(n => n.id == noteId);
            // AllNotes.RemoveAt(indexAllNotes);
        });

        connection.On<string>("Note.Archive", (noteId) =>
        {
            var indexNotes = Notes.ToList().FindIndex(n => n.id == noteId);
            if (indexNotes != -1) Notes.RemoveAt(indexNotes);
            // var indexAllNotes = AllNotes.FindIndex(n => n.id == noteId);
            // AllNotes.RemoveAt(indexAllNotes);
        });
    }

    private void UpdateNote(string noteId, string noteName, string? noteLocation)
    {
        var index = Notes.ToList().FindIndex(n => n.id == noteId);
        var existingNote = Notes[index];
        var newNote = new Note
        {
            id = existingNote.id,
            userId = existingNote.userId,
            cosmosId = existingNote.cosmosId,
            name = noteName,
            location = noteLocation
        };
        Notes.RemoveAt(index);
        Notes.Add(newNote);
        SortNotes();
    }

    private void SortNotes()
    {
        var sorted = new ObservableCollection<Note>(Notes.OrderBy(n => $"{n.location ?? "."}/{n.name}"));
        Notes = sorted;
    }

    public async Task OnDisappearing()
    {
        await _signalRService.StopConnection();
    }

    public void SearchForNotes(object? sender, EventArgs eventArgs)
    {
        // SearchBar searchBar = (SearchBar)sender;
        // var searchKeyword = searchBar.Text;
        //
        // if (searchKeyword == string.Empty)
        // {
        //     Notes = AllNotes;
        //     return;
        // }
        //
        // Notes = AllNotes.FindAll(n =>
        // {
        //     var keyword = $"{n.location}/{n.name}";
        //     return keyword.Contains(searchKeyword, StringComparison.CurrentCultureIgnoreCase);
        // });
    }

    [RelayCommand]
    private async Task NoteSelected(Note note)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Note", note }
        };

        await Shell.Current.GoToAsync("..", navigationParameter);
    }

    [RelayCommand]
    private async Task DeleteNote(Note note)
    {
        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("Delete", args: new object?[] {note.id});
        Notes.Remove(note);
    }

    [RelayCommand]
    private async Task ArchiveNote(Note note)
    {
        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("Archive", args: new object?[] {note.id});
        Notes.Remove(note);
    }
}