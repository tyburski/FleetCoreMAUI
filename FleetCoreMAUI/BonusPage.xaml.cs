namespace FleetCoreMAUI;

public partial class BonusPage : ContentPage
{
	public BonusPage()
	{
		InitializeComponent();

		BindingContext = new BonusViewModel();
	}
}