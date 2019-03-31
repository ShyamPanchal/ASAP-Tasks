using AsapTasks.Data;
using AsapTasks.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AsapTasks.Pages
{
    public partial class MainPage : ContentPage
    {
        #region Private Variables
        private bool _emailValid;
        private bool _passwordValid;

        private DeveloperManager developerManager;
        #endregion

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _emailValid = false;
            _passwordValid = false;

            entry_email.Unfocused += fn_emailChanged;
            entry_email.TextChanged += fn_emailChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.activityIndicator.IsRunning = true;
            developerManager = DeveloperManager.DefaultManager;
            this.activityIndicator.IsRunning = false;
        }

        public void fn_emailChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            var regex = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";

            if (entry.Text == null)
            {
                entry.ErrorText = "Email Address is empty";
                _emailValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            var match = Regex.Match(entry.Text, regex, RegexOptions.IgnoreCase);

            if (entry.Text == "")
            {
                entry.ErrorText = "Email Address is empty";
                _emailValid = false;
            }
            else if (!match.Success)
            {
                entry.ErrorText = "Invalid Email Format";
                _emailValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _emailValid = true;
            }
        }

        public void fn_passwordChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter a password";
                _passwordValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter a password";
                _passwordValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _passwordValid = true;
            }
        }

        private async void fn_loginClicked(object sender, EventArgs e)
        {
            try
            {
                this.activityIndicator.IsRunning = true;

                string __email = entry_email.Text;
                if (_emailValid)
                    _emailValid = !(__email == "");

                string __password = entry_password.Text;
                if (_passwordValid)
                    _passwordValid = !(__password == "");


                if (_emailValid && _passwordValid)
                {
                    Developer developer = await developerManager.GetDeveloperAsync(__email, __password);
                    // Developer developer = new Developer();
                    // await Task.Delay(3000);

                    if (developer == null)
                    {
                        label_error.IsVisible = true;
                    }
                    else
                    {
                        App.developer = developer;

                        Settings.DeveloperId = developer.Id;

                        this.activityIndicator.IsRunning = false;
                        await Navigation.PushAsync(new ProjectsHomePage());
                    }
                }
                else
                {
                    fn_emailChanged(entry_email, e);
                    fn_passwordChanged(entry_password, e);
                    return;
                }
                this.activityIndicator.IsRunning = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                this.activityIndicator.IsRunning = false;
            }
        }

        private async void fn_forgotPassword(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }

        private async void fn_registerNow(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}
