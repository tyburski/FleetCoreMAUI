using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FleetCoreMAUI
{
    internal class BonusViewModel
    {
        public BonusViewModel() 
        {
            GetMonth();
        }
        public string _BonusContent = "";
        public string _Month = "";
        public string BonusContent
        {
            get { return _BonusContent; }
            set
            {
                if (_BonusContent != value)
                {
                    _BonusContent = value;
                }
            }
        }
        public string Month
        {
            get { return _Month; }
            set
            {
                if (_Month != value)
                {
                    _Month = value;
                }
            }
        }
        void GetMonth()
        {
            _Month = $"{SecureStorage.GetAsync("fullname").Result.ToUpper()}\nMIESIĄC:  {DateTime.Now.ToString("MM")}-{DateTime.Now.ToString("yyyy")}";
        }
        public async Task<bool> CreateBonus()
        {
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
            var http = devSslHelper.HttpClient;
            var user =  SecureStorage.GetAsync("userId").Result;
            Debug.WriteLine(user);

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
        public ICommand CreateBonusCommand =>
        new Command(async () =>
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await CreateBonus() is true)
            {
                popup.Close();
                await Application.Current.MainPage.DisplayAlert(null, "Pomyślnie dodano bonus", "Ok");
                await Shell.Current.GoToAsync("///menu");
            }
            else
            {
                popup.Close();
                Application.Current.MainPage.DisplayAlert(null, "Błąd", "Ok");
            }
        });
    }
}
