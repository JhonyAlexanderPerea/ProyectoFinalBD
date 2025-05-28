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

        public async Task createMunicipality(Municipality municipality)
        {
            await _repository.Create(municipality);

        }

        public async Task EliminarMunicipio(string municipioMunicipalityId)
        {
            try
            {
                await _repository.Delete(municipioMunicipalityId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EL MUNICIPO DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
                Console.ResetColor(); 
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL MUNICIPO");
                Console.ResetColor();               
            } 
            ;        }

        public async Task <Municipality> GetMunicipalityById(string entityId)
        {
            return await _repository.GetById(entityId);
        }

        public async Task UpdateMunicipality(Municipality municipality)
        {
            try
            {
                await _repository.Update(municipality);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR AL INTENTAR ACTUALIZAR EL MUNICIPIO: ERROR -> {e}");
                throw;
            }
        }
    }
}