using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DealOrNoDealGame.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace DealOrNoDealGame.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Box> Boxes { get; set; }

    private List<double> boxValues = new List<double> { 0.01, 0.10, 0.50, 1, 2, 5, 10, 50, 100, 250, 500, 750,
        1000, 1500, 2500, 5000, 7500, 10000, 12500, 15000, 20000, 25000, 50000, 100000};

    private bool _isPlayerBoxSelected;
    public bool IsPlayerBoxSelected
    {
        get => _isPlayerBoxSelected;
        set
        {
            _isPlayerBoxSelected = value;
            OnPropertyChanged(nameof(IsPlayerBoxSelected));
        }
    }

    private int boxesToOpen;
    public int BoxesToOpen
    {
        get => boxesToOpen;
        set
        {
            boxesToOpen = value;
            OnPropertyChanged(nameof(BoxesToOpen));
        }
    }

    private int roundNumber = 1;
    public int RoundNumber
    {
        get => roundNumber;
        set
        {
            roundNumber = value;
            OnPropertyChanged(nameof(roundNumber));
        }
    }

    private Box _playerBox;
    public Box PlayerBox
    {
        get => _playerBox;
        set
        {
            _playerBox = value;
            OnPropertyChanged(nameof(PlayerBox));
        }
    }

    private bool _isDealButtonVisible;
    public bool IsDealButtonVisible
    {
        get => _isDealButtonVisible;
        set
        {
            if (_isDealButtonVisible != value)
            {
                _isDealButtonVisible = value;
                OnPropertyChanged(nameof(IsDealButtonVisible));
            }
        }
    }

    private ICommand _dealOrNoDealCommand;
    public ICommand DealOrNoDealCommand => _dealOrNoDealCommand ??= new Command(DealOrNoDeal);

    public ICommand SelectOrOpenBoxCommand { get; }

    public MainViewModel()
    {
        Boxes = new ObservableCollection<Box>();

        for (int i = 1; i <= 24; i++)
        {
            var box = new Box { Number = i, Value = GetRandomValue() };
            Boxes.Add(box);
            boxValues.Remove(box.Value);
        }
        SelectOrOpenBoxCommand = new Command<int>(SelectOrOpenBox);
    }

    private async void SelectOrOpenBox(int boxNumber)
    {
        if (!IsPlayerBoxSelected)
        {
            var selectedBox = Boxes.FirstOrDefault(b => b.Number == boxNumber);
            if (selectedBox != null)
            {
                selectedBox.IsPlayerBox = true;
                PlayerBox = selectedBox;
                IsPlayerBoxSelected = true;
                Boxes.Remove(selectedBox);

                StartGame();
            }
        }
        else
        {
            if (BoxesToOpen <= 0)
            {
                return;
            }
            var boxToOpen = Boxes.FirstOrDefault(b => b.Number == boxNumber);
            if (boxToOpen != null)
            {
                boxToOpen.OpenedValue = boxToOpen.Value;
                    
                boxToOpen.OnPropertyChanged(nameof(Box.DisplayText));
                boxToOpen.OnPropertyChanged(nameof(Box.BackgroundColor));

                await Task.Delay(2000);

                Boxes.Remove(boxToOpen);
                BoxesToOpen--;

                if (BoxesToOpen == 0)
                {
                    EndRound();
                    IsDealButtonVisible = true;
                }
            }
        }
    }

    private void StartGame()
    {
        StartRound();
    }

    private void StartRound()
    {
        switch (RoundNumber)
        {
            case 1: BoxesToOpen = 6;
                break;
            case 2: BoxesToOpen = 4;
                break;
            case 3: BoxesToOpen = 3;
                break;
            case 4: BoxesToOpen = 3;
                break;
            case 5: BoxesToOpen = 3;
                break;
            case 6: BoxesToOpen = 2;
                break;
            case 7: BoxesToOpen = 1;
                break;
        }
    }

    private void EndRound()
    {
        RoundNumber++;
    }

    private double GetRandomValue()
    {
        Random random = new Random();
        int randomIndex = random.Next(0, boxValues.Count);

        return boxValues[randomIndex];
    }

    private void DealOrNoDeal()
    {
        StartRound();

        IsDealButtonVisible = false;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
