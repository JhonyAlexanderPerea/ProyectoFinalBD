using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class Loan : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _loanId;
    private DateTime _date;
    private DateTime _dueDate;
    private decimal _penaltyCost;

    private string? _equipmentId;
    private Equipment? _equipment;

    private string? _userId;
    private User? _user;

    public string LoanId
    {
        get => _loanId;
        set
        {
            if (_loanId != value)
            {
                _loanId = value;
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

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate != value)
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal PenaltyCost
    {
        get => _penaltyCost;
        set
        {
            if (_penaltyCost != value)
            {
                _penaltyCost = value;
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

    public string? UserId
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

    public User? User
    {
        get => _user;
        set
        {
            if (_user != value)
            {
                _user = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
