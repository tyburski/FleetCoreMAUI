
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

using FleetCoreMAUI.Models;

namespace FleetCoreMAUI
{
    class ModViewModel
    {
        public ICommand ModCreateUserCommand => new Command(ModCreateUser);
        public ICommand ModCreateVehicleCommand => new Command(ModCreateVehicle);
        public ICommand ModLogsCommand => new Command(ModLogs);
        public ICommand ModUsersCommand => new Command(ModUsers); 
        
        private async void ModCreateUser()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modCreateUser");
        }

        private async void ModCreateVehicle()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modCreateVehicle");
        }
        private async void ModLogs()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modLogs");
        }

        private async void ModUsers()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modUsers");
        }
        
    }
}
