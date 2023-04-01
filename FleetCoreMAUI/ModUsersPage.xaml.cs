
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FleetCoreMAUI;

public partial class ModUsersPage : ContentPage
{
	public ModUsersPage()
	{
		InitializeComponent();
        GetUsers();

    }

    public async Task GetUsers()
    {
        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
        var http = devSslHelper.HttpClient;
        try
        {
            var response = await http.GetAsync(devSslHelper.DevServerRootUrl + "/api/account/getall");
            var result = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(result);
            list.ItemsSource = users.ToList();
        }
        catch
        {

        }
    }
    async void OpenMenu(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var fullName = button.ClassId;
        var role = button.AutomationId;

        string option = "";
        string action = "";
        if(role.Equals("User"))
        {
            option = $"Zmień na 'Moderator'";
            await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło", option, "Usuń użytkownika");
        }
        else if(role.Equals("Moderator"))
        {
            option = $"Zmień na 'User'";
            await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło", option, "Usuń użytkownika");
        }
        else if(role.Equals("Owner"))
        {
            await App.Current.MainPage.DisplayActionSheet($"{fullName}:Menu", "Anuluj", null, "Wyświetl bonusy", "Wyświetl tankowania", "Resetuj hasło");
        }
    }


}