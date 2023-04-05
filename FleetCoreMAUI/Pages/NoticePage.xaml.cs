using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Web;
using static System.Net.WebRequestMethods;

namespace FleetCoreMAUI;

public partial class NoticePage : ContentPage
{
	public NoticePage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if (await GetNotices() is true)
        {
            popup.Close();
           
        }
        else
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
        }
    }
    async Task<bool> GetNotices()
    {
        using(var http = new HttpClient())
        {
            try
            {
                var response = await http.GetAsync("https://primasystem.pl/api/organization/getnotices");

                var resString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Notice>>(resString);

                if (response.IsSuccessStatusCode)
                {
                    foreach (Notice n in result)
                    {
                        n.ConvertedDate = n.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    }
                    var l = result.OrderByDescending(x => x.CreatedAt);
                    List.ItemsSource = l;
                    await UpdateNoticeDate(l.First().CreatedAt);
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
    async Task UpdateNoticeDate(DateTime date)
    {
        using(var http = new HttpClient())
        {
            var update = new UpdateNoticeDate()
            {
                Date = date,
                userId = await SecureStorage.GetAsync("userId")
            };
            var json = JsonConvert.SerializeObject(update);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            await http.PostAsync("https://primasystem.pl/api/account/noticedate", content);
            await SecureStorage.SetAsync("notice", date.ToString("dd-MM-yyyy HH:mm"));
        }        
    }
}