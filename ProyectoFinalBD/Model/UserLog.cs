using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class UserLog : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _userLogId;
    private DateTime _date;
    private string _entry;
    private string? _userId;
    

    public string UserLogId
    {
        get => _userLogId;
        set
        {
            if (_userLogId != value)
            {
                _userLogId = value;
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

    public string Entry
    {
        get => _entry;
        set
        {
            if (_entry != value)
            {
                _entry = value;
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
    

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}