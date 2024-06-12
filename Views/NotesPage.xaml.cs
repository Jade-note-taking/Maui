using JadeMaui.Helpers;
using JadeMaui.Models;
using JadeMaui.Services;
using JadeMaui.ViewModels;

namespace JadeMaui.Views;

public partial class NotesPage : ContentPage
{
    private readonly NoteService _noteService = ServiceHelper.GetService<NoteService>();
    private NotesViewModel ViewModel;

    public NotesPage()
    {
        InitializeComponent();
        ViewModel = new NotesViewModel();
        BindingContext = ViewModel;
    }

    private async void NoteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Note note) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Note", note }
        };

        await Shell.Current.GoToAsync("..", navigationParameter);
    }

    private async void GoBack(object? sender, EventArgs eventArgs)
    {
        var navigationParameter = new Dictionary<string, object?>
        {
            { "Note", null }
        };

        await Shell.Current.GoToAsync("..", navigationParameter);
    }
    private void SearchForNotes(object? sender, EventArgs e) => ViewModel.SearchForNotes(sender, e);
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.OnAppearing();
    }
}