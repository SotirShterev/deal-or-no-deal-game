using DealOrNoDealGame.ViewModels;

namespace DealOrNoDealGame;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
