
namespace FleetCoreMAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute("bonus", typeof(BonusPage));
        Routing.RegisterRoute("notice", typeof(NoticePage));
        Routing.RegisterRoute("vehicles", typeof(VehiclesPage));
        Routing.RegisterRoute("vehicleDetails", typeof(VehicleDetailsPage));
        Routing.RegisterRoute("modpanel", typeof(ModPanel));
        Routing.RegisterRoute("modCreateUser", typeof(ModCreateUserPage));
        Routing.RegisterRoute("modCreateNotice", typeof(ModCreateNoticePage));
        Routing.RegisterRoute("modUsers", typeof(ModUsersPage));
        Routing.RegisterRoute("modCreateVehicle", typeof(ModCreateVehiclePage));
        Routing.RegisterRoute("modUpdateCompany", typeof(ModUpdateCompanyPage));
        Routing.RegisterRoute("modLogs", typeof(ModLogs));
        Routing.RegisterRoute("refuel", typeof(RefuelPage));
    }
}
