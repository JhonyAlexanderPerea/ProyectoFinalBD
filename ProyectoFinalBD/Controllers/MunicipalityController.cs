using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class MunicipalityController
    {
        private readonly MunicipalityRepository _repository;

        public MunicipalityController()
        {
            _repository = new MunicipalityRepository();
        }

        public async Task<IEnumerable<Municipality>> ObtenerMunicipios()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} municipios");
            return lista ?? new List<Municipality>();
        }
    }
}