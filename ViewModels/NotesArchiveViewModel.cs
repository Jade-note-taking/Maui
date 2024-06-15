using System.Collections.ObjectModel;
using System.Diagnostics;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

public partial class NotesArchiveViewModel : ObservableObject
{
    private readonly SignalRService _signalRService = ServiceHelper.GetService<SignalRService>();
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();

    private ObservableCollection<Note>? notes;
    public ObservableCollection<Note>? Notes
    {
        get => notes;
        set => SetProperty(ref notes, value);
    }

    public async Task OnAppearing()
    {
        Notes = new ObservableCollection<Note>(await _noteService.GetArchiveNotes());
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

        connection.On<Note>("Note.Archive", (note) =>
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
        });

        connection.On<Note>("Note.Inbox", (Note note) =>
        {
            var indexNotes = Notes.ToList().FindIndex(n => n.id == note.id);
            if (indexNotes != -1) Notes.RemoveAt(indexNotes);
        });
    }

    private void UpdateNote(string noteId, string noteName, string? noteLocation)
    {
        var index = Notes.ToList().FindIndex(n => n.id == noteId);
        if (index == -1) return;
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
    private async Task InboxNote(Note note)
    {
        Debug.WriteLine("InboxNote command is hit");
        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("Inbox", args: new object?[] {note.id});
        Notes.Remove(note);
    }
}