using System;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProyectoFinalBD.DAO;

namespace ProyectoFinalBD.View;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }
    
    private void OpenMain(object? sender, RoutedEventArgs e)
    {
        string userName = Username.Text;
        string password = Password.Text;
        if (validarLogin(userName, password))
        {
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
        else
        {
            Console.WriteLine("Nombre de usuario o contraseña incorrectas");
        }
    }
    private bool validarLogin(String userName, String password)
    {
       
       //TOO
       return true;
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
