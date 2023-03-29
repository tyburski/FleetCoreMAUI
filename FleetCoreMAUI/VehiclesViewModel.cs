
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FleetCoreMAUI
{
    internal class VehiclesViewModel
    {

        public ICommand VehicleMenuCommand =>
        new Command(async () => await VehicleMenu());

        public async Task VehicleMenu()
        {
            await App.Current.MainPage.DisplayAlert("Jesteś pewny?", "Zostaniesz wylogowany", "Ok", "Anuluj");
            await App.Current.MainPage.DisplayActionSheet("Share message by...", "Cancel", null, "SMS", "Email", "WhatsApp");
        }
    }
}
