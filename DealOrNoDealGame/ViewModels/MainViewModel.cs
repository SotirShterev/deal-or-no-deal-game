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

    private Dictionary<double, Color> values = new Dictionary<double, Color>
        {
            { 0.01, Colors.Gold },
            { 0.10, Colors.Gold },
            { 0.50, Colors.Gold },
            { 1, Colors.Gold },
            { 2, Colors.Gold },
            { 5, Colors.Gold },
            { 10, Colors.Gold },
            { 50, Colors.Gold },
            { 100, Colors.Gold },
            { 250, Colors.Gold },
            { 500, Colors.Gold },
            { 750, Colors.Gold },
            { 1000, Colors.Gold },
            { 1500, Colors.Gold },
            { 2500, Colors.Gold },
            { 5000, Colors.Gold },
            { 7500, Colors.Gold },
            { 10000, Colors.Gold },
            { 12500, Colors.Gold },
            { 15000, Colors.Gold },
            { 20000, Colors.Gold },
            { 25000, Colors.Gold },
            { 50000, Colors.Gold },
            { 100000, Colors.Gold }
        };

    public List<KeyValuePair<double, Color>> Values => values.ToList();

    private bool isPlayerBoxSelected;
    public bool IsPlayerBoxSelected
    {
        get => isPlayerBoxSelected;
        set
        {
            isPlayerBoxSelected = value;
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

    private Box playerBox;
    public Box PlayerBox
    {
        get => playerBox;
        set
        {
            playerBox = value;
            OnPropertyChanged(nameof(PlayerBox));
        }
    }

    private string bankOfferText;
    public string BankOfferText
    {
        get => bankOfferText;
        set
        {
            bankOfferText = value;
            OnPropertyChanged(nameof(BankOfferText));
        }
    }

    private bool isBankOfferVisible;
    public bool IsBankOfferVisible
    {
        get => isBankOfferVisible;
        set
        {
            isBankOfferVisible = value;
            OnPropertyChanged(nameof(IsBankOfferVisible));
        }
    }

    private bool isGameOver;
    public bool IsGameOver
    {
        get => isGameOver;
        set
        {
            isGameOver = value;
            OnPropertyChanged(nameof(IsGameOver));
        }
    }

    private string gameResult;
    public string GameResult
    {
        get => gameResult;
        set
        {
            gameResult = value;
            OnPropertyChanged(nameof(GameResult));
        }
    }

    private bool isSelectingNewBox;
    public bool IsSelectingNewBox
    {
        get => isSelectingNewBox;
        set
        {
            isSelectingNewBox = value;
            OnPropertyChanged(nameof(IsSelectingNewBox));
        }
    }

    private bool isSwappingBox;
    public bool IsSwappingBox
    {
        get => isSwappingBox;
        set
        {
            isSwappingBox = value;
            OnPropertyChanged(nameof(IsSwappingBox));
        }
    }

    private double bankOfferValue;
    public double BankOfferValue
    {
        get => bankOfferValue;
        set
        {
            bankOfferValue = value;
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

    private Color valueColor = Color.FromRgba("#f8c21b");
    public Color ValueColor
    {
        get => valueColor;
        set
        {
            if (valueColor != value)
            {
                valueColor = value;
                OnPropertyChanged(nameof(ValueColor));
            }
        }
    }

    public ICommand DealCommand { get; }
    public ICommand NoDealCommand { get; }

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
            IsSwappingBox = true;
            IsBankOfferVisible = false;

            if (RoundNumber == 8)
            {
                EndGame();
            }
        }
        else
        {
            GameResult = $"Поздравления! Ти спечели: {BankOfferValue:C}";
            IsGameOver = true;
        }
    }

    private void HandleNoDeal()
    {
        IsBankOfferVisible = false;
        if (RoundNumber == 8)
        {
            EndGame();
        }
        StartRound();
    }

    private async void EndGame()
    {
        GameResult = $"Във вашата кутия има:";
        await Task.Delay(3000);
        IsGameOver = true;
    }

    private void StartBankOffer()
    {
        IsBankOfferVisible = true;

        if (new Random().NextDouble() < 0.7)
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
        if (!IsPlayerBoxSelected && !IsSwappingBox)
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
        else if (IsSwappingBox)
        {
            var newPlayerBox = Boxes.FirstOrDefault(b => b.Number == boxNumber);
            if (newPlayerBox != null)
            {
                PlayerBox.IsPlayerBox = false;
                Boxes.Add(PlayerBox);

                newPlayerBox.IsPlayerBox = true;
                PlayerBox = newPlayerBox;

                Boxes.Remove(newPlayerBox);

                IsSwappingBox = false;

                StartRound();
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

                if (values.ContainsKey(boxToOpen.Value))
                {
                    values[boxToOpen.Value] = Colors.Brown;
                    OnPropertyChanged(nameof(Values));
                }

                boxToOpen.OnPropertyChanged(nameof(Box.DisplayText));
                boxToOpen.OnPropertyChanged(nameof(Box.BackgroundColor));

                await Task.Delay(2000);

                Boxes.Remove(boxToOpen);
                BoxesToOpen--;
                RemainingBoxes = $"Кутии за отваряне: {BoxesToOpen}";


                if (BoxesToOpen == 0)
                {
                    EndRound();
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
