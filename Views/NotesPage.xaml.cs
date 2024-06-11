using JadeMaui.Helpers;
using JadeMaui.Models;
using JadeMaui.Services;

namespace JadeMaui.Views;

public partial class NotesPage : ContentPage
{
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();

    public NotesPage()
    {
        InitializeComponent();
    }

    private async void NoteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Note note)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Note", note }
            };

            await Shell.Current.GoToAsync("..", navigationParameter);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var notes = await _noteService.GetNotes();
        NoteList.ItemsSource = notes;
    }
}
