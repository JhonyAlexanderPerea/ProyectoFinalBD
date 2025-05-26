using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.Model;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using System.Linq;

namespace ProyectoFinalBD.View;

public partial class Entities : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<Maintenance> _mantenimientos = new();
    
    private ObservableCollection<Maintenance> _tipoEquipos = new();

    public ObservableCollection<Maintenance> Mantenimientos
    {
        get => _mantenimientos;
        set
        {
            if (_mantenimientos != value)
            {
                _mantenimientos = value;
                OnPropertyChanged();
            }
        }
    }
    public ObservableCollection<Maintenance> TipoEquipos
    {
        get => _tipoEquipos;
        set
        {
            if (_tipoEquipos != value)
            {
                _tipoEquipos = value;
                OnPropertyChanged();
            }
        }
    }

    public Entities()
    {
        InitializeComponent();
        DataContext = this;
        
        
        // Carga inicial
        Loaded += async (s, e) => await CargarMantenimientosAsync();
    }

private async void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    if (MainTabControl?.SelectedItem is not TabItem selectedTab) return;
    
    try 
    {
        switch (selectedTab.Header?.ToString())
        {
            case "Mantenimiento":
                await CargarMantenimientosAsync();
                break;
            case "Tipo de Equipo":
                // Por ahora, no lanzar excepción
                Console.WriteLine("Carga de tipos de equipo no implementada aún");
                break;
            case "Ubicación":
                // Por ahora, no lanzar excepción
                Console.WriteLine("Carga de ubicaciones no implementada aún");
                break;
            default:
                Console.WriteLine($"Tab no manejada: {selectedTab.Header}");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al cambiar de pestaña: {ex.Message}");
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var msg = MessageBoxManager.GetMessageBoxStandardWindow(
                "Error",
                $"Error al cargar los datos: {ex.Message}");
            await msg.Show();
        });
    }
}

private async Task CargarTiposEquipoAsync()
{
    // Por ahora solo registramos que no está implementado
    Console.WriteLine("CargarTiposEquipoAsync no implementado");
    await Task.CompletedTask;
}

private async Task CargarUbicacionesAsync()
{
    // Por ahora solo registramos que no está implementado
    Console.WriteLine("CargarUbicacionesAsync no implementado");
    await Task.CompletedTask;
}

private async Task CargarMantenimientosAsync()
{
    try
    {
        Console.WriteLine("Iniciando carga de mantenimientos...");
        
        var controller = new MaintenanceController();
        var lista = await controller.ObtenerMantenimientos();
        
        if (lista == null || lista.Count == 0)
        {
            Console.WriteLine("No se recuperaron datos de la base de datos.");
        }
        else
        {
            Console.WriteLine($"Datos recuperados: {lista.Count} registros");
            foreach (var item in lista)
            {
                Console.WriteLine($"ID: {item.MaintenanceId}, Fecha: {item.Date}, Costo: {item.Cost}");
            }
        }

        // Crear y asignar ObservableCollection
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Mantenimientos = new ObservableCollection<Maintenance>(lista);
        });

        Console.WriteLine($"Se asignaron {Mantenimientos.Count} registros al ObservableCollection.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al cargar mantenimientos: {ex.Message}");
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var msg = MessageBoxManager.GetMessageBoxStandardWindow(
                "Error",
                $"Error al cargar los datos: {ex.Message}");
            await msg.Show();
        });
    }
}
    

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}