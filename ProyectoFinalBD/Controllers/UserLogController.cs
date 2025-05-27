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

        public async Task EliminarRegistro(string registroUserLogId)
        {
            try
            {
                await _repository.Delete(registroUserLogId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL LOG DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL LOG");
                Console.ResetColor();               
            } 
            ;  
        }
    }
}