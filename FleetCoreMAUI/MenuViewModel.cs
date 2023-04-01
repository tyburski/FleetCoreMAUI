
using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FleetCoreMAUI
{
    internal class MenuViewModel
    {
        public MenuViewModel()
        {
            GetDatas();
        }
        public string fullName = string.Empty;

        public ICommand NoticeCommand => new Command(Notice);
        public ICommand VehiclesCommand => new Command(Vehicles);
        public ICommand BonusCommand => new Command(Bonus);
        public ICommand CompanyCommand => new Command(Company);
        public ICommand ChangePasswordCommand => new Command(async()=>await ChangePassword());
        public ICommand LogOutCommand => new Command(async()=> await LogOut());
        public ICommand ModPanelCommand => new Command(ModPanel);

        private async void Notice()
        {
            await App.Current.MainPage.Navigation.PushAsync(new NoticePage());
        }
        private async void Vehicles()
        {
            await Shell.Current.GoToAsync("//menu/vehicles");
        }
        private async void Bonus()
        {
            string Month = $"{SecureStorage.GetAsync("fullname").Result.ToUpper()}\nMIESIĄC:  {DateTime.Now.ToString("MM")}-{DateTime.Now.ToString("yyyy")}";
            string bon = await Application.Current.MainPage.DisplayPromptAsync(Month, "Wpisz bonus:", "Prześlij", "Anuluj");
            if(bon!=null)
            {
                if (bon.Equals(String.Empty)) await Application.Current.MainPage.DisplayAlert("BŁĄD", "Pole nie może być puste", "Ok");
                else
                {
                    if (await App.Current.MainPage.DisplayAlert(Month, $"Czy chcesz dodać bonus '{bon}'?", "Ok", "Anuluj"))
                    {
                        var popup = new Spinner();
                        Application.Current.MainPage.ShowPopup(popup);
                        if (await CreateBonus(bon) is true)
                        {
                            popup.Close();
                            await Application.Current.MainPage.DisplayAlert("SUKCES", "Pomyślnie dodano bonus", "Ok");
                        }
                        else
                        {
                            popup.Close();
                            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                        }
                    }
                }
            }                   
        }
        private async void Company()
        {
            await App.Current.MainPage.Navigation.PushAsync(new CompanyPage());
        }
        private async Task ChangePassword()
        {
            if (await App.Current.MainPage.DisplayAlert("Jesteś pewny?", "Czy chcesz zmienić hasło?", "Ok", "Anuluj"))
            {
                var user = await SecureStorage.GetAsync("fullname");
                var id = await SecureStorage.GetAsync("userId");

                string password1 = await App.Current.MainPage.DisplayPromptAsync($"{user}:Zmiana hasła", "Podaj nowe hasło:", "Dalej", "Anuluj");
                if(password1!=null)
                {
                    if (!password1.Equals(String.Empty))
                    {
                        string password2 = await App.Current.MainPage.DisplayPromptAsync($"{user}:Zmiana hasła", "Powtórz hasło:", "Potwierdź", "Anuluj");
                        if(password2!=null)
                        {
                            if (!password2.Equals(String.Empty))
                            {
                                if (password1.Equals(password2))
                                {
                                    var popup = new Spinner();
                                    Application.Current.MainPage.ShowPopup(popup);
                                    var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
                                    var http = devSslHelper.HttpClient;

                                    var password = new ChangePasswordModel()
                                    {
                                        userId = id,
                                        Password = password1
                                    };
                                    try
                                    {
                                        var json = JsonConvert.SerializeObject(password);
                                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                                        var response = await http.PostAsync(devSslHelper.DevServerRootUrl + "/api/account/password", content);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            popup.Close();
                                            await App.Current.MainPage.DisplayAlert("SUKCES", "Hasło zostało zmienione", "Ok");
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
                                else await App.Current.MainPage.DisplayAlert("BŁĄD", "Podane hasła nie są takie same", "Ok");
                            }
                            else await App.Current.MainPage.DisplayAlert("BŁĄD", "Pole nie może być puste", "Ok");
                        }                       
                    }
                    else await App.Current.MainPage.DisplayAlert("BŁĄD", "Pole nie może być puste", "Ok");
                }
                
            }
        }
        private async void ModPanel()
        {
            await Shell.Current.GoToAsync("//menu/modpanel");
        }
        private async Task LogOut()
        {
            if (await App.Current.MainPage.DisplayAlert("Jesteś pewny?", "Zostaniesz wylogowany", "Ok", "Anuluj"))
            {
                SecureStorage.Default.RemoveAll();
                
                if (await SecureStorage.Default.GetAsync("userId") is null &&
                    await SecureStorage.Default.GetAsync("fullname") is null &&
                    await SecureStorage.Default.GetAsync("role") is null)
                {
                    await Shell.Current.GoToAsync("//login");
                }
                
            }           
        }
        public async Task<bool> CreateBonus(string BonusContent)
        {
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
            var http = devSslHelper.HttpClient;
            var user = SecureStorage.GetAsync("userId").Result;

            try
            {

                var bonus = await Task.Run(() => JsonConvert.SerializeObject(new CreateBonusModel()
                {
                    userId = user,
                    content = BonusContent

                }));
                StringContent content = new StringContent(bonus, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(devSslHelper.DevServerRootUrl + "/api/bonus/create", content);


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
        async Task GetDatas()
        {
            fullName =  SecureStorage.Default.GetAsync("fullname").Result;              
        }
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
    }
}
