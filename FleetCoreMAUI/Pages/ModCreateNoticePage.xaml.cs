
using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace FleetCoreMAUI;


public partial class ModCreateNoticePage : ContentPage
{
    public ModCreateNoticePage()
    {
        InitializeComponent();       
    }
    async void Button_Clicked(object sender, EventArgs args)
    {
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if (await CreateNotice() is true)
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("SUKCES", "Ogłoszenie utworzone pomyślnie", "Ok");
            await Shell.Current.GoToAsync("//menu/modpanel", true);
        }
        else
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
        }
    }
    async Task<bool> CreateNotice()
    {
        var id = await SecureStorage.Default.GetAsync("userId");
        string cont = Cont.Text;

        if (!cont.Equals(String.Empty))
        {
            using(var http = new HttpClient())
            {
                try
                {
                    var c = new CreateNoticeModel()
                    {
                        userId = id,
                        Content = cont
                    };
                    var json = JsonConvert.SerializeObject(c);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/organization/createnotice", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }           
        }
        else return false;
    }
}



    



