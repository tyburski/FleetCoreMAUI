
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


}