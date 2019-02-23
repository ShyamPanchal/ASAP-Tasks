using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsapTasks.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegistrationPage : ContentPage
	{
        bool _nameValid;
        bool _phoneValid;
        bool _passwordsValid;

		public RegistrationPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);

            _nameValid = false;
            _phoneValid = false;
            _passwordsValid = false;

            entry_phone.Text = "+1";

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;

            entry_phone.Unfocused += fn_phoneChanged;
            entry_phone.TextChanged += fn_phoneChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;

            entry_confirmPassword.Unfocused += fn_confirmPasswordChanged;
            entry_confirmPassword.TextChanged += fn_confirmPasswordChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        private async void fn_signupClicked(object sender, EventArgs e)
        {
            try
            {
                string __name = entry_name.Text;
                if (_nameValid)
                    _nameValid = !(__name == "");

                string __password = entry_password.Text;
                if (_passwordsValid)
                    _passwordsValid = !(__password == "");

                string __phone = entry_phone.Text;
                if (_phoneValid)
                    _phoneValid = !(__phone == "");

                string __confirmPassword = entry_confirmPassword.Text;
                if (_passwordsValid)
                    _passwordsValid = !(__confirmPassword == "");


                if (_nameValid && _passwordsValid && _phoneValid)
                {
                    // Signup Successful
                    // to phone verification page
                    await Navigation.PushAsync(new ProjectsHomePage());
                    // else send to phone number verification page
                }
                else
                {
                    fn_nameChanged(entry_name, e);
                    fn_passwordChanged(entry_password, e);
                    fn_confirmPasswordChanged(entry_confirmPassword, e);
                    fn_phoneChanged(entry_phone, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async void fn_loginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public void fn_nameChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter a display name";
                _nameValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter a display name";
                _nameValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _nameValid = true;
            }
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
                _passwordsValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter a password";
                _passwordsValid = false;
            }
            else
            {
                if(entry_confirmPassword.Text == null || entry_confirmPassword.Text == "")
                {
                    entry.ErrorText = "";
                    _passwordsValid = true;
                }
                else if(entry.Text == entry_confirmPassword.Text)
                {
                    entry.ErrorText = "";
                    _passwordsValid = true;
                }
                else
                {
                    entry.ErrorText = "Passwords do not match";
                    _passwordsValid = false;
                }
            }
        }

        public void fn_confirmPasswordChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter a password";
                _passwordsValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter a password";
                _passwordsValid = false;
            }
            else
            {
                if (entry.Text == entry_password.Text)
                {
                    entry.ErrorText = "";
                    _passwordsValid = true;
                }
                else
                {
                    entry.ErrorText = "Passwords do not match";
                    _passwordsValid = false;
                }
            }
        }
    }
}