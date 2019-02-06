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
        bool _emailValid;
        bool _passwordValid;

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

        public void fn_emailChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            var regex = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";

            if (entry.Text == null)
            {
                entry.ErrorText = "Email address is empty";
                _emailValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            var match = Regex.Match(entry.Text, regex, RegexOptions.IgnoreCase);

            if (entry.Text == "")
            {
                entry.ErrorText = "Email address is empty";
                _emailValid = false;
            }
            else if (!match.Success)
            {
                entry.ErrorText = "Please enter a valid email address";
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
                string __email = entry_email.Text;
                if (_emailValid)
                    _emailValid = !(__email == "");

                string __password = entry_password.Text;
                if (_passwordValid)
                    _passwordValid = !(__password == "");


                if (_emailValid && _passwordValid)
                {
                    // Login Successful
                    // if phone verified, send to projects home page
                    await Navigation.PushAsync(new ProjectsHomePage());
                    // else send to phone number verification page
                }
                else
                {                    
                    fn_emailChanged(entry_email, e);
                    fn_passwordChanged(entry_password, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void fn_forgotPassword(object sender, EventArgs e)
        {

        }

        private void fn_registerNow(object sender, EventArgs e)
        {

        }
    }
}
