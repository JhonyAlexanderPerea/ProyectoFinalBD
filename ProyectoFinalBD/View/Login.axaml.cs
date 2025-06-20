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
        string userId = UserId.Text;
        string password = Password.Text;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            await ShowError("Error", "Por favor ingrese usuario y contraseña");
            return;
        }

        try
        {
            if (userId == "admin" && password == "122021")
            {
                MainMenu mainMenu = new MainMenu();
                mainMenu.Show();
                mainMenu.setUserId(userId);
                this.Close();
                return; // Salir después de cerrar
            }

            if (await validarLogin(userId, password))
            {
                MainUserView mainUserView = new MainUserView();
                mainUserView.Show();
                mainUserView.setUserId(userId);
                this.Close();
            }
            else
            {
                await ShowError("Error", "Usuario o contraseña incorrectos");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<bool> validarLogin(string cedula, string password)
    {
        try
        {
            
            return await _userController.Login(cedula, password);
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
    
}