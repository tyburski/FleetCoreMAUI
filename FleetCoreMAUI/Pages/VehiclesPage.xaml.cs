using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace FleetCoreMAUI;

public partial class VehiclesPage : ContentPage
{
    ObservableCollection<VehicleViewModel> list { get; set; } = new();

    public VehiclesPage()
    {
        InitializeComponent();       
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Render();
    }
    async Task Render()
    {
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);

        if (await GetVehicles() is true)
        {
            ColList.ItemsSource = list;
            await Task.Delay(1000);
            popup.Close();
        }
    }
    async void RefuelClicked(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var plate = button.ClassId;
        var mileage = button.AutomationId;

        string result = await DisplayPromptAsync($"{plate}:Tankowanie", "Podaj litry:", "Dalej","Anuluj");
        if(result!=null)
        {
            if (!result.Equals(String.Empty))
            {
                string result2 = await DisplayPromptAsync($"{plate}:Tankowanie", "Podaj przebieg:", "Prześlij", "Anuluj");
                if (result2!=null)
                {
                    if (!result2.Equals(String.Empty))
                    {
                        bool resParse = Double.TryParse(result, out double resultParsed);
                        bool res2Parse = Int64.TryParse(result2, out long result2Parsed);
                        Int64.TryParse(mileage, out long mileageParsed);

                        if (resParse && res2Parse)
                        {

                            if (result2Parsed > mileageParsed)
                            {
                                var popup = new Spinner();
                                Application.Current.MainPage.ShowPopup(popup);
                                var refuel = new RefuelModel()
                                {
                                    Plate = plate.ToString(),
                                    Mileage = result2Parsed,
                                    Quantity = resultParsed,
                                    userId = SecureStorage.Default.GetAsync("userId").Result
                                };

                                if (await Refuel(refuel) is true)
                                {
                                    popup.Close();
                                    await Application.Current.MainPage.DisplayAlert("SUKCES", $"Pomyślnie zapisano tankowanie dla pojazdu {plate}", "Ok");
                                    OnAppearing();
                                }
                                else
                                {
                                    popup.Close();
                                    await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("BŁĄD", "Podany przebieg jest \n niższy od aktualnego", "Ok");
                            }

                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Błędne wartości", "Ok");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("BŁĄD", "Pole nie może być puste", "Ok");
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("BŁĄD", "Pole nie może być puste", "Ok");
            }
        }       
    }
    async Task<bool> Refuel(RefuelModel model)
    {
        using(var http = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync("https://primasystem.pl/api/vehicle/refueling", content);

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
    async void OpenMenu(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var plate = button.ClassId;

        var role = SecureStorage.GetAsync("role").Result;
        if (role.Equals("User"))
        {
                await Shell.Current.GoToAsync($"vehicleDetails?plate={plate}");
        }
        else
        {
            string action = await App.Current.MainPage.DisplayActionSheet($"{plate}:Menu pojazdu", "Anuluj", null, "Szczegóły pojazdu", "Edytuj numer rej.","Edytuj przebieg", "Usuń pojazd");
            if(action!=null)
            {
                if (action.Equals("Szczegóły pojazdu"))
                {
                    await Shell.Current.GoToAsync($"vehicleDetails?plate={plate}");
                }
                else if (action.Equals("Edytuj numer rej."))
                {
                    await UpdateVehicle(plate);
                }
                else if (action.Equals("Edytuj przebieg"))
                {
                    await UpdateMileage(plate);
                }
                else if (action.Equals("Usuń pojazd"))
                {
                    await DeleteVehicle(plate);
                }
            }          
        }   
    }
    public async Task<bool> GetVehicles()
    {
       using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/vehicle");
                var result = await response.Content.ReadAsStringAsync();
                var vehicles = JsonConvert.DeserializeObject<List<VehicleViewModel>>(result);


                foreach (VehicleViewModel v in vehicles)
                {
                    foreach (Event e in v.Events)
                    {
                        var checkDate = e.Date.Subtract(DateTime.Now);

                        if (checkDate.TotalDays <= 14)
                        {
                            v.isWarning = true;
                        }
                        else v.isWarning = false;
                    }
                    foreach (Repair r in v.Repairs)
                    {
                        if (r.UserFinished is null)
                        {
                            v.isToRepair = true;
                        }
                        else v.isToRepair = false;
                    }
                }

                list = new ObservableCollection<VehicleViewModel>(vehicles.OrderBy(x => x.Plate));


                return true;

            }
            catch
            {
                return false;
            }
        }       
    }
    public async Task UpdateVehicle(string plate)
    {
        string result = await DisplayPromptAsync($"{plate}:Zmiana Numeru", "Podaj nowy numer rejestracyjny:", "Prześlij", "Anuluj");
        if(result!=null)
        {
            if(!result.Equals(String.Empty))
            {
                if (plate != null)
                {
                    var popup = new Spinner();
                    Application.Current.MainPage.ShowPopup(popup);

                    using(var http = new HttpClient())
                    {
                        try
                        {
                            var veh = new UpdateVehicleModel()
                            {
                                Plate = plate,
                                newPlate = result.ToUpper()

                            };
                            var json = JsonConvert.SerializeObject(veh);
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await http.PostAsync("https://primasystem.pl/api/vehicle/update", content);


                            if (response.IsSuccessStatusCode)
                            {
                                popup.Close();
                                await App.Current.MainPage.DisplayAlert("SUKCES", "Zmiany zostały zapisane", "Ok");
                                OnAppearing();
                            }
                            else
                            {
                                popup.Close();
                                await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                            }
                        }
                        catch
                        {
                            popup.Close();
                            await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                        }
                    }                    
                }       
            }
        }
    }
    public async Task UpdateMileage(string plate)
    {
        string result = await DisplayPromptAsync($"{plate}:Zmiana Przebiegu", "Podaj nowy przebieg:", "Prześlij", "Anuluj", keyboard:Keyboard.Numeric);
        if (result != null)
        {
            if (!result.Equals(String.Empty))
            {
                var popup = new Spinner();
                Application.Current.MainPage.ShowPopup(popup);
                if (Int64.TryParse(result, out long resultParsed))
                {
                    if (plate != null)
                    {
                        using(var http = new HttpClient())
                        {
                            try
                            {
                                var veh = new UpdateMileageModel()
                                {
                                    Plate = plate.ToUpper(),
                                    Mileage = resultParsed

                                };
                                var json = JsonConvert.SerializeObject(veh);
                                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await http.PostAsync("https://primasystem.pl/api/vehicle/mileage", content);


                                if (response.IsSuccessStatusCode)
                                {
                                    popup.Close();
                                    await App.Current.MainPage.DisplayAlert("SUKCES", "Zmiany zostały zapisane", "Ok");
                                    OnAppearing();
                                }
                                else
                                {
                                    popup.Close();
                                    await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                                }
                            }
                            catch
                            {
                                popup.Close();
                                await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                            }
                        }                        
                    }
                    else
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                    }

                }
                else
                {
                    popup.Close();
                    await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                }
            }
        }
    }
    public async Task DeleteVehicle(string plate)
    {
        if(plate!=null)
        {
            if(await Application.Current.MainPage.DisplayAlert("Jesteś pewny?", $"Czy chcesz usunąć pojazd {plate}?","Tak","Anuluj"))
            {
                var popup = new Spinner();
                Application.Current.MainPage.ShowPopup(popup);

                using(var http = new HttpClient())
                {
                    try
                    {
                        var json = JsonConvert.SerializeObject(plate);
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await http.PostAsync("https://primasystem.pl/api/vehicle/delete", content);


                        if (response.IsSuccessStatusCode)
                        {
                            popup.Close();
                            await App.Current.MainPage.DisplayAlert("SUKCES", "Pojazd został usunięty", "Ok");
                            OnAppearing();
                        }
                        else
                        {
                            popup.Close();
                            await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                        }
                    }
                    catch
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                    }
                }               
            }            
        }
    }
}