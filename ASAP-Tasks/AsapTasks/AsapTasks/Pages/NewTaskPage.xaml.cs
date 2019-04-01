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
	public partial class NewTaskPage : ContentPage
	{
        bool _nameValid;

        bool _isInEditMode;

		public NewTaskPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            picker_status.ItemsSource = Constants.taskStatus;

            editor_description.Focused += fn_DescriptionFocused;
            editor_description.Unfocused += fn_DescriptionUnFocused;

            picker_status.Focused += fn_StatusFocused;
            picker_status.Unfocused += fn_StatusUnFocused;

            picker_status.SelectedIndex = 0;

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;

            if (App.selectedTask == null)
            {
                _isInEditMode = false;
            }
            else
            {
                _isInEditMode = true;
                text_Heading.Text = "Edit Task";
                entry_name.Text = App.selectedTask.Name;
                label_description.IsVisible = true;
                editor_description.Text = App.selectedTask.Description;
                for(int i = 0; i < Constants.taskStatus.Length; i++)
                {
                    if(Constants.taskStatus[i] == App.selectedTask.CompletionStatus)
                    {
                        picker_status.SelectedIndex = i;
                        break;
                    }
                }

                button_confirm.Text = "Save";
                frame_delete.IsVisible = true;
            }
        }

        public void fn_DescriptionFocused(object sender, EventArgs e)
        {
            label_description.IsVisible = true;
            label_description.TextColor = (Color)Application.Current.Resources["color_Amazon"];
            editor_description.Placeholder = "Task Description";
        }

        public void fn_DescriptionUnFocused(object sender, EventArgs e)
        {
            label_description.TextColor = (Color)Application.Current.Resources["color_DimGray"];
        }

        public void fn_StatusFocused(object sender, EventArgs e)
        {
            label_status.TextColor = (Color)Application.Current.Resources["color_DimGray"];
        }

        public void fn_StatusUnFocused(object sender, EventArgs e)
        {
            label_status.TextColor = (Color)Application.Current.Resources["color_DimGray"];
        }

        public async void fn_cancelClicked(object sender, EventArgs e)
        {
            App.selectedTask = null;
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
                        App.selectedTask.Name = __name;
                        App.selectedTask.Description = editor_description.Text;
                        App.selectedTask.CompletionStatus = picker_status.SelectedItem.ToString();

                        await App.projectTaskManager.SaveProjectTaskAsync(App.selectedTask);

                        await DisplayAlert("Task Changes", "Task " + __name + " was updated successfully", "OK");
                    }
                    else
                    {
                        ProjectTask task = new ProjectTask();

                        task.Name = __name;
                        task.Description = editor_description.Text;
                        task.CompletionStatus = picker_status.SelectedItem.ToString();
                        task.ProjectId = App.selectedProject.Id;
                        task.DeveloperId = App.developer.Id;
                        task.EnrollmentId = (await App.enrollmentManager.GetEnrollmentFromBothIdAsync(App.developer.Id, App.selectedProject.Id)).Id;

                        await App.projectTaskManager.SaveProjectTaskAsync(task);

                        await DisplayAlert("New Task", "Task " + __name + " was created", "OK");
                    }

                    App.selectedTask = null;
                    await Navigation.PopAsync();
                }
                else
                {
                    fn_nameChanged(entry_name, e);
                    App.selectedTask = null;
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

        public async void fn_deleteClicked(object sender, EventArgs e)
        {
            string name = App.selectedTask.Name;

            await App.projectTaskManager.DeleteAsync(App.selectedTask);

            App.selectedTask = null;

            await DisplayAlert("Delete Task", "Task "+ name + " successfully deleted !","OK");

            await Navigation.PopAsync();
        }
    }
}