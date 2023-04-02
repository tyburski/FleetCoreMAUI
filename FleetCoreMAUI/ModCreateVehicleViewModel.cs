using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FleetCoreMAUI
{
    class ModCreateVehicleViewModel
    {
        public string _plate = "";
        public long? _mileage = null;
        public string _vin = "";

        public string plate
        {
            get { return _plate; }
            set
            {
                if (_plate != value)
                {
                    _plate = value;
                }
            }
        }
        public long? mileage
        {
            get { return _mileage; }
            set
            {
                if(_mileage != value)
                {
                    _mileage = value;
                }
            }
        }
        public string vin
        {
            get { return _vin; }
            set
            {
                if (_vin != value)
                {
                    _vin = value;
                }
            }
        }
        public async Task<bool> Create()
        {
            using(var http = new HttpClient())
            {
                try
                {
                    string json = await Task.Run(() => JsonConvert.SerializeObject(new CreateVehicleModel()
                    {
                        Plate = plate,
                        Mileage = mileage,
                        VIN = vin
                    }));

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/vehicle/create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }              
        }
        public ICommand CreateVehicleCommand =>
        new Command(async () =>
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await Create() is true)
            {
                popup.Close();
                await App.Current.MainPage.DisplayAlert(null, "Pojazd utworzony pomyślnie", "Ok");
                await Shell.Current.GoToAsync("..");
            } 
            else
            {
                popup.Close();
                await Application.Current.MainPage.DisplayAlert(null, "Błąd", "Ok");
            }
        });
    }
}
