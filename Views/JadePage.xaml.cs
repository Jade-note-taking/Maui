using System.Diagnostics;
using JadeMaui.ViewModels;

namespace JadeMaui.Views;

public partial class JadePage : ContentPage, IQueryAttributable
{
    private JadeViewModel ViewModel { get; }

    public JadePage()
    {
        InitializeComponent();
        ViewModel = new JadeViewModel();
        BindingContext = ViewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object?> query) => ViewModel.ApplyQueryAttributes(query);

    private void NoteContentUpdate(object sender, TextChangedEventArgs e) =>
        ViewModel.NoteContentUpdate(e); // Note content update

    private void NoteNameUpdate(object? sender, EventArgs e) => ViewModel.NoteNameUpdate();
    private void NoteLocationUpdate(object? sender, EventArgs e) => ViewModel.NoteLocationUpdate();

    private void OnLoaded(object? sender, EventArgs e) => ViewModel.OnLoaded();
}