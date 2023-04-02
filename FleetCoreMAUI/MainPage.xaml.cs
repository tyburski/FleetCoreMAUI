
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

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
        var notice = await SecureStorage.Default.GetAsync("notice");

        if (id != null && fullname != null && role != null && notice != null)
        {
            using (var http = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(id);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/account/validate", content);

                    if (response.IsSuccessStatusCode)
                    {
                        
                        return true;
                    }
                    else return false;
                }
                catch
                {
                    
                    return false;
                }
            }          
        }
        else return false;
    }
}



    



