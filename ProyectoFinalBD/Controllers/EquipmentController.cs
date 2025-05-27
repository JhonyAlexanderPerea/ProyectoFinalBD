using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class EquipmentController
    {
        private readonly EquipmentRepository _repository;

        public EquipmentController()
        {
            _repository = new EquipmentRepository();
        }

        public async Task<IEnumerable<Equipment>> ObtenerEquipos()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} equipos");
            return lista ?? new List<Equipment>();
        }

        public async Task CreateEquipment(Equipment equipment)
        {
            await _repository.Create(equipment);
        }
        

        public async Task EliminarEquipo(string equipoEquipmentId)
        {
            try
            {
                await _repository.Delete(equipoEquipmentId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL EQUIPO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL EQUIPO");
                Console.ResetColor();               
            } 
            ;
        }
    }
}