using JadeMaui.Helpers;
using JadeMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JadeMaui.Models;

namespace JadeMaui.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();

    [ObservableProperty]
    private List<Note>? _allNotes;

    [ObservableProperty]
    private List<Note>? _notes;

    public NotesViewModel()
    {
        Task.Run(async () =>
        {
            AllNotes = await _noteService.GetNotes();
        });
    }

    public async Task OnAppearing()
    {
        Notes = await _noteService.GetNotes();
    }

    public void SearchForNotes(object? sender, EventArgs eventArgs)
    {
        SearchBar searchBar = (SearchBar)sender;
        var searchKeyword = searchBar.Text.Trim();

        if (searchKeyword == string.Empty)
        {
            Notes = AllNotes;
            return;
        }

        Notes = AllNotes.FindAll(n =>
        {
            var keyword = $"{n.location}/{n.name}";
            return keyword.Contains(searchKeyword, StringComparison.CurrentCultureIgnoreCase);
        });
    }
}