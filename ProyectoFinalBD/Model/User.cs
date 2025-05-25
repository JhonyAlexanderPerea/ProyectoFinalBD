namespace ProyectoFinalBD.Model;

public class User
{
    public string UserId { get; set; } = string.Empty; // codigoUser
    public string Name { get; set; } = string.Empty;  // nombreUser
    public string Email { get; set; } = string.Empty; // correoUser
    public string Password { get; set; } = string.Empty; // contrasenaUser
    public string? UserRoleId { get; set; }           // rolUsuario
    public string? UserRole { get; set; }           // Objeto de navegación
    public string? MunicipalityId { get; set; }       // municipio
    public Municipality? Municipality { get; set; }    // Objeto de navegación
}