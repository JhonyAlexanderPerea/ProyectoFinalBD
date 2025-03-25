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
    
    private void OpenMenuPrincipal(object? sender, RoutedEventArgs e)
    {
        string userName = Username.Text;
        string password = Password.Text;
        if (validarLogin(userName, password))
        {
            MenuPrincipal menuPrincipal = new MenuPrincipal();
            menuPrincipal.Show();
            this.Close();
        }
        else
        {
            Console.WriteLine("Nombre de usuario o contraseña incorrectas");
        }
    }
    private bool validarLogin(String userName, String password)
    {
        UserDAO userDao = new UserDAO();
       //TOO
       return true;
    }

    private void Salir(object? sender, RoutedEventArgs e)
    {
        System.Environment.Exit(0);
    }
    
}
