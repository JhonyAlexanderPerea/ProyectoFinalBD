using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

namespace ProyectoFinalBD.Controllers
{
    public class UserRoleController
    {
        private readonly UserRoleRepository _repository;

        public UserRoleController()
        {
            _repository = new UserRoleRepository();
        }

        public async Task<IEnumerable<UserRole>> ObtenerRolesUsuario()
        {
            var lista = await _repository.GetAll();
            Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} roles de usuario");
            return lista ?? new List<UserRole>();
        }
    }
}