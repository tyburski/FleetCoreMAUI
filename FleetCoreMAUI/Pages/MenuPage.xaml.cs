
using FleetCoreMAUI.Models;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace FleetCoreMAUI;


public partial class MenuPage : ContentPage
{

    public int isEndingCounter = 0;
    public DateTime lastNoticeDate;
    public bool noticesExist = false;
    public MenuPage()
    {
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new MenuViewModel();
        isEndingCounter = 0;
        ModButton();
        await GetVehicles();
        if (isEndingCounter > 0)
        {
            warning.IsVisible = true;
        }
        else warning.IsVisible = false;

        noticesExist = false;
        await GetNotices();
        if (!lastNoticeDate.ToString("dd/MM/yyyy HH:mm").Equals(await SecureStorage.Default.GetAsync("notice")) && noticesExist)
        {
            notice.IsVisible = true;
        }
        else notice.IsVisible = false;

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
    public async Task GetVehicles()
    {
        using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/vehicle");
                var result = await response.Content.ReadAsStringAsync();
                var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(result);

                foreach (Vehicle v in vehicles)
                {
                    foreach (Event e in v.Events)
                    {
                        var checkDate = e.Date.Subtract(DateTime.Now);

                        if (checkDate.TotalDays <= 30 || e.Date <= DateTime.Now)
                        {
                            isEndingCounter++;
                            Debug.WriteLine(e.Date);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
    public async Task GetNotices()
    {
        using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/organization/getnotices");

                var resString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Notice>>(resString);

                if (response.IsSuccessStatusCode)
                {
                    if (result.Count > 0)
                    {
                        foreach (Notice n in result)
                        {
                            n.ConvertedDate = n.CreatedAt.ToString("dd/MM/yyyy");
                        }
                        var l = result.OrderByDescending(x => x.CreatedAt);
                        lastNoticeDate = l.First().CreatedAt;
                        noticesExist= true;
                    }
                    else noticesExist = false;                  
                }
            }
            catch
            {

            }
        }
    }
}



    



