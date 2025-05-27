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
}