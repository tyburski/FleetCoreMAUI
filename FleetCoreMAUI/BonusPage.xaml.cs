using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace FleetCoreMAUI;

public partial class BonusPage : ContentPage, IQueryAttributable
{
    public string fullName { get; set; }

    Microsoft.Maui.Graphics.Color colorDark = Microsoft.Maui.Graphics.Color.FromArgb("#22459C");
    Microsoft.Maui.Graphics.Color colorLight = Microsoft.Maui.Graphics.Color.FromArgb("#3664D6");

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        fullName = HttpUtility.UrlDecode(query["fullName"].ToString());
        title.Text = $"{fullName}:Bonusy";
        if (fullName != null)
        {
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);
            if(await GetBonuses(fullName, "current") is true)
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
    public BonusPage()
	{
		InitializeComponent();
	}

	async void Button1_Clicked(object sender, EventArgs e)
	{
        Button1.BackgroundColor = colorDark;
        Button2.BackgroundColor = colorLight;

        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if (await GetBonuses(fullName, "current") is true)
        {
            popup.Close();
        }
        else
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
        }
    }
	async void Button2_Clicked(object sender, EventArgs e)
	{
        Button1.BackgroundColor = colorLight;
        Button2.BackgroundColor = colorDark;

        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);
        if (await GetBonuses(fullName, "previous") is true)
        {
            popup.Close();
        }
        else
        {
            popup.Close();
            await Application.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
        }
    }
    async Task<bool> GetBonuses(string fullname, string action)
    {
        using(var http = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(fullname);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync("https://primasystem.pl/api/bonus/get", content);

                var resString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Bonus>>(resString);

                if (response.IsSuccessStatusCode)
                {
                    var contentList = new List<Bonus>();
                    if (action.Equals("current"))
                    {
                        var selectedRange = result.Where(x => x.CreatedAt.Month.Equals(DateTime.Now.Month)).OrderByDescending(x => x.CreatedAt);
                        contentList.AddRange(selectedRange);

                        foreach (Bonus b in contentList)
                        {
                            b.ConvertedDate = b.CreatedAt.ToString("dd/MM/yyyy");
                        }
                    }
                    else if (action.Equals("previous"))
                    {
                        var selectedRange = result.Where(x => x.CreatedAt.Month.Equals(DateTime.Now.AddMonths(-1).Month)).OrderByDescending(x => x.CreatedAt);
                        contentList.AddRange(selectedRange);

                        foreach (Bonus b in contentList)
                        {
                            b.ConvertedDate = b.CreatedAt.ToString("dd/MM/yyyy");
                        }
                    }
                    List.ItemsSource = contentList;
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