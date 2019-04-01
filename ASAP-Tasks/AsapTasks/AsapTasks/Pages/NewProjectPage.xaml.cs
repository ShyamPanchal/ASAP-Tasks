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
	public partial class NewProjectPage : ContentPage
	{
        bool _nameValid;

        bool _isInEditMode;

        public NewProjectPage ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            editor_description.Focused += fn_DescriptionFocused;
            editor_description.Unfocused += fn_DescriptionUnFocused;

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;

            if(App.selectedProject == null)
            {
                _isInEditMode = false;
                checkBox_status.Checked = false;
                checkBox_status.IsEnabled = false;
            }
            else
            {
                _isInEditMode = true;
                label_heading.Text = "Edit Project Details";
                entry_name.Text = App.selectedProject.Name;
                picker_startDate.Date = DateTime.Parse(App.selectedProject.StartDate);
                editor_description.Text = App.selectedProject.Description;
                checkBox_status.Checked = !App.selectedProject.OpenStatus;

                button_confirm.Text = "Save";
            }
        }

        public void fn_nameChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter name for the task";
                _nameValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter name for the task";
                _nameValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _nameValid = true;
            }
        }

        public void fn_DescriptionFocused(object sender, EventArgs e)
        {
            label_description.IsVisible = true;
            label_description.TextColor = (Color)Application.Current.Resources["color_Amazon"];
            editor_description.Placeholder = "Project Description";
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
                        Project project = App.selectedProject;                        
                        project.Name = __name;
                        project.Description = editor_description.Text;
                        project.OpenStatus = !checkBox_status.Checked;
                        project.StartDate = picker_startDate.Date.ToShortDateString();

                        await App.projectManager.SaveProjectAsync(project);

                        await DisplayAlert("Project Changes", "Project " + __name + " was updated successfully.", "OK");
                    }
                    else
                    {
                        Project project = new Project();

                        project.Name = __name;
                        project.Description = editor_description.Text;
                        project.OpenStatus = true;
                        project.StartDate = picker_startDate.Date.ToShortDateString();

                        await App.projectManager.SaveProjectAsync(project);

                        Enrollment enrollment = new Enrollment();
                        enrollment.AcceptStatus = true;
                        enrollment.DeveloperId = App.developer.Id;
                        enrollment.ProjectId = project.Id;

                        System.Diagnostics.Debug.WriteLine("Project: " + project.Id);

                        await App.enrollmentManager.SaveEnrollmentAsync(enrollment);

                        await DisplayAlert("New Project", "Project " + project.Name + " was created", "OK");
                    }
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
    }
}