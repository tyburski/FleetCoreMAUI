
using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace FleetCoreMAUI;

public partial class ModUsersPage : ContentPage
{
	public ModUsersPage()
	{
		InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if(await GetUsers() is true)
        {
            popup.Close();
        }
        else
        {
            popup.Close();
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
            await Shell.Current.GoToAsync("..");
        }       
    }

    public async Task<bool> GetUsers()
    {
        using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/account/getall");
                var result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(result);
                list.ItemsSource = users.ToList();
                return true;
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
        var fullName = button.ClassId;
        var role = button.AutomationId;

        string option = "";
        string action = String.Empty;

        var userRole = await SecureStorage.Default.GetAsync("role");

        if (userRole.Equals("Owner"))
        {
            if (role.Equals("User"))
            {
                option = $"Zmień na 'Moderator'";
                action = await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło", option, "Usuń użytkownika");
            }
            else if (role.Equals("Moderator"))
            {
                option = $"Zmień na 'User'";
                action = await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło", option, "Usuń użytkownika");
            }
            else if (role.Equals("Owner"))
            {
                action = await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło");
            }
        }
        else if (userRole.Equals("Moderator"))
        {
            if (role.Equals("User") || role.Equals("Moderator"))
            {
                action = await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl tankowania", "Resetuj hasło", "Usuń użytkownika");
            }
            else if (role.Equals("Owner"))
            {
                action = await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl tankowania", "Resetuj hasło");
            }
        }
        else if (userRole.Equals("Deleted"))
        {
            await App.Current.MainPage.DisplayAlert($"{fullName}:Menu", "Użytkownik usunięty", "Ok");
        }

        if (action!=null)
        {
            if(!action.Equals(String.Empty))
            {
                if(action.Equals("Wyświetl bonusy"))
                {
                    GetBonuses(fullName);
                }
                else if(action.Equals("Wyświetl tankowania"))
                {
                    GetRefuelings(fullName);
                }
                else if(action.Equals("Resetuj hasło"))
                {
                    ResetPassword(fullName);
                }
                else if(action.Equals("Usuń użytkownika"))
                {
                    DeleteUser(fullName);
                }
                else if(action.Equals("Zmień na 'Moderator'") || action.Equals("Zmień na 'User'"))
                {
                    ChangeRole(fullName);
                }
            }
        }
    }

    async void GetBonuses(string fullname)
    {
        await Shell.Current.GoToAsync($"//menu/modpanel/modUsers/bonus?fullName={fullname}");
    }
    async void GetRefuelings(string fullname)
    {
        await Shell.Current.GoToAsync($"//menu/modpanel/modUsers/refuel?fullName={fullname}");
    }
    async void ResetPassword(string fullname)
    {
        if (await Application.Current.MainPage.DisplayAlert("Jesteś pewny?", $"Czy chcesz zresetować hasło użytkownika {fullname}?", "Ok", "Anuluj"))
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);

            using(var http = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(fullname);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/account/rpassword", content);

                    if (response.IsSuccessStatusCode)
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("SUKCES", "Hasło zostało zresetowane", "Ok");
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
    async void DeleteUser(string fullname)
    {
        if(await Application.Current.MainPage.DisplayAlert("Jesteś pewny?", $"Czy chcesz usunąc użytkownika {fullname}?", "Ok","Anuluj"))
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);

            using(var http = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(fullname);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/account/delete", content);

                    if (response.IsSuccessStatusCode)
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("SUKCES", "Użytkownik został usunięty", "Ok");
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
    async void ChangeRole(string fullname)
    {
        if (await Application.Current.MainPage.DisplayAlert("Jesteś pewny?", $"Czy chcesz zmienić rolę użytkownika {fullname}?", "Ok", "Anuluj"))
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);

            using(var http = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(fullname);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/account/role", content);

                    if (response.IsSuccessStatusCode)
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("SUKCES", $"Rola użytkownika {fullname} została zmieniona", "Ok");
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