using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoFinalBD.DAO;
using ProyectoFinalBD.Model;

public class UserController
{
    private readonly UserRepository _repository;
    private static User _currentUser;

    public UserController()
    {
        _repository = new UserRepository();
    }

    public static User CurrentUser => _currentUser;
    
    public async Task<IEnumerable<User>> ObtenerUsuarios()
    {
        var lista = await _repository.GetAllUsers();
        Console.WriteLine($"Controlador recuperó {lista?.Count ?? 0} roles de usuario");
        return lista ?? new List<User>();
    }


    public async Task<bool> Login(string cedula, string password)
    {
        try
        {
            var user = await _repository.ValidateLogin(cedula, password);
            if (user != null)
            {
                _currentUser = user;
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public async Task createUser(User user)
    {
        await _repository.CreateUser(user);

    }

    public async Task EliminarUsuario(string usuarioUserId)
    {
        try
        {
            await _repository.Delete(usuarioUserId);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("EL USUARIO DE DAÑO HA SIDO ELIMINADO CORRECTAMENTE");
            Console.ResetColor(); 
        }
        catch(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HUBO UN ERROR AL INTENTAR ELIMINAR EL USUARIO");
            Console.ResetColor();               
        } 
        ;
    }

    public async Task <User> GetUserById(string entityId)
    {
        return await _repository.GetUserById(entityId);
    }

    public async Task UpdateUser(User user)
    {
        try
        {
            await _repository.Update(user);
            Console.WriteLine("USUARIO ACTUALIZADO CORRECTAMENTE");
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR AL INTENTAR ACTUALIZAR EL USUARIO: ERROR ->{e}");
            throw;
        }
    }
}