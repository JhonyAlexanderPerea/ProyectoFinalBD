using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Threading;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.Controllers;
using MessageBox.Avalonia;
using ProyectoFinalBD.DAO;

namespace ProyectoFinalBD.View
{
  public partial class MaintenanceView : UserControl, INotifyPropertyChanged
{
    private ObservableCollection<Maintenance> _mantenimientos;
    private Maintenance _selectedMaintenance;
    private readonly MaintenanceController _controller;

    public event PropertyChangedEventHandler? PropertyChanged;

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

    public Maintenance SelectedMaintenance
    {
        get => _selectedMaintenance;
        set
        {
            if (_selectedMaintenance != value)
            {
                _selectedMaintenance = value;
                OnPropertyChanged();
            }
        }
    }

    public MaintenanceView()
    {
        InitializeComponent();
        _controller = new MaintenanceController();
        _mantenimientos = new ObservableCollection<Maintenance>();
        DataContext = this;

        // Asegurarse de que la carga inicial se realice después de que el control esté listo
        this.Loaded += async (s, e) => await CargarDatos();
    }

    private async Task CargarDatos()
    {
        try
        {
            // Usar Dispatcher para asegurar que las actualizaciones de UI se realicen en el hilo correcto
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var mantenimientos = await _controller.ObtenerMantenimientos();
                
                // Limpiar la colección existente
                Mantenimientos.Clear();
                
                // Agregar los nuevos items uno por uno
                foreach (var mantenimiento in mantenimientos)
                {
                    Mantenimientos.Add(mantenimiento);
                }

                // Debug: Verificar que los datos se han cargado
                Console.WriteLine($"Datos cargados en UI: {Mantenimientos.Count} registros");
            });
        }
        catch (Exception ex)
        {
            await ShowError("Error al cargar datos", ex.Message);
        }
    }


private async void OnNewMaintenance(object sender, RoutedEventArgs e)
{
    var newMaintenance = new Maintenance
    {
        MaintenanceId = "MANT" + DateTime.Now.Ticks.ToString().Substring(0, 8),
        Date = DateTime.Now,
        Findings = "",
        Cost = 0,
        EquipmentId = ""
    };

    Mantenimientos.Add(newMaintenance);
    SelectedMaintenance = newMaintenance;

    // Opción: Llamar a un método para guardar en la BD
    try
    {
        //await _controller.AgregarMantenimiento(SelectedMaintenance);
        await ShowMessage("Éxito", "Nuevo mantenimiento creado.");
    }
    catch (Exception ex)
    {
        await ShowError("Error al crear mantenimiento", ex.Message);
    }
}

        private async void OnSaveMaintenance(object sender, RoutedEventArgs e)
        {
            if (SelectedMaintenance == null) return;

            try
            {
                //await _controller.ActualizarMantenimiento(SelectedMaintenance);
                await ShowMessage("Éxito", "Mantenimiento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                await ShowError("Error al actualizar", ex.Message);
            }
        }

        private async void OnDeleteMaintenance(object sender, RoutedEventArgs e)
        {
            if (SelectedMaintenance == null) return;

            var result = await ShowConfirmation("Confirmar eliminación", 
                "¿Está seguro de eliminar este mantenimiento?");

            if (result)
            {
                try
                {
                   // await _controller.EliminarMantenimiento(SelectedMaintenance.MaintenanceId);
                    //Mantenimientos.Remove(SelectedMaintenance);
                    await ShowMessage("Éxito", "Mantenimiento eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    await ShowError("Error al eliminar", ex.Message);
                }
            }
        }

        private void OnMaintenanceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid && grid.SelectedItem is Maintenance maintenance)
            {
                SelectedMaintenance = maintenance;
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // Implementar búsqueda
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            // Implementar filtrado
        }

        private async Task ShowError(string title, string message)
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandardWindow(title, message);
            await messageBox.Show();
        }

        private async Task ShowMessage(string title, string message)
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandardWindow(title, message);
            await messageBox.Show();
        }

        private async Task<bool> ShowConfirmation(string title, string message)
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandardWindow(title, message);
            var result = await messageBox.Show();
            return result == MessageBox.Avalonia.Enums.ButtonResult.Ok;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async Task AgregarMantenimiento(Maintenance maintenance)
        {
            var repository = new MaintenanceRepository();
            await repository.Create(maintenance);
        }

        public async Task ActualizarMantenimiento(Maintenance maintenance)
        {
            var repository = new MaintenanceRepository();
            await repository.Update(maintenance);
        }

        public async Task EliminarMantenimiento(string maintenanceId)
        {
            var repository = new MaintenanceRepository();
            await repository.Delete(maintenanceId);
        }

        
    }
  
}