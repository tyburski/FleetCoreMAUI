
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
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
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (await isAuthenticated())
        {
            await Shell.Current.GoToAsync("//menu");
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
        base.OnNavigatedTo(args);
    }

    async Task<bool> isAuthenticated()
    {
        await Task.Delay(2000);
        var id = await SecureStorage.Default.GetAsync("userId");
        var fullname = await SecureStorage.Default.GetAsync("fullname");
        var role = await SecureStorage.Default.GetAsync("role");

        if (id != null && fullname != null && role != null)
        {
            return true;
        }
        else return false;

    }
}



    



