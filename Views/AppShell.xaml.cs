namespace JadeMaui.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(NotesPage), typeof(NotesPage));
        Routing.RegisterRoute(nameof(JadePage), typeof(JadePage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
    }
}
