using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProyectoFinalBD.View;

public partial class Register : Window
{
    public Register()
    {
        InitializeComponent();
        
    }

    private void Exit(object? sender, RoutedEventArgs e)
    {
        Login login = new Login();
        login.Show();
        this.Close();
        
    }

    private void RegisterUser(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}