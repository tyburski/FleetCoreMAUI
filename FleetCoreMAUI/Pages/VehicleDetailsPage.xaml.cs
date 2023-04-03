
using CommunityToolkit.Maui.Views;
using FleetCoreMAUI.Models;
using Microsoft.Maui.Controls.Internals;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Windows.Input;
using static System.Net.WebRequestMethods;


namespace FleetCoreMAUI;


public partial class VehicleDetailsPage : ContentPage, IQueryAttributable
{
    public string plate { get; set; }
    public DateTime selectedDate { get; set; }
    public int selectedId { get; set; }
    ObservableCollection<EventViewModel> eventsList { get; set; } = new();


    Microsoft.Maui.Graphics.Color colorDark = Microsoft.Maui.Graphics.Color.FromArgb("#22459C");
    Microsoft.Maui.Graphics.Color colorLight = Microsoft.Maui.Graphics.Color.FromArgb("#3664D6");
    Microsoft.Maui.Graphics.Color colorWarning = Microsoft.Maui.Graphics.Color.FromArgb("#D32B47");

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        plate = HttpUtility.UrlDecode(query["plate"].ToString());
        if (plate != null)
        {
            await GetVehicle(plate, "events");
        }
    }
    public VehicleDetailsPage()
    {
        InitializeComponent();
    }
    private async Task GetVehicle(string plate, string state)
    {
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);

        using(var http = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(plate);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync("https://primasystem.pl/api/vehicle/get", content);

                var resString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Vehicle>(resString);

                Plate.Text = result.Plate;
                Mileage.Text = result.Mileage.ToString();
                VIN.Text = result.VIN;

                if (response.IsSuccessStatusCode)
                {
                    if (state.Equals("events"))
                    {
                        EventsRender(result);
                    }
                    else if (state.Equals("repairs"))
                    {
                        RepairsRender(result);
                    }
                    else if (state.Equals("donerepairs"))
                    {
                        DoneRepairsRender(result);
                    }
                    else
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                    }
                    popup.Close();
                }
                else
                {
                    popup.Close();
                    await App.Current.MainPage.DisplayAlert("BŁĄD", "Spróbuj ponownie", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }       
    }

    async Task GetEvents(Vehicle vehicle)
    {
        List<EventViewModel> convertedList = new List<EventViewModel>();

        foreach (Event e in vehicle.Events)
        {
            var checkDate = e.Date.Subtract(DateTime.Now);
            Microsoft.Maui.Graphics.Color color = colorDark;
            if (checkDate.TotalDays <= 14)
            {
                color = colorWarning;
            }
            else color = colorDark;
            var x = new EventViewModel();
            x.Id = e.Id;
            x.Content = e.Content;
            x.Date = e.Date;
            x.ConvertedDate = e.Date.ToString("dd/MM/yyyy");
            x.color = color;
            convertedList.Add(x);
        }
        eventsList = new ObservableCollection<EventViewModel>(convertedList);
        Events.ItemsSource = eventsList;
    }
    async Task GetRepairs(Vehicle vehicle)
    {
        var list = new List<RepairViewModel>();

        foreach(Repair r in vehicle.Repairs)
        {
            if(r.UserFinished is null)
            {
                var x = new RepairViewModel()
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedBy = r.User.fullName,
                    CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy")
                };
                list.Add(x);
            }
        }
        RepairsList.ItemsSource = list.OrderByDescending(x=>x.CreatedAt);
    }
    async Task GetDoneRepairs(Vehicle vehicle)
    {
        var list = new List<RepairViewModel>();

        foreach (Repair r in vehicle.Repairs)
        {
            if (r.UserFinished is not null)
            {
                var x = new RepairViewModel()
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedBy = r.User.fullName,
                    CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy"),
                    FinishedBy = r.UserFinished.fullName,
                    FinishAt = r.FinishAt.ToString("dd/MM/yyyy")
                };
                list.Add(x);
            }
        }
        DoneRepairsList.ItemsSource = list.OrderByDescending(x=>x.FinishAt);
    }

    async void EventsRender(Vehicle vehicle)
    {
        Button1.BackgroundColor = colorDark;
        Button2.BackgroundColor = colorLight;
        Button3.BackgroundColor = colorLight;
        await GetEvents(vehicle);
        Events.IsVisible = true;
        DateChange.IsVisible = false;
        Repairs.IsVisible = false;
        createRepairButton.IsVisible = false;
        DoneRepairs.IsVisible = false;
    }
    async void RepairsRender(Vehicle vehicle)
    {
        Button1.BackgroundColor = colorLight;
        Button2.BackgroundColor = colorDark;
        Button3.BackgroundColor = colorLight;
        await GetRepairs(vehicle);
        Events.IsVisible = false;
        DateChange.IsVisible = false;
        Repairs.IsVisible = true;
        createRepairButton.IsVisible = true;
        RepairCreate.IsVisible = false;
        DoneRepairs.IsVisible = false;
    }
    async void DoneRepairsRender(Vehicle vehicle)
    {
        Button1.BackgroundColor = colorLight;
        Button2.BackgroundColor = colorLight;
        Button3.BackgroundColor = colorDark;
        await GetDoneRepairs(vehicle);
        Events.IsVisible = false;
        DateChange.IsVisible = false;
        Repairs.IsVisible = false;
        createRepairButton.IsVisible = false;
        RepairCreate.IsVisible = false;
        DoneRepairs.IsVisible = true;
    }

    async void Button1_Clicked(object sender, EventArgs args)
    {
        await GetVehicle(plate, "events");
    }
    async void Button2_Clicked(object sender, EventArgs args)
    {
        await GetVehicle(plate, "repairs");
    }
    async void Button3_Clicked(object sender, EventArgs args)
    {
        await GetVehicle(plate, "donerepairs");
    }

    void OnDateClicked(object sender, EventArgs args)
    {
        var button = (ImageButton)sender;
        var id = Int32.Parse(button.ClassId);

        GetDataChange(id);
    }
    void GetDataChange(int id)
    {
        Events.IsVisible = false;
        DateChange.IsVisible = true;
        var ev = eventsList.FirstOrDefault(x => x.Id.Equals(id));
        content.Text = ev.Content;
        date.Text = ev.ConvertedDate;
        picker.Date = ev.Date;
        selectedId = id;
    }
    async void OnSaveDateClicked(object sender, EventArgs args)
    {
        var model = new UpdateEventDate()
        {
            Id = selectedId,
            Date = selectedDate
        };
        var popup = new Spinner();
        Application.Current.MainPage.ShowPopup(popup);

        using (var http = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync("https://primasystem.pl/api/vehicle/event", content);

                if (response.IsSuccessStatusCode)
                {
                    popup.Close();
                    await App.Current.MainPage.DisplayAlert("SUKCES", "Data została zmieniona", "Ok");
                    DateChange.IsVisible = false;
                    Events.IsVisible = true;
                    await GetVehicle(plate, "events");
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
    void OnCancelDateClicked(object sender, EventArgs args)
    {
        DateChange.IsVisible = false;
        Events.IsVisible = true;
    }
    public void OnDateSelected(object sender, DateChangedEventArgs args)
    {
        var picker = (DatePicker)sender;

        if (picker.Date != null)
        {
            selectedDate = picker.Date;
        }
    }

    void CreateRepair(object sender, EventArgs args)
    {
        createRepairButton.IsVisible = false;
        Repairs.IsVisible = false;
        RepairCreate.IsVisible = true;
    }
    async void OnCreateRepairClicked(object sender, EventArgs args)
    {
        if (repairEntry.Text.Length<4)
        {
            await App.Current.MainPage.DisplayAlert("BŁĄD", "Musisz wpisać więcej niż 3 znaki", "Ok");
        }
        else
        {
            var model = new CreateRepairModel()
            {
                Content = repairEntry.Text,
                Plate = plate,
                userId = await SecureStorage.GetAsync("userId")
            };
            var popup = new Spinner();
            Application.Current.MainPage.ShowPopup(popup);

            using(var http = new HttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync("https://primasystem.pl/api/vehicle/repair/create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        popup.Close();
                        await App.Current.MainPage.DisplayAlert("SUKCES", "Usterka została utworzona", "Ok");
                        await GetVehicle(plate, "repairs");
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
    async void OnCancelRepairClicked(object sender, EventArgs args)
    {
        await GetVehicle(plate, "repairs");
    }
    async void FinishRepair(object sender, EventArgs args)
    {
        var button = (Button)sender;
        var id = Int32.Parse(button.ClassId);

        if(await App.Current.MainPage.DisplayAlert("Jesteś pewny?", "Czy chcesz zakończyć naprawę?", "Tak", "Anuluj"))
        {
            var finishRepair = new FinishRepairModel()
            {
                repairId = id,
                userId = await SecureStorage.Default.GetAsync("userId")
            };
            string action = await DisplayActionSheet("Wybierz akcję", "Anuluj", null, "Zakończ i dodaj jako bonus", "Po prostu zakończ");
            if(action!=null)
            {
                if (!action.Equals(String.Empty))
                {
                    if (action.Equals("Zakończ i dodaj jako bonus"))
                    {
                        finishRepair.isBonus = true;
                    }
                    else if (action.Equals("Po prostu zakończ"))
                    {
                        finishRepair.isBonus = false;
                    }

                    var popup = new Spinner();
                    Application.Current.MainPage.ShowPopup(popup);

                    using(var http = new HttpClient())
                    {
                        try
                        {
                            var json = JsonConvert.SerializeObject(finishRepair);
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await http.PostAsync("https://primasystem.pl/api/vehicle/repair/finish", content);

                            if (response.IsSuccessStatusCode)
                            {
                                popup.Close();
                                await App.Current.MainPage.DisplayAlert("SUKCES", "Naprawa została zakończona", "Ok");
                                await GetVehicle(plate, "repairs");
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
    }
}

    



