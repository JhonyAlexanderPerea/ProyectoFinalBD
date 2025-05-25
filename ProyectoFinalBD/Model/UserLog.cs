using System;

namespace ProyectoFinalBD.Model;

public class UserLog
{
    public string UserLogId { get; set; }
    public DateTime Date { get; set; }
    public string Entry { get; set; }

    public string? UserId { get; set; }
    public User? User { get; set; }
}