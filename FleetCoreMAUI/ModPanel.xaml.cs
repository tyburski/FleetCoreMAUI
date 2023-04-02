
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace FleetCoreMAUI;


public partial class ModPanel : ContentPage
{
    public ModPanel()
    {
        InitializeComponent();

        BindingContext = new ModViewModel();
    }
}



    



