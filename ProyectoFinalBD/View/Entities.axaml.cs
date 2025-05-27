using System;
using Avalonia.Controls;

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using System.Linq;
using System.Reactive;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.Model;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using Location = ProyectoFinalBD.Model.Location;

namespace ProyectoFinalBD.View;

public partial class Entities : UserControl, INotifyPropertyChanged, IReactiveObject
{
    public event PropertyChangedEventHandler? PropertyChanged;


    private ObservableCollection<Maintenance> _mantenimientos = new();
    private ObservableCollection<DamageReport> _damageReports = new();
    private ObservableCollection<Equipment> _equipment = new();
    private ObservableCollection<EquipmentStatus> _equipmentStatus = new();
    private ObservableCollection<EquipmentType> _equipmentType = new();
    private ObservableCollection<Loan> _loans = new();
    private ObservableCollection<Location> _locations = new();
    private ObservableCollection<Municipality> _municipalities = new();
    private ObservableCollection<Return> _returns = new();
    private ObservableCollection<Supplier> _suppliers = new();
    private ObservableCollection<User> _users = new();
    private ObservableCollection<UserLog> _userLogs = new();
    private ObservableCollection<UserRole> _userRoles = new();
    
    private object _selectedItem;
    private IReactiveObject _reactiveObjectImplementation;

    public object SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    
    public ReactiveCommand<string, Unit> CreateCommand { get; }
    public Entities()
    {
        InitializeComponent();
        AddCommand = ReactiveCommand.Create(AddItem);
        EditCommand = ReactiveCommand.Create(EditItem);

    
        CreateCommand = ReactiveCommand.CreateFromTask<string>(CreateEntityAsync);
        DataContext = this;

        Loaded += async (s, e) =>
        {
            try
            {
                await CargarTodosLosDatos();
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al cargar los datos iniciales: {ex.Message}");
            }
        };
    }

    private async Task CargarTodosLosDatos()
    {
        await Task.WhenAll(
            CargarMantenimientosAsync(),
            CargarTiposEquipoAsync(),
            CargarUbicacionesAsync(),
            CargarEstadosEquipoAsync(),
            CargarReportesDaniosAsync(),
            CargarEquiposAsync(),
            CargarProveedoresAsync(),
            CargarUsuariosAsync(),
            CargarMunicipiosAsync(),
            CargarDevolucionesAsync(),
            CargarPrestamosAsync(),
            CargarRegistrosUsuarioAsync()
        );
    }
    
    private async void AddItem()
    {
        // Ejemplo: abrir ventana para agregar un nuevo mantenimiento, etc.
  
       // var crudWindow = new CreateEntityWindow();
        //await crudWindow.ShowDialog(parentWindow);
       // crudWindow.setEntity();
        
        Console.WriteLine("Agregar ítem");
    }
    public async Task create(string entityType)
    {
        var crudWindow = new CreateEntityWindow();
        crudWindow.SetEntityType(entityType);
        await crudWindow.ShowDialog(GetWindow());
    }

    private async Task CreateEntityAsync(string entityType)
    {
        var crudWindow = new CreateEntityWindow();
        crudWindow.SetEntityType(entityType);
    
        // Asume que GetWindow() obtiene la ventana actual de forma segura
        await crudWindow.ShowDialog(GetWindow()); 
    }
    private void EditItem()
    {
        Console.WriteLine("Editar ítem");
    }
    
    
    public async Task DeleteItem(string entityType)
    {
        if (_selectedItem == null)
        {
            await ShowMessage("Error", "Por favor, seleccione un elemento para eliminar.");
            return;
        }

        bool confirmDelete = await ShowConfirmation("Confirmar Eliminación", 
            "¿Está seguro que desea eliminar este elemento? Esta acción no se puede deshacer.");

        if (!confirmDelete) return;

        try
        {
            switch (entityType)
            {
                case "Maintenance":
                    var mantenimiento = (Maintenance)_selectedItem;
                    await new MaintenanceController().EliminarMantenimiento(mantenimiento.MaintenanceId);
                    CargarMantenimientosAsync();
                    break;

                case "EquipmentType":
                    var tipoEquipo = (EquipmentType)_selectedItem;
                    await new EquipmentTypeController().EliminarTipoEquipo(tipoEquipo.EquipmentTypeId);
                    CargarTiposEquipoAsync();
                    break;

                case "Location":
                    var ubicacion = (Location)_selectedItem;
                    await new LocationController().EliminarUbicacion(ubicacion.LocationId);
                    CargarUbicacionesAsync();
                    break;

                case "Equipment":
                    var equipo = (Equipment)_selectedItem;
                    await new EquipmentController().EliminarEquipo(equipo.EquipmentId);
                    CargarEquiposAsync();
                    break;

                case "DamageReport":
                    var reporte = (DamageReport)_selectedItem;
                    await new DamageReportController().EliminarReporteDaño(reporte.DamageReportId);
                    CargarReportesDaniosAsync();
                    break;

                case "Supplier":
                    var proveedor = (Supplier)_selectedItem;
                    await new SupplierController().EliminarProveedor(proveedor.SupplierId);
                    CargarProveedoresAsync();
                    break;

                case "User":
                    var usuario = (User)_selectedItem;
                    await new UserController().EliminarUsuario(usuario.UserId);
                    CargarUsuariosAsync();
                    break;

                case "Municipality":
                    var municipio = (Municipality)_selectedItem;
                    await new MunicipalityController().EliminarMunicipio(municipio.MunicipalityId);
                    CargarMunicipiosAsync();
                    break;

                case "Return":
                    var devolucion = (Return)_selectedItem;
                    await new ReturnController().EliminarDevolucion(devolucion.ReturnId);
                    CargarDevolucionesAsync();
                    break;

                case "Loan":
                    var prestamo = (Loan)_selectedItem;
                    await new LoanController().EliminarPrestamo(prestamo.LoanId);
                    CargarPrestamosAsync();
                    break;

                case "UserLog":
                    var registro = (UserLog)_selectedItem;
                    await new UserLogController().EliminarRegistro(registro.UserLogId);
                    CargarRegistrosUsuarioAsync();
                    break;

                default:
                    await ShowMessage("Error", "Tipo de entidad no soportado.");
                    return;
            }

            await ShowMessage("Éxito", "El elemento ha sido eliminado correctamente.");
            _selectedItem = null;
        }
        catch (Exception ex)
        {
            await ShowError("Error", $"Error al eliminar el elemento: {ex.Message}");
        }
    }

    public ObservableCollection<EquipmentStatus> EquipmentStatus
    {
        get => _equipmentStatus;
        set
        {
            if (_equipmentStatus != value)
            {
                _equipmentStatus = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<DamageReport> DamageReports
    {
        get => _damageReports;
        set
        {
            if (_damageReports != value)
            {
                _damageReports = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Equipment> Equipment
    {
        get => _equipment;
        set
        {
            if (_equipment != value)
            {
                _equipment = value;
                OnPropertyChanged();
            }
        }
    }



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

    public ObservableCollection<EquipmentType> EquipmentType
    {
        get => _equipmentType;
        set
        {
            if (_equipmentType != value)
            {
                _equipmentType = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Loan> Loans
    {
        get => _loans;
        set
        {
            if (_loans != value)
            {
                _loans = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Location> Locations
    {
        get => _locations;
        set
        {
            if (_locations != value)
            {
                _locations = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Municipality> Municipalities
    {
        get => _municipalities;
        set
        {
            if (_municipalities != value)
            {
                _municipalities = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Return> Returns
    {
        get => _returns;
        set
        {
            if (_returns != value)
            {
                _returns = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Supplier> Suppliers
    {
        get => _suppliers;
        set
        {
            if (_suppliers != value)
            {
                _suppliers = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<User> Users
    {
        get => _users;
        set
        {
            if (_users != value)
            {
                _users = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<UserLog> UserLogs
    {
        get => _userLogs;
        set
        {
            if (_userLogs != value)
            {
                _userLogs = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<UserRole> UserRoles
    {
        get => _userRoles;
        set
        {
            if (_userRoles != value)
            {
                _userRoles = value;
                OnPropertyChanged();
            }
        }
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
                    await CargarTiposEquipoAsync();
                    break;
                case "Ubicación":
                    await CargarUbicacionesAsync();
                    break;
                case "Estado de Equipo":  
                    await CargarEstadosEquipoAsync();
                    break;
                case "Daños":
                    await CargarReportesDaniosAsync();
                    break;
                case "Equipos":
                    await CargarEquiposAsync();
                    break;
                case "Proveedores":
                    await CargarProveedoresAsync();
                    break;
                case "Usuarios":
                    await CargarUsuariosAsync();
                    break;
                case "Municipios":
                    await CargarMunicipiosAsync();
                    break;
                case "Devoluciones":
                    await CargarDevolucionesAsync();
                    break;
                case "Préstamos":
                    await CargarPrestamosAsync();
                    break;
                case "Log de Usuarios":
                    await CargarRegistrosUsuarioAsync();
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
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }

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
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }

    }

    private async Task CargarReportesDaniosAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de reportes de daños...");

            var controller = new DamageReportController();
            var lista = (await controller.ObtenerReportesDaños()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron reportes de daños de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.DamageReportId}, " +
                                      $"Fecha: {item.Date}, " +
                                      $"Causa: {item.Cause}");
                }
            }

            // Crear y asignar ObservableCollection
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                DamageReports = new ObservableCollection<DamageReport>(lista);
            });

            Console.WriteLine($"Se asignaron {DamageReports.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Reportes daños: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para EquipmentController
    private async Task CargarEquiposAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de equipos...");

            var controller = new EquipmentController();
            var lista = (await controller.ObtenerEquipos()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron equipos de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.EquipmentId}, " +
                                      $"Nombre: {item.Name}, " +
                                      $"Estado: {item.EquipmentStatusId}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Equipment = new ObservableCollection<Equipment>(lista); });

            Console.WriteLine($"Se asignaron {Equipment.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Equipos: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para EquipmentStatusController
    private async Task CargarEstadosEquipoAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de estados de equipo...");

            var controller = new EquipmentStatusController();
            var lista = (await controller.ObtenerEstadosEquipo()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron estados de equipo de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.EquipmentStatusId}, ");

                }
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                EquipmentStatus = new ObservableCollection<EquipmentStatus>(lista);
            });

            Console.WriteLine($"Se asignaron {EquipmentStatus.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Estados de Equipos: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para EquipmentTypeController
    private async Task CargarTiposEquipoAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de tipos de equipo...");

            var controller = new EquipmentTypeController();
            var lista = (await controller.ObtenerTiposEquipo()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron tipos de equipo de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.EquipmentTypeId}, " +
                                      $"Descripción: {item.Description}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                EquipmentType = new ObservableCollection<EquipmentType>(lista);
            });

            Console.WriteLine($"Se asignaron {EquipmentType.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Tipos de Equipo: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para LoanController
    private async Task CargarPrestamosAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de préstamos...");

            var controller = new LoanController();
            var lista = (await controller.ObtenerPrestamos()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron préstamos de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.LoanId}, " +
                                      $"Fecha: {item.Date}, " +
                                      $"Usuario: {item.UserId}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Loans = new ObservableCollection<Loan>(lista); });

            Console.WriteLine($"Se asignaron {Loans.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Prestamos: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para LocationController
    private async Task CargarUbicacionesAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de ubicaciones...");

            var controller = new LocationController();
            var lista = (await controller.ObtenerUbicaciones()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron ubicaciones de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.LocationId}, " +
                                      $"Nombre: {item.Name}, " +
                                      $"Municipio: {item.LocationId}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Locations = new ObservableCollection<Location>(lista); });

            Console.WriteLine($"Se asignaron {Locations.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Ubicaciones: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para MunicipalityController
    private async Task CargarMunicipiosAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de municipios...");

            var controller = new MunicipalityController();
            var lista = (await controller.ObtenerMunicipios()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron municipios de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.MunicipalityId}, " +
                                      $"Nombre: {item.Name}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Municipalities = new ObservableCollection<Municipality>(lista);
            });

            Console.WriteLine($"Se asignaron {Municipalities.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Municipios: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para ReturnController
    private async Task CargarDevolucionesAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de devoluciones...");

            var controller = new ReturnController();
            var lista = (await controller.ObtenerDevoluciones()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron devoluciones de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.ReturnId}, " +
                                      $"Fecha: {item.Date}, " +
                                      $"Préstamo: {item.LoanId}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Returns = new ObservableCollection<Return>(lista); });

            Console.WriteLine($"Se asignaron {Returns.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Devoluciones: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para SupplierController
    private async Task CargarProveedoresAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de proveedores...");

            var controller = new SupplierController();
            var lista = (await controller.ObtenerProveedores()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron proveedores de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.SupplierId}, " +
                                      $"Nombre: {item.Name}, " +
                                      $"Contacto: {item.Contact}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Suppliers = new ObservableCollection<Supplier>(lista); });

            Console.WriteLine($"Se asignaron {Suppliers.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Proveedores: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para UserController
    private async Task CargarUsuariosAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de usuarios...");

            var controller = new UserController();
            var lista = (await controller.ObtenerUsuarios()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron usuarios de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.UserId}, " +
                                      $"Email: {item.Email}, " +
                                      $"Rol: {item.UserRoleId}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { Users = new ObservableCollection<User>(lista); });

            Console.WriteLine($"Se asignaron {Users.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Usuarios: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    // Para UserLogController
    private async Task CargarRegistrosUsuarioAsync()
    {
        try
        {
            Console.WriteLine("Iniciando carga de registros de usuario...");

            var controller = new UserLogController();
            var lista = (await controller.ObtenerRegistrosUsuario()).ToList();

            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No se recuperaron registros de usuario de la base de datos.");
            }
            else
            {
                Console.WriteLine($"Datos recuperados: {lista.Count} registros");
                foreach (var item in lista)
                {
                    Console.WriteLine($"ID: {item.UserLogId}, " +
                                      $"Usuario: {item.UserId}, " +
                                      $"Fecha: {item.Date}");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => { UserLogs = new ObservableCollection<UserLog>(lista); });

            Console.WriteLine($"Se asignaron {UserLogs.Count} registros al ObservableCollection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar Logs Usuarios: {ex.Message}");
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowError("Error", $"Error al cargar los datos: {ex.Message}");
            });
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task ShowMessage(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new Button
                    {
                        Content = "Aceptar",
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        ((Button)((StackPanel)dialog.Content).Children[1]).Click += (s, e) => dialog.Close();
        await dialog.ShowDialog(GetWindow());
    }

    private async Task ShowError(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(Colors.Red)
                    },
                    new Button
                    {
                        Content = "Aceptar",
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        ((Button)((StackPanel)dialog.Content).Children[1]).Click += (s, e) => dialog.Close();
        await dialog.ShowDialog(GetWindow());
    }

    private async Task<bool> ShowConfirmation(string title, string message)
    {
        var tcs = new TaskCompletionSource<bool>();

        var dialog = new Window
        {
            Title = title,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                        {
                            new Button { Content = "Aceptar" },
                            new Button { Content = "Cancelar" }
                        }
                    }
                }
            },
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var buttonPanel = (StackPanel)((StackPanel)dialog.Content).Children[1];
        var okButton = (Button)buttonPanel.Children[0];
        var cancelButton = (Button)buttonPanel.Children[1];

        okButton.Click += (s, e) =>
        {
            tcs.SetResult(true);
            dialog.Close();
        };

        cancelButton.Click += (s, e) =>
        {
            tcs.SetResult(false);
            dialog.Close();
        };

        await dialog.ShowDialog(GetWindow());
        return await tcs.Task;
    }

    private Window GetWindow() => (Window)this.VisualRoot!;


    public event PropertyChangingEventHandler? PropertyChanging
    {
        add => _reactiveObjectImplementation.PropertyChanging += value;
        remove => _reactiveObjectImplementation.PropertyChanging -= value;
    }

    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        _reactiveObjectImplementation.RaisePropertyChanging(args);
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        _reactiveObjectImplementation.RaisePropertyChanged(args);
    }
}