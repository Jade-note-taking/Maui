using System.Collections.ObjectModel;
using System.Diagnostics;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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
        foreach (var note in AllNotes)
        {
            connection.On<string, string?>($"Note.Update.{note.cosmosId}", (noteName, noteLocation) =>
            {
                UpdateNote(note.id, noteName, noteLocation);
            });
        }

        connection.On<Note>("Note.Create", (note) =>
        {
            AllNotes.Add(note);
            connection.On<string, string?>($"Note.Update.{note.cosmosId}", (noteName, noteLocation) => UpdateNote(note.id, noteName, noteLocation));
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

    public void DeleteNote(object? sender, EventArgs eventArgs)
    {
        if (sender is not Note note) return;

        Debug.WriteLine($"DELETE {note.name}");

        // Delete the note
    }

    public void ArchiveNote(object? sender, EventArgs eventArgs)
    {
        if (sender is not Note note) return;

        Debug.WriteLine($"ARCHIVE {note.name}");

        // Archive the note
    }
}