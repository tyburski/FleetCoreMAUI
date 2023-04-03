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
    class ModUpdateCompanyViewModel
    {
        public string _name = "";
        public string _address1 = "";
        public string _address2 = "";
        public string _nip = "";

        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }
        public string address1
        {
            get { return _address1; }
            set
            {
                if(_address1 != value)
                {
                    _address1 = value;
                }
            }
        }
        public string address2
        {
            get { return _address2; }
            set
            {
                if (_address2 != value)
                {
                    _address2 = value;
                }
            }
        }
        public string nip
        {
            get { return _nip; }
            set
            {
                if (_nip != value)
                {
                    _nip = value;
                }
            }
        }

        

        public async Task<bool> Update()
        {
            using(var http = new HttpClient())
            {
                try
                {
                    string json = await Task.Run(() => JsonConvert.SerializeObject(new UpdateCompanyModel()
                    {
                        Name = name,
                        Address1 = address1,
                        Address2 = address2,
                        NIP = nip
                    }));

                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/organization/update", content);

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
        public ICommand UpdateCompanyCommand =>
        new Command(async () =>
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await Update() is true)
            {
                popup.Close();
                await App.Current.MainPage.DisplayAlert("SUKCES", "Zmiany zostały zapisane", "Ok");
                await Shell.Current.GoToAsync("..");
            } 
            else
            {
                popup.Close();
                await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
            }
        });
    }
}
