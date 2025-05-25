using System;
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

    public async Task<bool> Login(string email, string password)
    {
        try
        {
            var user = await _repository.ValidateLogin(email, password);
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
}