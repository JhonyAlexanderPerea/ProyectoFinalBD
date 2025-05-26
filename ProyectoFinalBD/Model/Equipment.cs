using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class Equipment : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _equipmentId;
    private string _name;
    private decimal _cost;
    private string _features;
    private string? _equipmentTypeId;
    private EquipmentType? _equipmentType;
    private string? _locationId;
    private Location? _location;
    private string? _equipmentStatusId;
    private EquipmentStatus? _equipmentStatus;
    private string? _supplierId;
    private Supplier? _supplier;

    public string EquipmentId
    {
        get => _equipmentId;
        set
        {
            if (_equipmentId != value)
            {
                _equipmentId = value;
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

    public decimal Cost
    {
        get => _cost;
        set
        {
            if (_cost != value)
            {
                _cost = value;
                OnPropertyChanged();
            }
        }
    }

    public string Features
    {
        get => _features;
        set
        {
            if (_features != value)
            {
                _features = value;
                OnPropertyChanged();
            }
        }
    }

    public string? EquipmentTypeId
    {
        get => _equipmentTypeId;
        set
        {
            if (_equipmentTypeId != value)
            {
                _equipmentTypeId = value;
                OnPropertyChanged();
            }
        }
    }

    public EquipmentType? EquipmentType
    {
        get => _equipmentType;
        set
        {
            if (_equipmentType != value)
            {
                _equipmentType = value;
                OnPropertyChanged();
            }
        }
    }

    public string? LocationId
    {
        get => _locationId;
        set
        {
            if (_locationId != value)
            {
                _locationId = value;
                OnPropertyChanged();
            }
        }
    }

    public Location? Location
    {
        get => _location;
        set
        {
            if (_location != value)
            {
                _location = value;
                OnPropertyChanged();
            }
        }
    }

    public string? EquipmentStatusId
    {
        get => _equipmentStatusId;
        set
        {
            if (_equipmentStatusId != value)
            {
                _equipmentStatusId = value;
                OnPropertyChanged();
            }
        }
    }

    public EquipmentStatus? EquipmentStatus
    {
        get => _equipmentStatus;
        set
        {
            if (_equipmentStatus != value)
            {
                _equipmentStatus = value;
                OnPropertyChanged();
            }
        }
    }

    public string? SupplierId
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

    public Supplier? Supplier
    {
        get => _supplier;
        set
        {
            if (_supplier != value)
            {
                _supplier = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}