using FleetCoreMAUI.Models;
using GoogleGson;
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
        public async Task Login()
        {
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
            var http = devSslHelper.HttpClient;

            try
            {
                string json = await Task.Run(() => JsonConvert.SerializeObject( new LoginModel()
                {
                    UserName = username,
                    Password = password
                }));
                
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(devSslHelper.DevServerRootUrl + "/api/account/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Log in success");
                    Debug.WriteLine(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }            
        }
        public ICommand LoginCommand =>
        new Command(async () =>
        {
            await Login();
        });
    }
}
