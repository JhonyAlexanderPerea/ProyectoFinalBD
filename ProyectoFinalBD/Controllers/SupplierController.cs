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

        public async Task createSupplier(Supplier supplier)
        {
            await _repository.Create(supplier);
        }

        public async Task EliminarProveedor(string proveedorSupplierId)
        {
            try
            {
                await _repository.Delete(proveedorSupplierId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL PROVEEDOR HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL PROVEEDOR");
                Console.ResetColor();               
            } 
            ;
        }

        public async Task <Supplier> GetSupplierById(string entityId)
        {
            return await _repository.GetById(entityId);
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            try
            {
                await _repository.Update(supplier);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR AL INTENTAR ACTUALIZAR EL PROVEEDOR: ERROR -> {e}");
                throw;
            }
        }
    }
}