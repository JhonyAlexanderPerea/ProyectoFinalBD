using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;
using ProyectoFinalBD.util;

namespace ProyectoFinalBD.View;

public partial class Register : Window
{
    private UserRepository _userRepository;
    private Message _message;
    public ObservableCollection<Municipality> Municipalities { get; set; } = new();
    public Municipality SelectedMunicipality { get; set; }

    private readonly MunicipalityRepository _municipalityRepository = new();

    public Register()
    {
        InitializeComponent();
        DataContext = this;
        _userRepository = new UserRepository();
        _message = new Message(); 
        _ = LoadMunicipalitiesAsync();
        
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
    
    private void Exit(object? sender, RoutedEventArgs e)
    {
        Login login = new Login();
        login.Show();
        this.Close();
        
    }


    private async void RegisterUser(object? sender, RoutedEventArgs e)
    {
        await _message.ShowMessage(this, "Debug", "Método RegisterUser iniciado");

        string nombreUser = Name.Text?.Trim() ?? "";
        string password = Password.Text?.Trim() ?? "";
        string correoUser = Email.Text?.Trim() ?? "";
        string cedulaUser = Cedula.Text?.Trim() ?? "";
        string municipio = SelectedMunicipality?.MunicipalityId ?? ""; // Cambio importante aquí
        string rolUsuario ="USER"; // Rol fijo por defecto

        // Validar campos obligatorios
        if (string.IsNullOrEmpty(cedulaUser) || string.IsNullOrEmpty(nombreUser) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(municipio))
        {
            await _message.ShowMessage(this, "Error", "Por favor completa todos los campos obligatorios.");
            return;
        }

        // Validar si usuario existe (usando la cédula como ID)
        bool exists = await _userRepository.Exists(cedulaUser, correoUser);
        if (exists)
        {
            await _message.ShowMessage(this, "Error", "El usuario ya existe con la misma cédula o correo.");
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
            await _message.ShowMessage(this, "Debug", $"Intentando guardar usuario: {newUser.UserId}, {newUser.Email}, {newUser.MunicipalityId}");
            await _userRepository.CreateUser(newUser);
            await _message.ShowMessage(this, "Debug", $"Usuario {newUser.UserId} guardado (o al menos no hubo error)");
            await _message.ShowMessage(this, "Éxito", "Usuario registrado correctamente.");
            Login login = new Login();
            login.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            await _message.ShowMessage(this, "Error", $"Error al registrar usuario: {ex.Message}");
        }
    }


}