﻿
namespace FleetCoreMAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute("bonus", typeof(BonusPage));
        Routing.RegisterRoute("modpanel", typeof(ModPanel));
        Routing.RegisterRoute("modCreateUser", typeof(ModCreateUserPage));
        Routing.RegisterRoute("modUsers", typeof(ModUsersPage));
        Routing.RegisterRoute("modCreateVehicle", typeof(ModCreateVehiclePage));
    }
}