﻿using System;
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

		public NewTaskPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            List<string> itemList = new List<string>();
            itemList.Add("Incomplete");
            itemList.Add("In Progress");
            itemList.Add("Done");

            picker_status.ItemsSource = itemList;

            editor_description.Focused += fn_DescriptionFocused;
            editor_description.Unfocused += fn_DescriptionUnFocused;

            picker_status.Focused += fn_StatusFocused;
            picker_status.Unfocused += fn_StatusUnFocused;

            picker_status.SelectedIndex = 0;

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;
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
                    // create task
                    await DisplayAlert("New Task", "Task " + __name + " was created", "OK");

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
    }
}