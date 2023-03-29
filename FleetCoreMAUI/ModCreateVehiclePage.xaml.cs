
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace FleetCoreMAUI;


public partial class ModCreateVehiclePage : ContentPage
{


    public ModCreateVehiclePage()
    {
        InitializeComponent();

        BindingContext = new ModCreateVehicleViewModel();
        
    }
    

}



    



