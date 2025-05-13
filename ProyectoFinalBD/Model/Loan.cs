using System;

namespace ProyectoFinalBD.Model;

public class Loan
{
    public int LoanId { get; set; } 
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public DateTime DeliveryDate { get; set; } 
    public DateTime? ReturnDate { get; set; } 
    public string LoanStatus { get; set; } 
    /*public User User { get; set; }
    public Equipment Equipment { get; set; } */
}
