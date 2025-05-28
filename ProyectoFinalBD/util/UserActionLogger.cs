using System;
using System.Threading.Tasks;
using ProyectoFinalBD.Controllers;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.util;

public class UserActionLogger
{
    private  UserController _userController;
    private UserLog userLog;
    private string userId;

    public UserActionLogger()
    {
        _userController = new UserController();
        userLog = new UserLog();
    }
    public async Task<UserLog> createInfoUserLog(string info, string userId)
    {
        userLog = new UserLog
        {
            UserLogId = generarCodigoAleatorio(),
            UserId = userId,
            Date = DateTime.Now,
            Entry = info
        };
        return userLog;
    }
    public string generarCodigoAleatorio()
    {
        var random = new Random();
        int numero = random.Next(0, 100000); // 0 a 99999
        return numero.ToString("D5"); // Siempre 5 dígitos, con ceros a la izquierda
    }
}