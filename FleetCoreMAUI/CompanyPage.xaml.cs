using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FleetCoreMAUI;

public partial class CompanyPage : ContentPage
{
	public CompanyPage()
	{
		InitializeComponent();
        Get();
	}
    public async Task Get()
    {
        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7003);
        var http = devSslHelper.HttpClient;
        try
        {
            var response = await http.GetAsync(devSslHelper.DevServerRootUrl + "/api/organization/get");
            var result = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(result);

            Name.Text = company.Name.ToUpper();
            Address1.Text = company.Address1;
            Address2.Text = company.Address2;
            NIP.Text = $"NIP: {company.NIP}";
        }
        catch
        {

        }
    }
}