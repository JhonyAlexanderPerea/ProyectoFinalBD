using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers;

public class DamageReportController
{   
    private readonly DamageReportRepository _repository;
    public DamageReportController()
    {
        _repository = new DamageReportRepository();
    }
    public async Task<IEnumerable<DamageReport>> ObtenerReportesDaños()
    {
        var lista = await _repository.GetAll();
        Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} reportes de daños");
        return lista ?? new List<DamageReport>();

    }

    public async Task CreateReport(DamageReport damageReport)
    {
        await _repository.Create(damageReport);
    }

    public async Task EliminarReporteDaño(string reporteDamageReportId)
    {
        try
        {
            await _repository.Delete(reporteDamageReportId);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("EL REPORTE DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
            Console.ResetColor(); 
        }
        catch(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL REPORTE DE DAÑO");
            Console.ResetColor();               
        } 
        ;
    }
}