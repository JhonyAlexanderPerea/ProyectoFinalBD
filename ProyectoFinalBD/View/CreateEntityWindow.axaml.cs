using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.DAO;
using Location = ProyectoFinalBD.Model.Location;


namespace ProyectoFinalBD.View
{
    public partial class CreateEntityWindow : Window
    {
        private readonly Grid _mainGrid;
        private readonly StackPanel _formPanel;
        private readonly Button _guardarButton;
        private readonly ComboBox _entitySelector;
        private readonly Dictionary<string, List<TextBox>> _entityFields;
        // Declaraciones de los controllers
        private readonly DamageReportController _damageReportController;
        private readonly EquipmentController _equipmentController;
        private readonly EquipmentStatusController _equipmentStatusController;
        private readonly EquipmentTypeController _equipmentTypeController;
        private readonly LoanController _loanController;
        private readonly LocationController _locationController;
        private readonly MaintenanceController _maintenanceController;
        private readonly MunicipalityController _municipalityController;
        private readonly ReturnController _returnController;
        private readonly SupplierController _supplierController;
        private readonly UserController _userController;
        private readonly UserLogController _userLogController;
        
        
        
        public CreateEntityWindow()
        {
            Title = "Crear Entidad";
            Width = 500;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            _damageReportController = new DamageReportController();
            _equipmentController = new EquipmentController();
            _equipmentStatusController = new EquipmentStatusController();
            _equipmentTypeController = new EquipmentTypeController();
            _loanController = new LoanController();
            _locationController = new LocationController();
            _maintenanceController = new MaintenanceController();
            _municipalityController = new MunicipalityController();
            _returnController = new ReturnController();
            _supplierController = new SupplierController();
            _userController = new UserController();
            _userLogController = new UserLogController();


            _entityFields = new Dictionary<string, List<TextBox>>();

            _mainGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                },
                Margin = new Thickness(20)
            };

            _entitySelector = new ComboBox
            {
                ItemsSource = new ObservableCollection<string>
                {
                    "DamageReport",
                    "Equipment",
                    "EquipmentStatus",
                    "EquipmentType",
                    "Loan",
                    "Location",
                    "Maintenance",
                    "Municipality",
                    "Return",
                    "Supplier",
                    "User",
                    "UserLog"
                },
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            _entitySelector.SelectionChanged += OnEntitySelected;

            _formPanel = new StackPanel
            {
                Spacing = 10
            };

            var scrollViewer = new ScrollViewer
            {
                Content = _formPanel
            };

            _guardarButton = new Button
            {
                Content = "Guardar",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            _guardarButton.Click += OnGuardarClick;

            Grid.SetRow(_entitySelector, 0);
            Grid.SetRow(scrollViewer, 1);
            Grid.SetRow(_guardarButton, 2);

            _mainGrid.Children.Add(_entitySelector);
            _mainGrid.Children.Add(scrollViewer);
            _mainGrid.Children.Add(_guardarButton);

            Content = _mainGrid;
        }

        private async void OnEntitySelected(object? sender, SelectionChangedEventArgs e)
        {
            try
            {
                _formPanel.Children.Clear();
                var selectedEntity = _entitySelector.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedEntity)) return;

                var fields = GetEntityFields(selectedEntity);
                var textBoxes = new List<TextBox>();

                foreach (var field in fields)
                {
                    var fieldPanel = new DockPanel();
                    
                    var label = new TextBlock
                    {
                        Text = field.Name,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    DockPanel.SetDock(label, Dock.Left);

                    var textBox = new TextBox
                    {
                        Watermark = $"Ingrese {field.Name}",
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    if (field.Type == typeof(DateTime))
                    {
                        textBox.Watermark = "YYYY-MM-DD";
                    }
                    else if (field.Type == typeof(decimal))
                    {
                        textBox.Watermark = "0.00";
                    }

                    fieldPanel.Children.Add(label);
                    fieldPanel.Children.Add(textBox);
                    _formPanel.Children.Add(fieldPanel);
                    textBoxes.Add(textBox);
                }

                _entityFields[selectedEntity] = textBoxes;
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al cargar los campos: {ex.Message}");
            }
        }

        private async void OnGuardarClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEntity = _entitySelector.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedEntity))
                {
                    await ShowError("Error", "Debe seleccionar un tipo de entidad");
                    return;
                }

                var values = _entityFields[selectedEntity].Select(tb => tb.Text).ToList();
                var fields = GetEntityFields(selectedEntity);

                // Validar campos vacíos
                if (values.Any(string.IsNullOrWhiteSpace))
                {
                    await ShowError("Error", "Todos los campos son obligatorios");
                    return;
                }

                await ValidateFields(selectedEntity, values);
                await CreateEntity(selectedEntity, fields, values);
                Close();
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al crear la entidad: {ex.Message}");
            }
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

            var button = (Button)((StackPanel)dialog.Content).Children[1];
            button.Click += (s, e) => dialog.Close();
            await dialog.ShowDialog(this);
        }

        public void SetEntityType(string entityName)
        {
            _entitySelector.SelectedItem = entityName;
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

            var button = (Button)((StackPanel)dialog.Content).Children[1];
            button.Click += (s, e) => dialog.Close();
            await dialog.ShowDialog(this);
        }


        private async Task ValidateFields(string entityName, List<string> values)
        {
            var fields = GetEntityFields(entityName);
            for (int i = 0; i < fields.Count; i++)
            {
                var (name, type, maxLength) = fields[i];
                var value = values[i];

                // Validar campos NOT NULL
                if (string.IsNullOrWhiteSpace(value))
                {
                    // Excepciones para campos que permiten NULL en la BD
                    var nullableFields = new Dictionary<string, List<string>>
                    {
                        { "DamageReport", new List<string> { "descripcionDanio" } },
                        { "Maintenance", new List<string> { "hallazgosMantenimiento" } },
                        { "Return", new List<string> { "pagoMulta" } },
                        { "Supplier", new List<string> { "municipio" } },
                        { "User", new List<string> { "rolUsuario", "municipio" } },
                        { "Equipment", new List<string> { "tipoEquipo", "ubicacion", "estadoEquipo", "proveedor" } }
                    };

                    if (!(nullableFields.ContainsKey(entityName) && nullableFields[entityName].Contains(name)))
                    {
                        throw new ArgumentException($"El campo {name} es obligatorio");
                    }
                }

                // Validar tipos de datos
                if (type == typeof(DateTime))
                {
                    if (!DateTime.TryParse(value, out _))
                        throw new ArgumentException($"El campo {name} debe ser una fecha válida (YYYY-MM-DD)");
                }
                else if (type == typeof(decimal) || type == typeof(decimal?))
                {
                    if (!decimal.TryParse(value, out _))
                        throw new ArgumentException($"El campo {name} debe ser un número decimal válido");
                }
                else if (type == typeof(int))
                {
                    if (!int.TryParse(value, out _))
                        throw new ArgumentException($"El campo {name} debe ser un número entero válido");
                }

                // Validar longitud máxima
                if (maxLength.HasValue && value != null && value.Length > maxLength.Value)
                {
                    throw new ArgumentException($"El campo {name} no puede exceder {maxLength} caracteres");
                }

                // Validaciones especiales
                if (name.EndsWith("correo") || name.EndsWith("email")) // Para correoUser y correoProveedor
                {
                    if (!value.Contains("@") || !value.Contains("."))
                        throw new ArgumentException($"El campo {name} debe ser un email válido");
                }

                if (name == "garantiaGenMesProveedor" && int.TryParse(value, out int months))
                {
                    if (months < 0)
                        throw new ArgumentException("La garantía no puede ser negativa");
                }
            }
        }

        private List<(string Name, Type Type, int? MaxLength)> GetEntityFields(string entityName)
    {
    return entityName switch
    {
        "DamageReport" => new List<(string, Type, int?)>
        {
            ("codigoDanio", typeof(string), 10),
            ("fechaDanio", typeof(DateTime), null),
            ("causaDanio", typeof(string), 100),
            ("descripcionDanio", typeof(string), null), // CLOB
            ("equipo", typeof(string), 10)
        },
        "Equipment" => new List<(string, Type, int?)>
        {
            ("codigoEquipo", typeof(string), 10),
            ("nombreEquipo", typeof(string), 50),
            ("costoEquipo", typeof(decimal), null),
            ("caracteristicasEquipo", typeof(string), null), // CLOB
            ("tipoEquipo", typeof(string), 10),
            ("ubicacion", typeof(string), 10),
            ("estadoEquipo", typeof(string), 10),
            ("proveedor", typeof(string), 10)
        },
        "EquipmentStatus" => new List<(string, Type, int?)>
        {
            ("codigoEstadoEquipo", typeof(string), 10),
            ("nombreEstadoEquipo", typeof(string), 50)
        },
        "EquipmentType" => new List<(string, Type, int?)>
        {
            ("codigoTipoEquipo", typeof(string), 10),
            ("nombreTipoEquipo", typeof(string), 50),
            ("descripcionTipoEquipo", typeof(string), 250)
        },
        "Loan" => new List<(string, Type, int?)>
        {
            ("codigoPrestamo", typeof(string), 10),
            ("fechaPrestamo", typeof(DateTime), null),
            ("fechaLimitePrestamo", typeof(DateTime), null),
            ("costoMultaPrestamo", typeof(decimal), null),
            ("equipo", typeof(string), 10),
            ("usuario", typeof(string), 10)
        },
        "Location" => new List<(string, Type, int?)>
        {
            ("codigoUbicacion", typeof(string), 10),
            ("lugarUbicacion", typeof(string), 50)
        },
        "Maintenance" => new List<(string, Type, int?)>
        {
            ("codigoMantenimiento", typeof(string), 10),
            ("fechaMantenimiento", typeof(DateTime), null),
            ("hallazgosMantenimiento", typeof(string), null), // CLOB
            ("costoMantenimiento", typeof(decimal), null),
            ("equipo", typeof(string), 10)
        },
        "Municipality" => new List<(string, Type, int?)>
        {
            ("codigoMunicipio", typeof(string), 10),
            ("nombreMunicipio", typeof(string), 50),
            ("descripcionMunicipio", typeof(string), 250)
        },
        "Return" => new List<(string, Type, int?)>
        {
            ("codigoDevolution", typeof(string), 10),
            ("fechaDevolution", typeof(DateTime), null),
            ("observacionesDevolution", typeof(string), null), // CLOB
            ("pagoMulta", typeof(decimal?), null),
            ("prestamo", typeof(string), 10)
        },
        "Supplier" => new List<(string, Type, int?)>
        {
            ("codigoProveedor", typeof(string), 10),
            ("nombreProveedor", typeof(string), 50),
            ("noContactoProveedor", typeof(string), 15),
            ("correoProveedor", typeof(string), 50),
            ("garantiaGenMesProveedor", typeof(int), null),
            ("municipio", typeof(string), 10)
        },
        "User" => new List<(string, Type, int?)>
        {
            ("codigoUser", typeof(string), 10),
            ("nombreUser", typeof(string), 50),
            ("correoUser", typeof(string), 50),
            ("contrasenaUser", typeof(string), 100),
            ("rolUsuario", typeof(string), 10),
            ("municipio", typeof(string), 10)
        },
        "UserLog" => new List<(string, Type, int?)>
        {
            ("codigoLogUser", typeof(string), 10),
            ("fecha", typeof(DateTime), null),
            ("registro", typeof(string), null), // CLOB
            ("usuario", typeof(string), 10)
        },
        "UserRole" => new List<(string, Type, int?)>
        {
            ("codigoRolUsuario", typeof(string), 10),
            ("nombreRolUsuario", typeof(string), 50)
        },
        _ => new List<(string, Type, int?)>()
    };
}

        private async Task CreateEntity(string entityName, List<(string Name, Type Type, int? MaxLength)> fields, List<string> values) {
        // Implementar la lógica de creación según la entidad
        switch (entityName)
        {
            case "DamageReport":
                try
                {
                    if (string.IsNullOrEmpty(values[0]))
                        throw new ArgumentException("El código del reporte de daño no puede estar vacío");

                    if (!DateTime.TryParse(values[1], out DateTime damageDate))
                        throw new ArgumentException("La fecha ingresada no es válida");

                    if (string.IsNullOrEmpty(values[2]))
                        throw new ArgumentException("La causa del daño no puede estar vacía");

                    if (string.IsNullOrEmpty(values[4]))
                        throw new ArgumentException("El código del equipo no puede estar vacío");

                    var damageReport = new DamageReport
                    {
                        DamageReportId = values[0],
                        Date = damageDate,
                        Cause = values[2],
                        Description = values[3],
                        EquipmentId = values[4]
                    };

                    try
                    {
                        await _damageReportController.CreateReport(damageReport);
                        await ShowMessage("Éxito", "El reporte de daño se ha creado correctamente.");
                    }
                    catch (OracleException ex)
                    {
                        await ShowError("Error de Base de Datos",
                            $"Error al crear el reporte de daño en la base de datos: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        await ShowError("Error", $"Error al crear el reporte de daño: {ex.Message}");
                    }
                }
                catch
                {
                }

                break;
            case "Equipment":
                        var equipment = new Equipment
                        {
                            EquipmentId = values[0],
                            Name = values[1],
                            Cost = decimal.Parse(values[2]),
                            Features = values[3],
                            EquipmentTypeId = values[4],
                            LocationId = values[5],
                            EquipmentStatusId = values[6],
                            SupplierId = values[7]
                        };
                        try
                        {
                            await _equipmentController.CreateEquipment(equipment);
                            await ShowMessage("Éxito", "El equipo se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos",
                                $"Error al crear el equipo de daño en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el equipo de daño: {ex.Message}");
                        }
                        
                        break;
            
            case "EquipmentStatus":
                        var equipmentStatus = new EquipmentStatus
                        {
                            EquipmentStatusId = values[0],
                            Name = values[1]
                        };
                        try
                        {
                            await _equipmentStatusController.createEquipmentStatus(equipmentStatus);
                            await ShowMessage("Éxito", "El estado del equipo se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el estado del equipo en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el estado del equipo: {ex.Message}");
                        }

                        break;
            
                    case "EquipmentType":
                        var equipmentType = new EquipmentType
                        {
                            EquipmentTypeId = values[0],
                            Name = values[1],
                            Description = values[2]
                        };
                        try
                        {
                            await _equipmentTypeController.createEquipmentType(equipmentType);
                            await ShowMessage("Éxito", "El tipo de equipo se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el tipo de equipo en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el tipo de equipo: {ex.Message}");
                        }

                        break;
            
                    case "Loan":
                        var loan = new Loan
                        {
                            LoanId = values[0],
                            Date = DateTime.Parse(values[1]),
                            DueDate = DateTime.Parse(values[2]),
                            PenaltyCost = decimal.Parse(values[3]),
                            EquipmentId = values[4],
                            UserId = values[5]
                        };
                        try
                        {
                            await _loanController.createLoan(loan);
                            await ShowMessage("Éxito", "El préstamo se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el préstamo en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el préstamo: {ex.Message}");
                        }

                        break;
            
                    case "Location":
                        var location = new Location
                        {
                            LocationId = values[0],
                            Name = values[1]
                        };
                        try
                        {
                            await _locationController.createLocation(location);
                            await ShowMessage("Éxito", "La ubicación se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear la ubicación en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear la ubicación: {ex.Message}");
                        }

                        break;
            
                    case "Maintenance":
                        var maintenance = new Maintenance
                        {
                            MaintenanceId = values[0],
                            Date = DateTime.Parse(values[1]),
                            Findings = values[2],
                            Cost = decimal.Parse(values[3]),
                            EquipmentId = values[4]
                        };
                        try
                        {
                            await _maintenanceController.crearMantenimiento(maintenance);
                            await ShowMessage("Éxito", "El mantenimiento se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el mantenimiento en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el mantenimiento: {ex.Message}");
                        }

                        break;
            
                    case "Municipality":
                        var municipality = new Municipality
                        {
                            MunicipalityId = values[0],
                            Name = values[1],
                            Description = values[2]
                        };
                        try
                        {
                            await _municipalityController.createMunicipality(municipality);
                            await ShowMessage("Éxito", "El municipio se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el municipio en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el municipio: {ex.Message}");
                        }

                        break;
            
                    case "Return":
                        var return_ = new Return
                        {
                            ReturnId = values[0],
                            Date = DateTime.Parse(values[1]),
                            Notes = values[2],
                            PenaltyPaid = decimal.Parse(values[3]),
                            LoanId = values[4]
                        };
                        try
                        {
                            await _returnController.createReturn(return_);
                            await ShowMessage("Éxito", "La devolución se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear la devolución en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear la devolución: {ex.Message}");
                        }

                        break;
            
                    case "Supplier":
                        var supplier = new Supplier
                        {
                            SupplierId = values[0],
                            Name = values[1],
                            Contact = values[2],
                            Email = values[3],
                            WarrantyMonths = int.Parse(values[4]),
                            MunicipalityId = values[5]
                        };
                        try
                        {
                            await _supplierController.createSupplier(supplier);
                            await ShowMessage("Éxito", "El proveedor se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el proveedor en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el proveedor: {ex.Message}");
                        }

                        break;
            
                    case "User":
                        var user = new User
                        {
                            UserId = values[0],
                            Name = values[1],
                            Email = values[2],
                            Password = values[3],
                            UserRoleId = values[4],
                            MunicipalityId = values[5]
                        };
                        try
                        {
                            await _userController.createUser(user);
                            await ShowMessage("Éxito", "El usuario se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el usuario en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el usuario: {ex.Message}");
                        }

                        break;
            
                    case "UserLog":
                        var userLog = new UserLog
                        {
                            UserLogId = values[0],
                            Date = DateTime.Parse(values[1]),
                            Entry = values[2],
                            UserId = values[3]
                        };
                        try
                        {
                            await _userLogController.createUserLog(userLog);
                            await ShowMessage("Éxito", "El registro de usuario se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el registro de usuario en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            await ShowError("Error", $"Error al crear el registro de usuario: {ex.Message}");
                        }

                        break;
                }

            // Implementar los demás casos...
        }
    }
} 

