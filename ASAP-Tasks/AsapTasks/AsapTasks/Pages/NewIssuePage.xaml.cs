using AsapTasks.Data;
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
	public partial class NewIssuePage : ContentPage
	{
        bool _nameValid;

        bool _isInEditMode;

        public NewIssuePage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            editor_description.Focused += fn_DescriptionFocused;
            editor_description.Unfocused += fn_DescriptionUnFocused;

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;

            if(App.selectedIssue == null)
            {
                _isInEditMode = false;
                checkBox_status.Checked = false;
                checkBox_status.IsEnabled = false;
            }
            else
            {
                _isInEditMode = true;
                text_Heading.Text = "Edit Issue";
                entry_name.Text = App.selectedIssue.Name;
                label_description.IsVisible = true;
                editor_description.Text = App.selectedIssue.Description;
                checkBox_status.Checked = App.selectedIssue.CompletionStatus;

                button_confirm.Text = "Save";

                frame_delete.IsVisible = true;

                if (!App.selectedProject.OpenStatus)
                {
                    frame_delete.IsVisible = false;
                    button_confirm.IsVisible = false;
                }
            }
        }

        public void fn_DescriptionFocused(object sender, EventArgs e)
        {
            label_description.IsVisible = true;
            label_description.TextColor = (Color)Application.Current.Resources["color_Amazon"];
            editor_description.Placeholder = "Issue Description";
        }

        public void fn_DescriptionUnFocused(object sender, EventArgs e)
        {
            label_description.TextColor = (Color)Application.Current.Resources["color_DimGray"];
        }

        public async void fn_cancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public async void fn_confirmClicked(object sender, EventArgs e)
        {
            try
            {
                string __name = entry_name.Text;
                if (_nameValid)
                    _nameValid = !(__name == "");


                if (_nameValid)
                {
                    if (_isInEditMode)
                    {
                        Issue issue = App.selectedIssue;

                        issue.Name = __name;
                        issue.Description = editor_description.Text;
                        issue.CompletionStatus = checkBox_status.Checked;

                        await App.issueManager.SaveIssueAsync(issue);

                        await DisplayAlert("Issue Changes", "Issue " + __name + " was updated successfully", "OK");
                    }
                    else
                    {
                        Issue issue = new Issue();

                        issue.Name = __name;
                        issue.Description = editor_description.Text;
                        issue.CompletionStatus = false;
                        issue.ProjectId = App.selectedProject.Id;
                        issue.DeveloperId = App.developer.Id;
                        issue.EnrollmentId = (await App.enrollmentManager.GetEnrollmentFromBothIdAsync(App.developer.Id, App.selectedProject.Id)).Id;

                        await App.issueManager.SaveIssueAsync(issue);

                        await DisplayAlert("New Issue", "Issue " + __name + " was created", "OK");
                    }
                    App.selectedIssue = null;
                    await Navigation.PopAsync();
                }
                else
                {
                    fn_nameChanged(entry_name, e);                    
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void fn_nameChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter name for the issue";
                _nameValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter name for the issue";
                _nameValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _nameValid = true;
            }
        }

        public async void fn_deleteClicked(object sender, EventArgs e)
        {
            string name = App.selectedIssue.Name;

            await App.issueManager.DeleteAsync(App.selectedIssue);

            App.selectedIssue = null;

            await DisplayAlert("Delete Issue", "Task " + name + " successfully deleted !", "OK");

            await Navigation.PopAsync();
        }
    }
}