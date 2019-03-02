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
        bool _phoneValid;
        bool _passwordValid;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _phoneValid = false;
            _passwordValid = false;

            entry_phone.Unfocused += fn_phoneChanged;
            entry_phone.TextChanged += fn_phoneChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public void fn_phoneChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            var regex = @"^(\+([0-9]){1,3})?([0-9]){10}$";

            if (entry.Text == null)
            {
                entry.ErrorText = "Phone number is empty";
                _phoneValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            var match = Regex.Match(entry.Text, regex, RegexOptions.IgnoreCase);

            if (entry.Text == "")
            {
                entry.ErrorText = "Phone number is empty";
                _phoneValid = false;
            }
            else if (!match.Success)
            {
                entry.ErrorText = "Required format is +1XXXXXXXXX";
                _phoneValid = false;
            }
            else
            {
                if (entry.Text[0] != '+')
                    entry.Text = "+1" + entry.Text;
                entry.ErrorText = "";
                _phoneValid = true;
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
                string __phone = entry_phone.Text;
                if (_phoneValid)
                    _phoneValid = !(__phone == "");

                string __password = entry_password.Text;
                if (_passwordValid)
                    _passwordValid = !(__password == "");


                if (_phoneValid && _passwordValid)
                {
                    // Login Successful
                    // if phone verified, send to projects home page
                    await Navigation.PushAsync(new ProjectsHomePage());
                    // else send to phone number verification page
                }
                else
                {                    
                    fn_phoneChanged(entry_phone, e);
                    fn_passwordChanged(entry_password, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
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
