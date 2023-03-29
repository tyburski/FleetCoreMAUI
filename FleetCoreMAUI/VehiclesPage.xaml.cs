using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace FleetCoreMAUI;

public partial class VehiclesPage : ContentPage
{
    public VehiclesPage()
    {
        InitializeComponent();
        
        GetVehicles();
        //BindingContext = new VehiclesViewModel();
    }

    async void RefuelClicked(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var plate = button.ClassId;

        string result = await DisplayPromptAsync($"{plate}:Tankowanie", "Podaj ilość litrów:", "Dalej","Anuluj", keyboard: Keyboard.Numeric);
        if(result!=null)
        {
            string result2 = await DisplayPromptAsync($"{plate}:Tankowanie", "Podaj przebieg:", "Prześlij", "Anuluj", keyboard: Keyboard.Numeric);
            if(result2!=null)
            {
                bool resParse = Double.TryParse(result, out double resultParsed);
                bool res2Parse = Int64.TryParse(result2, out long result2Parsed);

                

                if (resParse && res2Parse)
                {
                    var refuel = new RefuelModel()
                    {
                        Plate = plate.ToString(),
                        Mileage = result2Parsed,
                        Quantity = resultParsed,
                        userId = SecureStorage.Default.GetAsync("userId").Result
                        

                    };
                    var popup = new Spinner();
                    Application.Current.MainPage.ShowPopup(popup);
                    if (await Refuel(refuel) is true)
                    {
                        popup.Close();
                        await Application.Current.MainPage.DisplayAlert(null, $"Pomyślnie zapisano tankowanie dla pojazdu {plate}", "Ok");
                        
                    }
                    else
                    {
                        popup.Close();
                        Application.Current.MainPage.DisplayAlert(null, "Błąd", "Ok");
                    }
                }
                
            }
        }
        
    }
    async Task<bool> Refuel(RefuelModel model)
    {
        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
        var http = devSslHelper.HttpClient;

        try
        {
            var json = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PostAsync(devSslHelper.DevServerRootUrl + "/api/vehicle/refueling", content);


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
    async void OpenMenu(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var plate = button.ClassId;
        string action = await App.Current.MainPage.DisplayActionSheet($"{plate}:Menu pojazdu", "Anuluj", null, "Wydarzenia", "Bieżące naprawy", "Historia napraw");
    }
    public async Task GetVehicles()
    {
        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
        var http = devSslHelper.HttpClient;
        try
        {
            var response = await http.GetAsync(devSslHelper.DevServerRootUrl + "/api/vehicle");
            var result = await response.Content.ReadAsStringAsync();
            var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(result);
            VehicleList.ItemsSource = vehicles.ToList();
           
        }
        catch
        {

        }
    }
   

}