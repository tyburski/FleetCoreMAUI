
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
        public ICommand ModCreateNoticeCommand => new Command(ModCreateNotice);
        public ICommand ModLogsCommand => new Command(ModLogs);
        public ICommand ModUsersCommand => new Command(ModUsers);
        public ICommand ModUpdateCompanyCommand => new Command(ModUpdateCompany);

        private async void ModCreateUser()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modCreateUser");
        }

        private async void ModCreateVehicle()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modCreateVehicle");
        }
        private async void ModCreateNotice()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modCreateNotice");
        }
        private async void ModLogs()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modLogs");
        }
        private async void ModUsers()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modUsers");
        }
        private async void ModUpdateCompany()
        {
            await Shell.Current.GoToAsync("//menu/modpanel/modUpdateCompany");
        }

    }
}
