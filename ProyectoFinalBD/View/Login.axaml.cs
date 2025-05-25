using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using ProyectoFinalBD.util;
using ReactiveUI;



namespace ProyectoFinalBD.View;

public partial class Login : Window
{
    private Message _message = new Message();
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
            await ShowError("Error","Por favor ingrese usuario y contraseña");
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
            await ShowError("Error","Usuario o contraseña incorrectos");
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
            await ShowError("Error",$"Error al intentar iniciar sesión: {ex.Message}");
            return false;
        }
    }

    private async Task ShowError(string title, string msg)
    {
        await _message.ShowMessage(this, title, msg);
    }


    private void Exit(object? sender, RoutedEventArgs e)
    {
        _userController.Logout();
        System.Environment.Exit(0);
    }

    private void OpenRegisterView(object? sender, RoutedEventArgs e)
    {
        Register register = new Register();
        register.Show();
        this.Close();
    }
}