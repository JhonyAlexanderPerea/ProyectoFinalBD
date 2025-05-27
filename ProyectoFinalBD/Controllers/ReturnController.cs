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

        public async Task createReturn(Return @return)
        {
            await _repository.Create(@return);

            }
    }
}