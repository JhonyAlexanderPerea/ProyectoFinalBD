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
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.util;
using Location = ProyectoFinalBD.Model.Location;

namespace ProyectoFinalBD.View
{
    public partial class EditEntityWindow : Window
    {
        private readonly Grid _mainGrid;
        private readonly StackPanel _formPanel;
        private readonly Button _actualizarButton;
        private readonly ComboBox _entitySelector;
        private readonly Dictionary<string, List<Control>> _entityFields;
        private string _currentEntityId;
        
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
        
        public EditEntityWindow(string entityType = null, string entityId = null)
        {
            Title = "Editar Entidad";
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

            _entityFields = new Dictionary<string, List<Control>>();
            _currentEntityId = entityId;

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
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsEnabled = string.IsNullOrEmpty(entityType) // Solo habilitado si no se especificó tipo
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

            _actualizarButton = new Button
            {
                Content = "Actualizar",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            _actualizarButton.Click += OnActualizarClick;

            Grid.SetRow(_entitySelector, 0);
            Grid.SetRow(scrollViewer, 1);
            Grid.SetRow(_actualizarButton, 2);

            _mainGrid.Children.Add(_entitySelector);
            _mainGrid.Children.Add(scrollViewer);
            _mainGrid.Children.Add(_actualizarButton);

            Content = _mainGrid;

            // Si se especificó un tipo de entidad, seleccionarlo y cargar datos
            if (!string.IsNullOrEmpty(entityType))
            {
                _entitySelector.SelectedItem = entityType;
                if (!string.IsNullOrEmpty(entityId))
                {
                    _ = LoadEntityData(entityType, entityId);
                }
            }
        }

        private async Task LoadEntityData(string entityType, string entityId)
        {
            try
            {
                switch (entityType)
                {
                    case "DamageReport":
                        var damageReport = await _damageReportController.GetReportById(entityId);
                        if (damageReport != null)
                            PopulateFields(entityType, new List<string>
                            {
                                damageReport.DamageReportId,
                                damageReport.Date.ToString("dd-MM-yyyy"),
                                damageReport.Cause,
                                damageReport.Description,
                                damageReport.EquipmentId
                            });
                        break;
                    case "Equipment":
                        var equipment = await _equipmentController.GetEquipmentById(entityId);
                        if (equipment != null)
                            PopulateFields(entityType, new List<string>
                            {
                                equipment.EquipmentId,
                                equipment.Name,
                                equipment.Cost.ToString(CultureInfo.InvariantCulture),
                                equipment.Features,
                                equipment.EquipmentTypeId,
                                equipment.LocationId,
                                equipment.EquipmentStatusId,
                                equipment.SupplierId
                            });
                        break;
                    case "EquipmentStatus":
                        var status = await _equipmentStatusController.GetEquipmentStatusById(entityId);
                        if (status != null)
                            PopulateFields(entityType, new List<string>
                            {
                                status.EquipmentStatusId,
                                status.Name
                            });
                        break;
                    case "EquipmentType":
                        var type = await _equipmentTypeController.GetEquipmentTypeById(entityId);
                        if (type != null)
                            PopulateFields(entityType, new List<string>
                            {
                                type.EquipmentTypeId,
                                type.Name,
                                type.Description
                            });
                        break;
                    case "Loan":
                        var loan = await _loanController.GetLoanById(entityId);
                        if (loan != null)
                            PopulateFields(entityType, new List<string>
                            {
                                loan.LoanId,
                                loan.Date.ToString("dd-MM-yyyy"),
                                loan.DueDate.ToString("dd-MM-yyyy"),
                                loan.PenaltyCost.ToString(CultureInfo.InvariantCulture),
                                loan.EquipmentId,
                                loan.UserId
                            });
                        break;
                    case "Location":
                        var location = await _locationController.GetLocationById(entityId);
                        if (location != null)
                            PopulateFields(entityType, new List<string>
                            {
                                location.LocationId,
                                location.Name
                            });
                        break;
                    case "Maintenance":
                        var maintenance = await _maintenanceController.GetMaintenanceById(entityId);
                        if (maintenance != null)
                            PopulateFields(entityType, new List<string>
                            {
                                maintenance.MaintenanceId,
                                maintenance.Date.ToString("dd-MM-yyyy"),
                                maintenance.Findings,
                                maintenance.Cost.ToString(CultureInfo.InvariantCulture),
                                maintenance.EquipmentId
                            });
                        break;
                    case "Municipality":
                        var municipality = await _municipalityController.GetMunicipalityById(entityId);
                        if (municipality != null)
                            PopulateFields(entityType, new List<string>
                            {
                                municipality.MunicipalityId,
                                municipality.Name,
                                municipality.Description
                            });
                        break;
                    case "Return":
                        var return_ = await _returnController.GetReturnById(entityId);
                        if (return_ != null)
                            PopulateFields(entityType, new List<string>
                            {
                                return_.ReturnId,
                                return_.Date.ToString("dd-MM-yyyy"),
                                return_.Notes,
                                return_.PenaltyPaid?.ToString(CultureInfo.InvariantCulture) ?? "",
                                return_.LoanId
                            });
                        break;
                    case "Supplier":
                        var supplier = await _supplierController.GetSupplierById(entityId);
                        if (supplier != null)
                            PopulateFields(entityType, new List<string>
                            {
                                supplier.SupplierId,
                                supplier.Name,
                                supplier.Contact,
                                supplier.Email,
                                supplier.WarrantyMonths.ToString(),
                                supplier.MunicipalityId
                            });
                        break;
                    case "User":
                        var user = await _userController.GetUserById(entityId);
                        if (user != null)
                            PopulateFields(entityType, new List<string>
                            {
                                user.UserId,
                                user.Name,
                                user.Email,
                                user.Password,
                                user.UserRoleId,
                                user.MunicipalityId
                            });
                        break;
                    case "UserLog":
                        var userLog = await _userLogController.GetUserLogById(entityId);
                        if (userLog != null)
                            PopulateFields(entityType, new List<string>
                            {
                                userLog.UserLogId,
                                userLog.Date.ToString("dd-MM-yyyy"),
                                userLog.Entry,
                                userLog.UserId
                            });
                        break;
                }
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al cargar los datos de la entidad: {ex.Message}");
            }
        }

        private void PopulateFields(string entityType, List<string> values)
        {
            if (!_entityFields.TryGetValue(entityType, out var controls))
                return;

            for (int i = 0; i < controls.Count && i < values.Count; i++)
            {
                if (controls[i] is TextBox textBox)
                {
                    textBox.Text = values[i];
                }
                else if (controls[i] is DatePicker datePicker && DateTime.TryParse(values[i], out DateTime date))
                {
                    datePicker.SelectedDate = date;
                }
            }
        }

      private async void OnEntitySelected(object? sender, SelectionChangedEventArgs e)
{
    try
    {
        _formPanel.Children.Clear();
        var selectedEntity = _entitySelector.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedEntity)) return;

        var fields = GetEntityFields(selectedEntity);
        var controls = new List<Control>();

        // Obtener el nombre de la PK para la entidad seleccionada
        string primaryKeyName = GetPrimaryKeyName(selectedEntity);

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

            Control inputControl;
            
            if (field.Type == typeof(DateTime))
            {
                var datePicker = new DatePicker
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsEnabled = field.Name != primaryKeyName // Deshabilitar si es PK
                };
                inputControl = datePicker;
            }   
            else
            {
                var textBox = new TextBox
                {
                    Watermark = $"Ingrese {field.Name}",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = field.Name == primaryKeyName // Solo lectura si es PK
                };

                if (field.Type == typeof(decimal))
                {
                    textBox.Watermark = "0.00";
                }

                inputControl = textBox;
            }

            fieldPanel.Children.Add(label);
            fieldPanel.Children.Add(inputControl);
            _formPanel.Children.Add(fieldPanel);
            controls.Add(inputControl);
        }

        _entityFields[selectedEntity] = controls;
        
        // Si hay un ID cargado, cargar los datos
        if (!string.IsNullOrEmpty(_currentEntityId))
        {
            await LoadEntityData(selectedEntity, _currentEntityId);
        }
    }
    catch (Exception ex)
    {
        await ShowError("Error", $"Error al cargar los campos: {ex.Message}");
    }
}

// Método auxiliar para obtener el nombre de la PK de cada entidad
        public string GetPrimaryKeyName(string entityType)
        {
            return entityType switch
            {
                "DamageReport" => "codigoDanio",
                "Equipment" => "codigoEquipo",
                "EquipmentStatus" => "codigoEstadoEquipo",
                "EquipmentType" => "codigoTipoEquipo",
                "Loan" => "codigoPrestamo",
                "Location" => "codigoUbicacion",
                "Maintenance" => "codigoMantenimiento",
                "Municipality" => "codigoMunicipio",
                "Return" => "codigoDevolution",
                "Supplier" => "codigoProveedor",
                "User" => "codigoUser",
                "UserLog" => "codigoLogUser",
                _ => throw new ArgumentException("Tipo de entidad no reconocido", nameof(entityType))
            };
        }
        
        

        private async void OnActualizarClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEntity = _entitySelector.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedEntity))
                {
                    await ShowError("Error", "Debe seleccionar un tipo de entidad");
                    return;
                }

                if (string.IsNullOrEmpty(_currentEntityId))
                {
                    await ShowError("Error", "No se ha especificado una entidad para editar");
                    return;
                }

                var values = new List<string>();
                foreach (var control in _entityFields[selectedEntity])
                {
                    if (control is TextBox textBox)
                    {
                        values.Add(textBox.Text);
                    }
                    else if (control is DatePicker datePicker && datePicker.SelectedDate.HasValue)
                    {
                        values.Add(datePicker.SelectedDate.Value.ToString("dd-MM-yyyy"));
                    }
                    else
                    {
                        values.Add("");
                    }
                }

                var fields = GetEntityFields(selectedEntity);

                // Validar campos obligatorios (excepto los de solo lectura)
                for (int i = 0; i < fields.Count; i++)
                {
                    var control = _entityFields[selectedEntity][i];
                    if (control is TextBox { IsReadOnly: false } && string.IsNullOrWhiteSpace(values[i]))
                    {
                        await ShowError("Error", $"El campo {fields[i].Name} es obligatorio");
                        return;
                    }
                }

                await ValidateFields(selectedEntity, values);
                await UpdateEntity(selectedEntity, fields, values);
                Close();
            }
            catch (Exception ex)
            {
                await ShowError("Error", $"Error al actualizar la entidad: {ex.Message}");
            }
        }

        private async Task UpdateEntity(string entityName, List<(string Name, Type Type, int? MaxLength)> fields, List<string> values)
        {
            switch (entityName)
            {
                case "DamageReport":
                    var damageReport = new DamageReport
                    {
                        DamageReportId = _currentEntityId,
                        Date = DateTime.Parse(values[1]),
                        Cause = values[2],
                        Description = values[3],
                        EquipmentId = values[4]
                    };
                    await _damageReportController.UpdateReport(damageReport);
                    await ShowMessage("Éxito", "Reporte de daño actualizado correctamente");
                    break;
                
                case "Equipment":
                    var equipment = new Equipment
                    {
                        EquipmentId = _currentEntityId,
                        Name = values[1],
                        Cost = decimal.Parse(values[2]),
                        Features = values[3],
                        EquipmentTypeId = values[4],
                        LocationId = values[5],
                        EquipmentStatusId = values[6],
                        SupplierId = values[7]
                    };
                    await _equipmentController.UpdateEquipment(equipment);
                    await ShowMessage("Éxito", "Equipo actualizado correctamente");
                    break;
                
                case "EquipmentStatus":
                    var status = new EquipmentStatus
                    {
                        EquipmentStatusId = _currentEntityId,
                        Name = values[1]
                    };
                    await _equipmentStatusController.UpdateEquipmentStatus(status);
                    await ShowMessage("Éxito", "Estado de equipo actualizado correctamente");
                    break;
                
                case "EquipmentType":
                    var type = new EquipmentType
                    {
                        EquipmentTypeId = _currentEntityId,
                        Name = values[1],
                        Description = values[2]
                    };
                    await _equipmentTypeController.UpdateEquipmentType(type);
                    await ShowMessage("Éxito", "Tipo de equipo actualizado correctamente");
                    break;
                
                case "Loan":
                    var loan = new Loan
                    {
                        LoanId = _currentEntityId,
                        Date = DateTime.Parse(values[1]),
                        DueDate = DateTime.Parse(values[2]),
                        PenaltyCost = decimal.Parse(values[3]),
                        EquipmentId = values[4],
                        UserId = values[5]
                    };
                    await _loanController.UpdateLoan(loan);
                    await ShowMessage("Éxito", "Préstamo actualizado correctamente");
                    break;
                
                case "Location":
                    var location = new Location
                    {
                        LocationId = _currentEntityId,
                        Name = values[1]
                    };
                    await _locationController.UpdateLocation(location);
                    await ShowMessage("Éxito", "Ubicación actualizada correctamente");
                    break;
                
                case "Maintenance":
                    var maintenance = new Maintenance
                    {
                        MaintenanceId = _currentEntityId,
                        Date = DateTime.Parse(values[1]),
                        Findings = values[2],
                        Cost = decimal.Parse(values[3]),
                        EquipmentId = values[4]
                    };
                    await _maintenanceController.UpdateMaintenance(maintenance);
                    await ShowMessage("Éxito", "Mantenimiento actualizado correctamente");
                    break;
                
                case "Municipality":
                    var municipality = new Municipality
                    {
                        MunicipalityId = _currentEntityId,
                        Name = values[1],
                        Description = values[2]
                    };
                    await _municipalityController.UpdateMunicipality(municipality);
                    await ShowMessage("Éxito", "Municipio actualizado correctamente");
                    break;
                
                case "Return":
                    var return_ = new Return
                    {
                        ReturnId = _currentEntityId,
                        Date = DateTime.Parse(values[1]),
                        Notes = values[2],
                        PenaltyPaid = string.IsNullOrWhiteSpace(values[3]) ? null : decimal.Parse(values[3]),
                        LoanId = values[4]
                    };
                    await _returnController.UpdateReturn(return_);
                    await ShowMessage("Éxito", "Devolución actualizada correctamente");
                    break;
                
                case "Supplier":
                    var supplier = new Supplier
                    {
                        SupplierId = _currentEntityId,
                        Name = values[1],
                        Contact = values[2],
                        Email = values[3],
                        WarrantyMonths = int.Parse(values[4]),
                        MunicipalityId = values[5]
                    };
                    await _supplierController.UpdateSupplier(supplier);
                    await ShowMessage("Éxito", "Proveedor actualizado correctamente");
                    break;
                
                case "User":
                    var user = new User
                    {
                        UserId = _currentEntityId,
                        Name = values[1],
                        Email = values[2],
                        Password = values[3],
                        UserRoleId = values[4],
                        MunicipalityId = values[5]
                    };
                    await _userController.UpdateUser(user);
                    await ShowMessage("Éxito", "Usuario actualizado correctamente");
                    break;
                
                case "UserLog":
                    var userLog = new UserLog
                    {
                        UserLogId = _currentEntityId,
                        Date = DateTime.Parse(values[1]),
                        Entry = values[2],
                        UserId = values[3]
                    };
                    await _userLogController.UpdateUserLog(userLog);
                    await ShowMessage("Éxito", "Registro de usuario actualizado correctamente");
                    break;
            }
        }

        // Métodos auxiliares (copiados de CreateEntityWindow)
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

        private List<(string Name, Type Type, int? MaxLength)> GetEntityFields(string entityName)
        {
            return entityName switch
            {
                "DamageReport" => new List<(string, Type, int?)>
                {
                    ("codigoDanio", typeof(string), 10),
                    ("fechaDanio", typeof(DateTime), null),
                    ("causaDanio", typeof(string), 100),
                    ("descripcionDanio", typeof(string), null),
                    ("equipo", typeof(string), 10)
                },
                "Equipment" => new List<(string, Type, int?)>
                {
                    ("codigoEquipo", typeof(string), 10),
                    ("nombreEquipo", typeof(string), 50),
                    ("costoEquipo", typeof(decimal), null),
                    ("caracteristicasEquipo", typeof(string), null),
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
                    ("hallazgosMantenimiento", typeof(string), null),
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
                    ("observacionesDevolution", typeof(string), null),
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
                    ("registro", typeof(string), null),
                    ("usuario", typeof(string), 10)
                },
                _ => new List<(string, Type, int?)>()
            };
        }

        // Métodos de validación específicos (copiados de CreateEntityWindow)
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

        private void ValidateStringField(string value, int maxLength, string fieldName)
        {
            if (value.Length > maxLength)
                throw new ArgumentException($"El campo {fieldName} no puede exceder {maxLength} caracteres");
        }
    }
}