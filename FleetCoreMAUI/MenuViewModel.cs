
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
        public ICommand LogOutCommand => new Command(async()=> await LogOut());
        public ICommand ModPanelCommand => new Command(ModPanel);

        private async void Notice()
        {
            await App.Current.MainPage.Navigation.PushAsync(new NoticePage());
        }
        private async void Vehicles()
        {
            await App.Current.MainPage.Navigation.PushAsync(new VehiclesPage());
        }
        private async void Bonus()
        {
            string Month = $"{SecureStorage.GetAsync("fullname").Result.ToUpper()}\nMIESIĄC:  {DateTime.Now.ToString("MM")}-{DateTime.Now.ToString("yyyy")}";
            string bon = await Application.Current.MainPage.DisplayPromptAsync(Month, "Wpisz bonus:", "Prześlij", "Anuluj", keyboard: Keyboard.Text);
            if(bon!=null)
            {

                var popup = new Spinner();
                Application.Current.MainPage.ShowPopup(popup);
                if (await CreateBonus(bon) is true)
                {
                    popup.Close();
                    await Application.Current.MainPage.DisplayAlert(null, "Pomyślnie dodano bonus", "Ok");

                }
                else
                {
                    popup.Close();
                    Application.Current.MainPage.DisplayAlert(null, "Błąd", "Ok");
                }
            }
        }
        private async void Company()
        {
            await App.Current.MainPage.Navigation.PushAsync(new CompanyPage());
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
