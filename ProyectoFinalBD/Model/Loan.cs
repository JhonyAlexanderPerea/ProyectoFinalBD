using System;

namespace ProyectoFinalBD.Model;

public class Loan
{
    public string LoanId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public decimal PenaltyCost { get; set; }

    public string? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public string? UserId { get; set; }
    public User? User { get; set; }
}
