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
    class LoginViewModel
    {
        public string _username = "";
        public string _password = "";

        public string username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                }
            }
        }
        public string password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                }
            }
        }
        public async Task<bool> Login()
        {
            using(var http = new HttpClient())
            {
                try
                {
                    string json = await Task.Run(() => JsonConvert.SerializeObject(new LoginModel()
                    {
                        UserName = username,
                        Password = password
                    }));

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/account/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (result != null)
                        {
                            var datas = JsonConvert.DeserializeObject<Dictionary<int, string>>(result);

                            await SecureStorage.Default.SetAsync("fullname", datas[0]);
                            await SecureStorage.Default.SetAsync("role", datas[1]);
                            await SecureStorage.Default.SetAsync("userId", datas[2]);
                            await SecureStorage.Default.SetAsync("notice", datas[3]);

                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Debug.Write(response.StatusCode);
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }          
        }
        public ICommand LoginCommand =>
        new Command(async () =>
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await Login() is true)
            {
                popup.Close();
                await Shell.Current.GoToAsync("//menu", true);
            } 
            else
            {
                popup.Close();
                await Application.Current.MainPage.DisplayAlert("BŁĄD", "Nieprawidłowa nazwa użytkownika \n lub hasło", "Ok");
            }
        });
    }
}
