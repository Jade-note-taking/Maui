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

    public void ApplyQueryAttributes(IDictionary<string, object> query) => ViewModel.ApplyQueryAttributes(query);

    private void NoteContentUpdate(object? sender, EventArgs e) => ViewModel.NoteContentUpdate(); // Note content update
    private void NoteUpdate(object? sender, EventArgs e) => ViewModel.NoteUpdate(); // Any other note update
}
