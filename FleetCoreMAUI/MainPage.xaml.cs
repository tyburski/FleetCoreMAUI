
using FleetCoreMAUI.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace FleetCoreMAUI;


public partial class MainPage : ContentPage
{


    public MainPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();

    }

}



    



