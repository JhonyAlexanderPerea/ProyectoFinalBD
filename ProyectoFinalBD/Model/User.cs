using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class User : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _userId = string.Empty;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string? _userRoleId;
    private string? _userRole;
    private string? _municipalityId;
    private Municipality? _municipality;

    public string UserId
    {
        get => _userId;
        set
        {
            if (_userId != value)
            {
                _userId = value;
                OnPropertyChanged();
            }
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
            }
        }
    }

    public string? UserRoleId
    {
        get => _userRoleId;
        set
        {
            if (_userRoleId != value)
            {
                _userRoleId = value;
                OnPropertyChanged();
            }
        }
    }

    public string? UserRole
    {
        get => _userRole;
        set
        {
            if (_userRole != value)
            {
                _userRole = value;
                OnPropertyChanged();
            }
        }
    }

    public string? MunicipalityId
    {
        get => _municipalityId;
        set
        {
            if (_municipalityId != value)
            {
                _municipalityId = value;
                OnPropertyChanged();
            }
        }
    }

    public Municipality? Municipality
    {
        get => _municipality;
        set
        {
            if (_municipality != value)
            {
                _municipality = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}