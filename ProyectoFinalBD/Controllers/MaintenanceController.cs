using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class MaintenanceController
    {
        private readonly MaintenanceRepository _repository;

        public MaintenanceController()
        {
            _repository = new MaintenanceRepository();
        }

        public async Task<List<Maintenance>> ObtenerMantenimientos()
        {
            // Llamamos al repositorio real
            var lista = await _repository.GetAll();

            // Puedes registrar para debug
            Console.WriteLine(lista == null
                ? "Repositorio devolvió null"
                : $"Repositorio devolvió {lista.Count} registros");

            return lista ?? new List<Maintenance>();
        }

        public async Task<Maintenance> crearMantenimiento(Maintenance mantenimiento) {
            try
            {
                await _repository.Create(mantenimiento);
                return mantenimiento;
            }
            catch(Exception e)
            {
                Console.WriteLine("HUBO UN ERROR AL INTENTAR GUARDAR EL MANTENIMIENTO");
                return null;
            }
        }
    }
}