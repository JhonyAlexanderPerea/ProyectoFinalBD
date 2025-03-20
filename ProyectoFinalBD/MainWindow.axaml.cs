using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ProyectoFinalBD;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void BotonClickeado(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        lblMensaje.Text = "¡Hola mundo!";
    }

    private void Salir(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Adios :)");
        System.Environment.Exit(0);
    }
}