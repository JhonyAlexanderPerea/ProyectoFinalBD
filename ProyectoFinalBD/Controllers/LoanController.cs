using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class LoanController
    {
        private readonly LoanRepository _repository;

        public LoanController()
        {
            _repository = new LoanRepository();
        }

        public async Task<IEnumerable<Loan>> ObtenerPrestamos()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} préstamos");
            return lista ?? new List<Loan>();
        }
    }
}