
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace FleetCoreMAUI;


public partial class ModCreateUserPage : ContentPage
{
    public ModCreateUserPage()
    {
        InitializeComponent();

        BindingContext = new ModCreateUserViewModel();        
    }
}



    



