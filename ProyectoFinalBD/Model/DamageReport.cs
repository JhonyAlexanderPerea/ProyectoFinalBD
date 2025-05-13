using System;

namespace ProyectoFinalBD.Model;


public class DamageReport
{
    public int DamageReportId { get; set; } // id_dano
    public int EquipmentId { get; set; } // FK

    public string Description { get; set; } // descripcion
    public DateTime ReportDate { get; set; } // fecha

    public Equipment Equipment { get; set; } // navegación opcional
}
