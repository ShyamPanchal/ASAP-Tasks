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
        #region Private Variables

        private bool _nameValid;

        private bool _isInEditMode;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public NewProjectPage ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        /// <summary>
        /// Function called when the page components are ready to be rendered
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            editor_description.Focused += fn_descriptionFocused;
            editor_description.Unfocused += fn_descriptionUnFocused;

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

        /// <summary>
        /// Function called when the name text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function called when the Description editor is Focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void fn_descriptionFocused(object sender, EventArgs e)
        {
            label_description.IsVisible = true;
            label_description.TextColor = (Color)Application.Current.Resources["color_Amazon"];
            editor_description.Placeholder = "Project Description";
        }

        /// <summary>
        /// Function called when the Description Editor is Unfocused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void fn_descriptionUnFocused(object sender, EventArgs e)
        {
            label_description.TextColor = (Color)Application.Current.Resources["color_DimGray"];
        }

        /// <summary>
        /// Function called whent the Cancel button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void fn_cancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Function called when the confirm button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        this.activityIndicator.IsRunning = true;

                        Project project = App.selectedProject;                        
                        project.Name = __name;
                        project.Description = editor_description.Text;
                        project.OpenStatus = !checkBox_status.Checked;
                        project.StartDate = picker_startDate.Date.ToShortDateString();

                        await App.projectManager.SaveProjectAsync(project);

                        this.activityIndicator.IsRunning = false;

                        await DisplayAlert("Project Changes", "Project " + __name + " was updated successfully.", "OK");
                    }
                    else
                    {
                        this.activityIndicator.IsRunning = true;

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

                        this.activityIndicator.IsRunning = false;

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
                this.activityIndicator.IsRunning = false;
            }
        }
    }
}