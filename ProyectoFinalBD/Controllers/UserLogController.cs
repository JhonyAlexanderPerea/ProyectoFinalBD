using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class UserLogController
    {
        private readonly UserLogRepository _repository;

        public UserLogController()
        {
            _repository = new UserLogRepository();
        }

        public async Task<IEnumerable<UserLog>> ObtenerRegistrosUsuario()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} registros de usuario");
            return lista ?? new List<UserLog>();
        }

        public async Task createUserLog(UserLog userLog)
        {
            await _repository.Create(userLog);
        }
    }
}