using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class SupplierController
    {
        private readonly SupplierRepository _repository;

        public SupplierController()
        {
            _repository = new SupplierRepository();
        }

        public async Task<IEnumerable<Supplier>> ObtenerProveedores()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} proveedores");
            return lista ?? new List<Supplier>();
        }
    }
}