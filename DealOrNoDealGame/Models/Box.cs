using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealOrNoDealGame.Models;

public class Box : INotifyPropertyChanged
{
    public int Number { get; set; }
    public double Value { get; set; }

    private double? _openedValue;
    public double? OpenedValue
    {
        get => _openedValue;
        set
        {
            if (_openedValue != value)
            {
                _openedValue = value;
                OnPropertyChanged(nameof(OpenedValue));
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
    }

    public bool IsPlayerBox { get; set; } = false;

    public string DisplayText
    {
        get
        {
            return OpenedValue.HasValue ? $"{OpenedValue.Value}" : $"{Number}";
        }
    }

    public Color BackgroundColor
    {
        get
        {
            // Use predefined Color values instead of strings
            return OpenedValue.HasValue ? Color.FromRgba("#f71d56") : Color.FromRgba("#000000");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}





