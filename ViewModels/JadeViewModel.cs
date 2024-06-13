﻿using System.Diagnostics;
using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace JadeMaui.ViewModels;

public partial class JadeViewModel : ObservableObject
{
    private readonly SignalRService _signalRService = ServiceHelper.GetService<SignalRService>();
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();
    private readonly DebounceService _debounceService = ServiceHelper.GetService<DebounceService>();

    [ObservableProperty] private Note? _note = null;
    [ObservableProperty] private string? _content = null;
    [ObservableProperty] private Timer? _noteUpdateTimer;
    [ObservableProperty] private List<string> _locations;
    [ObservableProperty] private string _selectedLocation;
    [ObservableProperty] private string _noteName;

    public async void OnLoaded()
    {
        if (Note == null) // magical code, mvvm toolkit is somehow crazy. DON'T TOUCH IT
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

        locationsList = locationsList.Distinct().ToList();
        locationsList.Sort();
        Locations = locationsList;

        SelectedLocation = (Note == null ? "./" : Note.location) ?? "./";
    }

    public void ApplyQueryAttributes(IDictionary<string, object?> query)
    {
        query.TryGetValue("Note", out var value);

        if (value == null)
        {
            Note = null;
            NoteName = "";
            Content = null;
            SelectedLocation = "./";
        }
        else
        {
            Note = value as Note;
            SelectedLocation = Note.location;
            NoteName = Note.name;
            StartSignalRConnection();
        }
    }

    private async void StartSignalRConnection()
    {
        if (Note == null) return;
        var connection = await _signalRService.GetConnection();
        connection.On<string>($"Note.UpdateContent.{Note.cosmosId}", (s) => Content = s);
        connection.On<string, string?>($"Note.Update.{Note.cosmosId}", (noteName, noteLocation) =>
        {
            NoteName = Note.name= noteName;
            SelectedLocation = noteLocation ?? "./";
            Note.location = noteLocation;
        });
        Content = await _noteService.GetNote(Note.id);
    }

    public void NoteContentUpdate(TextChangedEventArgs e) => _debounceService.Debounce(500, async () =>
    {
        if (Note == null || e.OldTextValue == e.NewTextValue) return;
        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("UpdateContent", args: new object?[] { Note.id, Content });
    });

    public void NoteNameUpdate() => _debounceService.Debounce(500, async () =>
    {
        if (Note == null) return;
        var connection = await _signalRService.GetConnection();
        Note.name = NoteName;
        await connection.InvokeCoreAsync("Update", args: new object?[] { Note.id, Note.name, Note.location });
    });

    public void NoteLocationUpdate() => _debounceService.Debounce(500, async () =>
    {
        if (Note == null) return;
        var connection = await _signalRService.GetConnection();
        Note.location = SelectedLocation == "./" ? null : SelectedLocation;
        await connection.InvokeCoreAsync("Update", args: new object?[] { Note.id, Note.name, Note.location });
    });

    [RelayCommand]
    private async Task SaveNote()
    {
        if (SelectedLocation == null || NoteName == null) return;
        var noteLocation = SelectedLocation == "./" ? null : SelectedLocation;

        var connection = await _signalRService.GetConnection();
        await connection.InvokeCoreAsync("Create", args: new object?[] {NoteName, noteLocation, Content});
        await NotesPage();
    }

    [RelayCommand]
    private async Task NotesPage()
    {
        await _signalRService.StopConnection();
        await Shell.Current.GoToAsync(Routes.NotesPage);
    }
}