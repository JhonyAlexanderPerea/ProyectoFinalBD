using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinalBD.Model;

public class Return : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _returnId;
    private DateTime _date;
    private string _notes;
    private decimal? _penaltyPaid;
    private string _loanId;
    private Loan? _loan;

    public string ReturnId
    {
        get => _returnId;
        set
        {
            if (_returnId != value)
            {
                _returnId = value;
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

    public string Notes
    {
        get => _notes;
        set
        {
            if (_notes != value)
            {
                _notes = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal? PenaltyPaid
    {
        get => _penaltyPaid;
        set
        {
            if (_penaltyPaid != value)
            {
                _penaltyPaid = value;
                OnPropertyChanged();
            }
        }
    }

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

    public Loan? Loan
    {
        get => _loan;
        set
        {
            if (_loan != value)
            {
                _loan = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}