using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;


namespace ProyectoFinalBD.View;

public partial class Login : Window
{
    private readonly UserController _userController;

    public Login()
    {
        InitializeComponent();
        _userController = new UserController();
    }
    
    private async void OpenMain(object? sender, RoutedEventArgs e)
    {
        string userName = Username.Text;
        string password = Password.Text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            await ShowError("Por favor ingrese usuario y contraseña");
            return;
        }

        if (await validarLogin(userName, password))
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
        else
        {
            await ShowError("Usuario o contraseña incorrectos");
        }
    }

    private async Task<bool> validarLogin(string email, string password)
    {
        try
        {
            
            return await _userController.Login(email, password);
        }
        catch (Exception ex)
        {
            await ShowError($"Error al intentar iniciar sesión: {ex.Message}");
            return false;
        }
    }

private async Task ShowError(string message)
{
    Window dialog = new Window();
    var okButton = new Button
    {
        Content = "OK",
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
    };
    okButton.Click += (_, _) => dialog.Close();
    
    dialog.Title = "Error";
    dialog.Width = 300;
    dialog.Height = 150;
    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    dialog.Content = new StackPanel
    {
        Margin = new Thickness(20),
        Spacing = 20,
        Children =
        {
            new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            },
            okButton
        }
    };

    await dialog.ShowDialog(this);
}

    private void Exit(object? sender, RoutedEventArgs e)
    {
        System.Environment.Exit(0);
    }

    private void OpenRegisterView(object? sender, RoutedEventArgs e)
    {
        Register register = new Register();
        register.Show();
        this.Close();
    }
}