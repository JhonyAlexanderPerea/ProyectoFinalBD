using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProyectoFinalBD.Controllers;

namespace ProyectoFinalBD.View;

public partial class Entities : UserControl
{
    public ObservableCollection<Maintenance> Mantenimientos { get; set; }

    public Entities()
    {
        Mantenimientos = new ObservableCollection<Maintenance>();
        
        this.DataContext = this;

        _ = CargarMantenimientos();
        
        InitializeComponent();

    }

    private async Task CargarMantenimientos()
    {
        var controller = new MaintenanceController();
        var lista = await controller.ObtenerMantenimientos();
        Console.WriteLine($"Se cargaron {lista.Count} mantenimientos."); // <-- aquí
        foreach (var m in lista)
            Mantenimientos.Add(m);
    }

}