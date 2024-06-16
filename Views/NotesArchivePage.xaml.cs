using JadeMaui.Helpers;
using JadeMaui.Services;
using JadeMaui.ViewModels;

namespace JadeMaui.Views;

public partial class NotesArchivePage : ContentPage
{
    private readonly INoteService _noteService = ServiceHelper.GetService<INoteService>();
    private NotesArchiveViewModel ViewModel;

    public NotesArchivePage()
    {
        InitializeComponent();

        ViewModel = new NotesArchiveViewModel();
        BindingContext = ViewModel;
    }

    private async void GoBack(object? sender, EventArgs eventArgs)
    {
        var navigationParameter = new Dictionary<string, object?>
        {
            { "Note", null }
        };

        await Shell.Current.GoToAsync("..", navigationParameter);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.OnAppearing();
    }
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await ViewModel.OnDisappearing();
    }
}