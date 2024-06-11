using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using JadeMaui.Models;

namespace JadeMaui.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();

    [ObservableProperty]
    private List<Note>? _notes = null;

    public NotesViewModel()
    {
        Task.Run(async () =>
        {
            _notes = await _noteService.GetNotes();
        });
    }
}