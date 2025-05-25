using System;

namespace ProyectoFinalBD.Model;

public class Return
{
    public string ReturnId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
    public decimal? PenaltyPaid { get; set; }

    public string LoanId { get; set; }
    public Loan Loan { get; set; }
}