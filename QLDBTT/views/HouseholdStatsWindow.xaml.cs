using System.Windows;
using Disease_Disaster.Controllers; 
namespace Disease_Disaster.Views
{
	public partial class HouseholdStatsWindow : Window
	{
		private readonly FacilityManagementController _controller = new FacilityManagementController();

		public HouseholdStatsWindow()
		{
			InitializeComponent(); 
			LoadData();
		}

		private void LoadData()
		{
			dgStats.ItemsSource = _controller.GetHouseholdStats();
		}
	}
}