
using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FleetCoreMAUI;

public partial class ModLogs : ContentPage
{
	public ModLogs()
	{
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if(await GetLogs() is true)
        {
            popup.Close();
        }
        else
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("Błąd", "Spróbuj ponownie", "Ok");
        }
    }
    public async Task<bool> GetLogs()
    {
       using(var http = new HttpClient())
       {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/account/getlogs");
                var result = await response.Content.ReadAsStringAsync();
                var logs = JsonConvert.DeserializeObject<List<Log>>(result);

                foreach (var l in logs)
                {
                    l.ConvertedDate = l.Date.ToString("dd/MM/yyyy HH:mm");
                }
                list.ItemsSource = logs.ToList();
                return true;
            }
            catch
            {
                return false;
            }
       }        
    }
    async void Refresh(object sender, EventArgs args)
    {
        OnAppearing();
    }
}