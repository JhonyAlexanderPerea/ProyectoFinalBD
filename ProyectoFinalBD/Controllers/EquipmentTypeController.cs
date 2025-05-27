using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class EquipmentTypeController
    {
        private readonly EquipmentTypeRepository _repository;

        public EquipmentTypeController()
        {
            _repository = new EquipmentTypeRepository();
        }

        public async Task<IEnumerable<EquipmentType>> ObtenerTiposEquipo()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} tipos de equipo");
            return lista ?? new List<EquipmentType>();
        }

        public async Task createEquipmentType(EquipmentType equipmentType)
        {
            await _repository.Create(equipmentType);

        }

        public async Task EliminarTipoEquipo(string tipoEquipoEquipmentTypeId)
        {
            try
            {
                await _repository.Delete(tipoEquipoEquipmentTypeId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL TIPO EQUIPO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
                
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL TIPO EQUIPO");
                Console.ResetColor(); 
                
            }
        }
    }
}