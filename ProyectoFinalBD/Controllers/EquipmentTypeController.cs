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
    }
}