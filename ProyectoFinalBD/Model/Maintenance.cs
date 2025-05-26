using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class Maintenance : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _maintenanceId; 
    private DateTime _date; 
    private string? _findings;
    private decimal _cost;
    private string? _equipmentId;
    
    


    public string MaintenanceId
    {
        get => _maintenanceId;
        set
        {
            if (_maintenanceId != value)
            {
                _maintenanceId = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime Date
    {
        get => _date;
        set
        {
            if (_date != value)
            {
                _date = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Findings
    {
        get => _findings;
        set
        {
            if (_findings != value)
            {
                _findings = value;
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

    public string? EquipmentId
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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}