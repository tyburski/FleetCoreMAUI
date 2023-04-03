using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace FleetCoreMAUI;

public partial class RefuelPage : ContentPage, IQueryAttributable
{
    public string fullName { get; set; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        fullName = HttpUtility.UrlDecode(query["fullName"].ToString());
        title.Text = $"{fullName}:Tankowania";
        if (fullName != null)
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if (await GetRefuelings(fullName) is true)
            {
                popup.Close();
            }
            else
            {
                popup.Close();
                await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
            }
        }
    }
    public RefuelPage()
	{
		InitializeComponent();
	}
    async Task<bool> GetRefuelings(string fullname)
    {
        using(var http = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(fullname);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync("https://primasystem.pl/api/account/getrefuelings", content);

                var resString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Refueling>>(resString);

                if (response.IsSuccessStatusCode)
                {
                    foreach (Refueling b in result)
                    {
                        b.ConvertedDate = b.CreatedAt.ToString("dd/MM/yyyy");
                    }
                    List.ItemsSource = result.OrderByDescending(x => x.CreatedAt);
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }       
    }
 }