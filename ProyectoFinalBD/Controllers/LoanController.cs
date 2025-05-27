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

        public async Task createLoan(Loan loan)
        {
            await _repository.Create(loan);

        }

        public async Task EliminarPrestamo(string prestamoLoanId)
        {
            try
            {
                await _repository.Delete(prestamoLoanId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL PRESTAMO DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL PRESTAMO");
                Console.ResetColor();               
            } 
            ;  
        }
    }
}