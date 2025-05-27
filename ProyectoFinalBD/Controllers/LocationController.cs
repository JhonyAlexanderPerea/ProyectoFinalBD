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
    }
}