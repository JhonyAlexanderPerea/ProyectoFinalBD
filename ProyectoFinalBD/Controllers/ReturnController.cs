using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class ReturnController
    {
        private readonly ReturnRepository _repository;

        public ReturnController()
        {
            _repository = new ReturnRepository();
        }

        public async Task<IEnumerable<Return>> ObtenerDevoluciones()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} devoluciones");
            return lista ?? new List<Return>();
        }

        public async Task createReturn(Return returnObj)
        {
            await _repository.Create(returnObj);

            }

        public async Task EliminarDevolucion(string devolucionReturnId)
        {
            try
            {
                await _repository.Delete(devolucionReturnId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("LA DEVOLUCIÓN DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR LA DEVOLUCIÓN");
                Console.ResetColor();               
            } 
            ;  
        }

        public async Task <Return> GetReturnById(string entityId)
        {
            return await _repository.GetById(entityId);
        }

        public async Task UpdateReturn(Return @return)
        {
            try
            {
                await _repository.Update(@return);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR AL INTENTAR ACTUALIZAR LA DEVOLUCIÓN: ERROR -> {e}");
                throw;
            }
        }
    }
}