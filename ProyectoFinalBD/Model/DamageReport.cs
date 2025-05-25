using System;

namespace ProyectoFinalBD.Model;


public class DamageReport
{
    public string DamageReportId { get; set; }
    public DateTime Date { get; set; }
    public string Cause { get; set; }
    public string? Description { get; set; }

    public string? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
}
