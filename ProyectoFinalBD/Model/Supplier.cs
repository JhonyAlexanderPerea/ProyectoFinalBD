using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class Supplier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _supplierId;
    private string _name;
    private string _contact;
    private string? _email;
    private int _warrantyMonths;
    private string? _municipalityId;
    private Municipality? _municipality;

    public string SupplierId
    {
        get => _supplierId;
        set
        {
            if (_supplierId != value)
            {
                _supplierId = value;
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

    public string Contact
    {
        get => _contact;
        set
        {
            if (_contact != value)
            {
                _contact = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Email
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

    public int WarrantyMonths
    {
        get => _warrantyMonths;
        set
        {
            if (_warrantyMonths != value)
            {
                _warrantyMonths = value;
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