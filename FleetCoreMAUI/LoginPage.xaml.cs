
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace FleetCoreMAUI;


public partial class LoginPage : ContentPage
{


    public LoginPage()
    {
        InitializeComponent();

        BindingContext = new LoginViewModel();
        
    }
    

}



    



