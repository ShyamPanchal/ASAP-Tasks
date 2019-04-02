using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsapTasks.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectInvitePage : ContentPage
	{
		public ProjectInvitePage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);

            det_name.Text = App.selectedProject.Name;
            det_date.Text = DateTime.Parse(App.selectedProject.StartDate).ToLongDateString();
            det_desc.Text = App.selectedProject.Description;
            det_stat.Text = App.selectedProject.OpenStatus ? "Open" : "Closed";
		}

        public async void fn_declineClicked(object sender, EventArgs e)
        {
            this.activityIndicator.IsRunning = true;

            await App.enrollmentManager.DeleteAsync(App.selectedEnrollment);

            this.activityIndicator.IsRunning = false;

            await DisplayAlert("Invite Declined", "Declined invite for the project " + App.selectedProject.Name, "OK");
            await Navigation.PopAsync();
        }

        public async void fn_acceptClicked(object sender, EventArgs e)
        {
            this.activityIndicator.IsRunning = true;

            App.selectedEnrollment.AcceptStatus = true;
            await App.enrollmentManager.SaveEnrollmentAsync(App.selectedEnrollment);

            this.activityIndicator.IsRunning = false;

            await DisplayAlert("Invite Accepted", "Accepted invite for the project " + App.selectedProject.Name, "OK");
            await Navigation.PopAsync();
        }

    }
}