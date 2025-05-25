using System;

namespace ProyectoFinalBD.Model;

public class Maintenance
{
    public string MaintenanceId { get; set; }
    public DateTime Date { get; set; }
    public string? Findings { get; set; }
    public decimal Cost { get; set; }

    public string? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
}