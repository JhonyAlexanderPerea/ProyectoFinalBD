using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class EquipmentStatusController
    {
        private readonly EquipmentStatusRepository _repository;

        public EquipmentStatusController()
        {
            _repository = new EquipmentStatusRepository();
        }

        public async Task<IEnumerable<EquipmentStatus>> ObtenerEstadosEquipo()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} estados de equipo");
            return lista ?? new List<EquipmentStatus>();
        }

        public async Task createEquipmentStatus(EquipmentStatus equipmentStatus)
        {
            await _repository.Create(equipmentStatus);

        }
    }
}