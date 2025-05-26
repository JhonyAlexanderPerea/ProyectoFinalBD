using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class DamageReport : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _damageReportId;
    private DateTime _date;
    private string _cause;
    private string? _description;
    private string? _equipmentId;
    private Equipment? _equipment;

    public string DamageReportId
    {
        get => _damageReportId;
        set
        {
            if (_damageReportId != value)
            {
                _damageReportId = value;
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

    public string Cause
    {
        get => _cause;
        set
        {
            if (_cause != value)
            {
                _cause = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
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

    public Equipment? Equipment
    {
        get => _equipment;
        set
        {
            if (_equipment != value)
            {
                _equipment = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}