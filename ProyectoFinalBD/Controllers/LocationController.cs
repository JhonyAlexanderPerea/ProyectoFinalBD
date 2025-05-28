using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class LocationController
    {
        private readonly LocationRepository _repository;

        public LocationController()
        {
            _repository = new LocationRepository();
        }

        public async Task<IEnumerable<Location>> ObtenerUbicaciones()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} ubicaciones");
            return lista ?? new List<Location>();
        }

        public async Task createLocation(Location location)
        {
            await _repository.Create(location);
        }

        public async Task EliminarUbicacion(string ubicacionLocationId)
        {
            try
            {
                await _repository.Delete(ubicacionLocationId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("LA UBICACIÓN HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR LA UBICACIÓN");
                Console.ResetColor();               
            } 
            ;
        }

        public async Task <Location> GetLocationById(string entityId)
        {
            return await _repository.GetById(entityId);
        }

        public async Task UpdateLocation(Location location)
        {
            try
            {
                await _repository.Update(location);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR AL INTENTAR ACTUALIZAR LA UBICACIÓN : ERROR -> {e}");
                throw;
            }
        }
    }
}