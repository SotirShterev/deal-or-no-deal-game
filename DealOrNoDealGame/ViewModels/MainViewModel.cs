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

    private List<double> initialBoxValues = new List<double> { 0.01, 0.10, 0.50, 1, 2, 5, 10, 50, 100, 250, 500, 750,
        1000, 1500, 2500, 5000, 7500, 10000, 12500, 15000, 20000, 25000, 50000, 100000};

    private List<double> values = new List<double> { 0.01, 0.10, 0.50, 1, 2, 5, 10, 50, 100, 250, 500, 750,
        1000, 1500, 2500, 5000, 7500, 10000, 12500, 15000, 20000, 25000, 50000, 100000};

    public List<double> Values
    {
        get => values;
        set
        {
            values = value;
            OnPropertyChanged(nameof(Values));
        }
    }

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
    private string _bankOfferText;
    public string BankOfferText
    {
        get => _bankOfferText;
        set
        {
            _bankOfferText = value;
            OnPropertyChanged(nameof(BankOfferText));
        }
    }

    private bool _isBankOfferVisible;
    public bool IsBankOfferVisible
    {
        get => _isBankOfferVisible;
        set
        {
            _isBankOfferVisible = value;
            OnPropertyChanged(nameof(IsBankOfferVisible));
        }
    }

    private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        set
        {
            _isGameOver = value;
            OnPropertyChanged(nameof(IsGameOver));
        }
    }

    private string _gameResult;
    public string GameResult
    {
        get => _gameResult;
        set
        {
            _gameResult = value;
            OnPropertyChanged(nameof(GameResult));
        }
    }

    private bool _isSelectingNewBox;
    public bool IsSelectingNewBox
    {
        get => _isSelectingNewBox;
        set
        {
            _isSelectingNewBox = value;
            OnPropertyChanged(nameof(IsSelectingNewBox));
        }
    }

    private bool _isSwappingBox;
    public bool IsSwappingBox
    {
        get => _isSwappingBox;
        set
        {
            _isSwappingBox = value;
            OnPropertyChanged(nameof(IsSwappingBox));
        }
    }

    private double _bankOfferValue;
    public double BankOfferValue
    {
        get => _bankOfferValue;
        set
        {
            _bankOfferValue = value;
            OnPropertyChanged(nameof(BankOfferValue));
        }
    }

    private string remainingBoxes;
    public string RemainingBoxes
    {
        get => remainingBoxes;
        set
        {
            remainingBoxes = value;
            OnPropertyChanged(nameof(RemainingBoxes));
        }
    }

    private bool isRoundOver;
    public bool IsRoundOver
    {
        get => isRoundOver;
        set
        {
            if (isRoundOver != value)
            {
                isRoundOver = value;
                OnPropertyChanged(nameof(IsRoundOver));
            }
        }
    }

    public ICommand DealCommand { get; }
    public ICommand NoDealCommand { get; }

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
            initialBoxValues.Remove(box.Value);
        }
        SelectOrOpenBoxCommand = new Command<int>(SelectOrOpenBox);
        DealCommand = new Command(HandleDeal);
        NoDealCommand = new Command(HandleNoDeal);
    }
    private void HandleDeal()
    {
        if (BankOfferText.Contains("смяна"))
        {
            IsSwappingBox = true; // Enter box swapping mode
            IsBankOfferVisible = false; // Hide the bank offer UI
        }
        else
        {
            EndGame(); // End the game with the bank offer value
        }
    }

    private void HandleNoDeal()
    {
        IsBankOfferVisible = false; // Hide the bank offer
        StartRound(); // Proceed to the next round
    }

    private void EndGame()
    {
        GameResult = $"Поздравления! Ти спечели: {BankOfferValue:C}";
        IsGameOver = true;
    }

    private void StartBankOffer()
    {
        IsBankOfferVisible = true;

        if (new Random().NextDouble() < 0.7) // Randomly choose the offer type
        {
            double averageValue = Boxes.Sum(b => b.Value) / Boxes.Count;
            BankOfferValue = Math.Round(averageValue);
            BankOfferText = $"Офертата на банката е: {BankOfferValue:C}";
        }
        else
        {
            BankOfferText = "Офертата на банката е: смяна на кутиите";
        }
    }

    private async void SelectOrOpenBox(int boxNumber)
    {
        // Handle initial player box selection
        if (!IsPlayerBoxSelected && !IsSwappingBox)
        {
            var selectedBox = Boxes.FirstOrDefault(b => b.Number == boxNumber);
            if (selectedBox != null)
            {
                selectedBox.IsPlayerBox = true;
                PlayerBox = selectedBox;
                IsPlayerBoxSelected = true;
                Boxes.Remove(selectedBox);

                StartGame(); // Proceed to the game
            }
        }
        // Handle box swapping during a bank offer
        else if (IsSwappingBox)
        {
            var newPlayerBox = Boxes.FirstOrDefault(b => b.Number == boxNumber);
            if (newPlayerBox != null)
            {
                // Return the previous player box to the list
                PlayerBox.IsPlayerBox = false; // Reset the old player box flag
                Boxes.Add(PlayerBox);

                // Set the new player box
                newPlayerBox.IsPlayerBox = true;
                PlayerBox = newPlayerBox;

                Boxes.Remove(newPlayerBox);

                IsSwappingBox = false; // End the swapping process

                // Proceed with the next steps after swapping
                StartRound(); // Resume the game
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
                RemainingBoxes = $"Кутии за отваряне: {BoxesToOpen}";
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
        isRoundOver = false;
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
        RemainingBoxes = $"Кутии за отваряне: {BoxesToOpen}";
    }

    private void EndRound()
    {
        isRoundOver = true;
        RoundNumber++;
        StartBankOffer();
    }

    private double GetRandomValue()
    {
        Random random = new Random();
        int randomIndex = random.Next(0, initialBoxValues.Count);

        return initialBoxValues[randomIndex];
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
