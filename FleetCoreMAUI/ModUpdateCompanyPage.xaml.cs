
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Xml.Linq;

namespace FleetCoreMAUI;


public partial class ModUpdateCompanyPage : ContentPage
{


    public ModUpdateCompanyPage()
    {
        InitializeComponent();

        BindingContext = new ModUpdateCompanyViewModel();
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/organization/get");
                var result = await response.Content.ReadAsStringAsync();
                var company = JsonConvert.DeserializeObject<Company>(result);

                Name.Text = company.Name.ToUpper();
                Address1.Text = company.Address1;
                Address2.Text = company.Address2;
                NIP.Text = company.NIP;
            }
            catch
            {

            }
        }       
    }
}



    



