
using FleetCoreMAUI.Models;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace FleetCoreMAUI;


public partial class MenuPage : ContentPage
{


    public MenuPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        BindingContext = new MenuViewModel();
        ModButton();
    }
    async Task ModButton()
    {
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        var role = await SecureStorage.Default.GetAsync("role");

        Version.Text = $"ver: {version}";

        if (role.Equals("Owner") || role.Equals("Moderator"))
        {
            modButton.IsVisible = true;
            Version.IsVisible = false;
        }
        else
        {
            modButton.IsVisible = false;
            Version.IsVisible = true;
        }
        
    }

    

    

}



    



