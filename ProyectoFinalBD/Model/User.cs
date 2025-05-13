using ProyectoFinalBD.Enums;

namespace ProyectoFinalBD.Model;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserType UserType { get; set; }
    public string Password { get; set; }
}

