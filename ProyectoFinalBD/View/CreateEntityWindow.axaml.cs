using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        

        // Validaciones específicas por entidad y campo
        switch (entityName)
        {
            case "Location":
                ValidateUbicacion(name, value, maxLength);
                break;
                
            case "EquipmentStatus":
                ValidateEstadoEquipo(name, value, maxLength);
                break;
                
            case "EquipmentType":
                ValidateTipoEquipo(name, value, maxLength);
                break;
                
            case "Municipality":
                ValidateMunicipio(name, value, maxLength);
                break;
                
            case "Supplier":
                ValidateProveedor(name, value, maxLength);
                break;
                
            case "Equipment":
                ValidateEquipo(name, value, maxLength);
                break;
                
            case "User":
                ValidateUsuario(name, value, maxLength);
                break;
                
            case "DamageReport":
                ValidateDanio(name, value, maxLength);
                break;
                
            case "Maintenance":
                ValidateMantenimiento(name, value, maxLength);
                break;
                
            case "Loan":
                ValidatePrestamo(name, value, maxLength);
                break;
                
            case "Return":
                ValidateDevolucion(name, value, maxLength);
                break;
                
            case "UserLog":
                ValidateLogUsuario(name, value, maxLength);
                break;
                
            default:
                throw new ArgumentException($"Entidad desconocida: {entityName}");
        }
    }
            
}
            private void ValidateDanio(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoDanio":
                ValidateStringField(value, 10, "codigoDanio");
                break;
            case "fechaDanio":
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException("La fecha del daño debe ser una fecha válida");
                break;
            case "causaDanio":
                ValidateStringField(value, 100, "causaDanio");
                break;
            case "equipo":
                ValidateStringField(value, 10, "equipo");
                break;
        }
    }

    private void ValidatePrestamo(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoPrestamo":
                ValidateStringField(value, 10, "codigoPrestamo");
                break;
            case "fechaPrestamo":
                case "fechaLimitePrestamo":
                    if (!DateTime.TryParse(value, out _))
                        throw new ArgumentException($"El campo {name} debe ser una fecha válida");
                    break;
            case "costoMultaPrestamo":
                if (!decimal.TryParse(value, out decimal multa) || multa < 0)
                    throw new ArgumentException("El costo de multa debe ser un número positivo");
                break;
            case "equipo":
            case "usuario":
                ValidateStringField(value, 10, name);
                break;
        }
    }

    private void ValidateDevolucion(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoDevolution":
                ValidateStringField(value, 10, "codigoDevolution");
                break;
            case "fechaDevolution":
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException("La fecha de devolución debe ser válida");
                break;
            case "pagoMulta":
                decimal multa = 0;
                if (!string.IsNullOrEmpty(value) && (!decimal.TryParse(value, out multa) || multa < 0))
                    throw new ArgumentException("El pago de multa debe ser un número positivo");
                break;
            case "prestamo":
                ValidateStringField(value, 10, "prestamo");
                break;
        }
    }

    private void ValidateLogUsuario(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoLogUser":
                ValidateStringField(value, 10, "codigoLogUser");
                break;
            case "fecha":
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException("La fecha del log debe ser válida");
                break;
            case "usuario":
                ValidateStringField(value, 10, "usuario");
                break;
        }
    }

    private void ValidateMunicipio(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoMunicipio":
                ValidateStringField(value, 10, "codigoMunicipio");
                break;
            case "nombreMunicipio":
                ValidateStringField(value, 50, "nombreMunicipio");
                break;
        }
    }

    private void ValidateTipoEquipo(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoTipoEquipo":
                ValidateStringField(value, 10, "codigoTipoEquipo");
                break;
            case "nombreTipoEquipo":
                ValidateStringField(value, 50, "nombreTipoEquipo");
                break;
        }
    }

    private void ValidateRolUsuario(string name, string value, int? maxLength)
    {
        switch (name)
        {
            case "codigoRolUsuario":
                ValidateStringField(value, 10, "codigoRolUsuario");
                break;
            case "nombreRolUsuario":
                ValidateStringField(value, 50, "nombreRolUsuario");
                break;
        }
    }

            private bool IsNullableField(string entityName, string fieldName)
            {
                return entityName switch
                {
                    "Danio" => fieldName == "descripcionDanio",
                    "Mantenimiento" => fieldName == "hallazgosMantenimiento" || fieldName == "equipo",
                    "Devolucion" => fieldName == "pagoMulta",
                    "Proveedor" => fieldName == "municipio",
                    "Usuario" => fieldName == "rolUsuario" || fieldName == "municipio",
                    "Equipo" => fieldName == "tipoEquipo" || fieldName == "ubicacion" || 
                                fieldName == "estadoEquipo" || fieldName == "proveedor",
                    _ => false
                };
            }

            // Métodos de validación específicos para cada entidad
            private void ValidateUbicacion(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoUbicacion":
                        ValidateStringField(value, 10, "codigoUbicacion");
                        break;
                    case "lugarUbicacion":
                        ValidateStringField(value, 50, "lugarUbicacion");
                        break;
                }
            }

            private void ValidateEstadoEquipo(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoEstadoEquipo":
                        ValidateStringField(value, 10, "codigoEstadoEquipo");
                        break;
                    case "nombreEstadoEquipo":
                        ValidateStringField(value, 50, "nombreEstadoEquipo");
                        break;
                }
            }

            private void ValidateProveedor(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoProveedor":
                        ValidateStringField(value, 10, "codigoProveedor");
                        break;
                    case "nombreProveedor":
                        ValidateStringField(value, 50, "nombreProveedor");
                        break;
                    case "noContactoProveedor":
                        ValidateStringField(value, 15, "noContactoProveedor");
                        break;
                    case "correoProveedor":
                        ValidateStringField(value, 50, "correoProveedor");
                        if (!value.Contains("@") || !value.Contains("."))
                            throw new ArgumentException("El correo debe tener formato válido");
                        break;
                    case "garantiaGenMesProveedor":
                        if (!int.TryParse(value, out int meses) || meses < 0)
                            throw new ArgumentException("La garantía debe ser un número entero positivo");
                        break;
                    case "municipio":
                        ValidateStringField(value, 10, "municipio");
                        break;
                }
            }

            private void ValidateEquipo(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoEquipo":
                        ValidateStringField(value, 10, "codigoEquipo");
                        break;
                    case "nombreEquipo":
                        ValidateStringField(value, 50, "nombreEquipo");
                        break;
                    case "costoEquipo":
                        if (!decimal.TryParse(value, out decimal costo) || costo < 0)
                            throw new ArgumentException("El costo debe ser un número positivo");
                        break;
                    case "tipoEquipo":
                    case "ubicacion":
                    case "estadoEquipo":
                    case "proveedor":
                        ValidateStringField(value, 10, name);
                        break;
                }
            }

            private void ValidateUsuario(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoUser":
                        ValidateStringField(value, 10, "codigoUser");
                        break;
                    case "nombreUser":
                        ValidateStringField(value, 50, "nombreUser");
                        break;
                    case "correoUser":
                        ValidateStringField(value, 50, "correoUser");
                        if (!value.Contains("@") || !value.Contains("."))
                            throw new ArgumentException("El correo debe tener formato válido");
                        break;
                    case "rolUsuario":
                    case "municipio":
                        ValidateStringField(value, 10, name);
                        break;
                }
            }

            private void ValidateMantenimiento(string name, string value, int? maxLength)
            {
                switch (name)
                {
                    case "codigoMantenimiento":
                        ValidateStringField(value, 10, "codigoMantenimiento");
                        break;
                    case "fechaMantenimiento":
                        if (!DateTime.TryParse(value, out _))
                            throw new ArgumentException("Fecha inválida");
                        break;
                    case "costoMantenimiento":
                        if (!decimal.TryParse(value, out decimal costo) || costo < 0)
                            throw new ArgumentException("El costo debe ser un número positivo");
                        break;
                    case "equipo":
                        ValidateStringField(value, 10, "equipo");
                        break;
                }
            }

            // Método auxiliar para validar campos string
            private void ValidateStringField(string value, int maxLength, string fieldName)
            {
                if (value.Length > maxLength)
                    throw new ArgumentException($"El campo {fieldName} no puede exceder {maxLength} caracteres");
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
                        var maintenance = new Maintenance()
                        {
                            MaintenanceId = values[0],
                            Date = DateTime.Parse(values[1], CultureInfo.InvariantCulture),
                            Findings = values[2],
                            Cost = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                            EquipmentId = values[4]
                        };
                        try
                        {
                            Console.WriteLine($"Intentando crear mantenimiento: ID={maintenance.MaintenanceId}, " +
                                             $"Fecha={maintenance.Date}, " +
                                             $"Hallazgos={maintenance.Findings}, " +
                                             $"Costo={maintenance.Cost}, " +
                                             $"EquipoID={maintenance.EquipmentId}");
                         
                            await _maintenanceController.crearMantenimiento(maintenance);
                            await ShowMessage("Éxito", "El mantenimiento se ha creado correctamente.");
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine($"Error de Oracle: {ex.Message}");
                            Console.WriteLine($"Código de error: {ex.Number}");
                            await ShowError("Error de Base de Datos", 
                                $"Error al crear el mantenimiento en la base de datos: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error general: {ex.Message}");
                            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
                try
                {
                    // Validaciones previas
                    if (string.IsNullOrWhiteSpace(values[0]))
                        throw new ArgumentException("El ID de devolución es obligatorio");
        
                    if (values[0].Length > 10)
                        throw new ArgumentException("El ID no puede exceder 10 caracteres");

                    if (string.IsNullOrWhiteSpace(values[2]))
                        throw new ArgumentException("Las observaciones son obligatorias");

                    if (string.IsNullOrWhiteSpace(values[4]))
                        throw new ArgumentException("El préstamo asociado es obligatorio");

                    var return_ = new Return
                    {
                        ReturnId = values[0],
                        Date = DateTime.ParseExact(values[1], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Notes = values[2],
                        PenaltyPaid = string.IsNullOrWhiteSpace(values[3]) ? null : decimal.Parse(values[3]),
                        LoanId = values[4]
                    };

                    await _returnController.createReturn(return_);
                    await ShowMessage("Éxito", "Devolución registrada correctamente");
                }
                catch (OracleException ex) when (ex.Number == 1)
                {
                    await ShowError("Error", $"Ya existe una devolución con el ID {values[0]}");
                }
                catch (OracleException ex) when (ex.Number == 2291)
                {
                    await ShowError("Error", $"El préstamo {values[4]} no existe en la base de datos");
                }
                catch (FormatException)
                {
                    await ShowError("Error de Formato", "Revise el formato de la fecha (debe ser DD/MM/AAAA) o del valor de penalización");
                }
                catch (ArgumentException ex)
                {
                    await ShowError("Error de Validación", ex.Message);
                }
                catch (Exception ex)
                {
                    await ShowError("Error Inesperado", $"Detalle técnico: {ex.Message}");
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