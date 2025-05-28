using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.util;

namespace ProyectoFinalBD.View;

public partial class RegisterUsersView : UserControl
{
    private UserRepository _userRepository;
    private Message _message;
    public ObservableCollection<Municipality> Municipalities { get; set; } = new();
    public ObservableCollection<UserRole> Roles { get; set; } = new();
    public Municipality SelectedMunicipality { get; set; } 
    public UserRole SelectedUserRol { get; set; }

    private readonly MunicipalityRepository _municipalityRepository = new();
    private readonly UserRoleController _userRoleController = new();

    public RegisterUsersView()
    {
        InitializeComponent();
        DataContext = this;
        _userRepository = new UserRepository();
        _message = new Message(); 
        _ = LoadMunicipalitiesAsync();
        _ = LoadRolesAsync();  
        
    }


    private async Task LoadMunicipalitiesAsync()
    {
        var municipios = await _municipalityRepository.GetAll();
        Municipalities.Clear();

        foreach (var municipio in municipios)
        {
            Municipalities.Add(municipio);
        }
    }
    private async Task LoadRolesAsync()
    {
        var roles = await _userRoleController.ObtenerRolesUsuario();
        Roles.Clear();

        foreach (var rol in roles)
        {
            Roles.Add(rol);
        }
    }


    private async void RegisterUser(object? sender, RoutedEventArgs e)
    {
        await ShowMessage("Debug", "Método RegisterUser iniciado");

        string nombreUser = Name.Text?.Trim() ?? "";
        string password = Password.Text?.Trim() ?? "";
        string correoUser = Email.Text?.Trim() ?? "";
        string cedulaUser = Cedula.Text?.Trim() ?? "";
        string municipio = SelectedMunicipality?.MunicipalityId ?? ""; // Cambio importante aquí
        string rolUsuario = SelectedUserRol?.UserRoleId ?? "";

        // Validar campos obligatorios
        if (string.IsNullOrEmpty(cedulaUser) || string.IsNullOrEmpty(nombreUser) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(municipio) || string.IsNullOrEmpty(rolUsuario))
        {
            await ShowMessage("Error", "Por favor completa todos los campos obligatorios.");
            return;
        }

        // Validar si usuario existe (usando la cédula como ID)
        bool exists = await _userRepository.Exists(cedulaUser, correoUser);
        if (exists)
        {
            await ShowMessage("Error", "El usuario ya existe con la misma cédula o correo.");
            return;
        }

        // Crear el usuario
        var newUser = new User
        {
            UserId = cedulaUser,
            Name = nombreUser,
            Password = password,
            Email = correoUser,
            UserRoleId = rolUsuario,
            MunicipalityId = municipio
        };

        try
        {
            
            await _userRepository.CreateUser(newUser);
            await ShowMessage("Éxito", "Usuario registrado correctamente.");
        }
        catch (Exception ex)
        {
            await ShowMessage("Error", $"Error al registrar usuario: {ex.Message}");
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

        ((Button)((StackPanel)dialog.Content).Children[1]).Click += (s, e) => dialog.Close();
        await dialog.ShowDialog(GetWindow());
    }
    private Window GetWindow() => (Window)this.VisualRoot!;



}