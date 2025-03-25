using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ProyectoFinalBD.View;

namespace ProyectoFinalBD;

public partial class MenuPrincipal : Window
{
    public MenuPrincipal()
    {
        InitializeComponent();
    }
    private void OpenViewEntidades(object? sender, RoutedEventArgs e)
    {
        //TODO
    }
    private void OpenViewTransacciones(object? sender, RoutedEventArgs e)
    {
        //TODO 
    }

    private void OpernViewReportes(object? sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void OpenViewUtilidades(object? sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void OpenViewAyudas(object? sender, RoutedEventArgs e)
    {
        //TODO
    }

    private void OpenViewLogin(object? sender, RoutedEventArgs e)
    {
        if (ValidarCierre())
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
        else
        {
            //TODO: mostrar en pantalla mensaje de error al tratar de cerrar sesión
        }
    }

    private bool ValidarCierre()
    {
        return true;
    }
}