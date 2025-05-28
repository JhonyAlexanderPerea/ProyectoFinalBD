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

        public async Task <EquipmentStatus>GetEquipmentStatusById(string entityId)
        {
            return await _repository.GetById(entityId);
        }

        public async Task UpdateEquipmentStatus(EquipmentStatus status)
        {
            try
            {
                await _repository.Update(status);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR AL INTENTAR ACTUALIZAR UN ESTADO DE EQUIPO: ERROR -> {e}" );
                throw;
            }
        }
    }
}