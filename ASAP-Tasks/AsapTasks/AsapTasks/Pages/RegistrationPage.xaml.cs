using AsapTasks.Data;
using AsapTasks.Managers;
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
        #region Private Variables
        bool _nameValid;
        bool _emailValid;
        bool _passwordsValid;

        DeveloperManager developerManager;
        #endregion

        public RegistrationPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _nameValid = false;
            _emailValid = false;
            _passwordsValid = false;

            entry_name.Unfocused += fn_nameChanged;
            entry_name.TextChanged += fn_nameChanged;

            entry_email.Unfocused += fn_emailChanged;
            entry_email.TextChanged += fn_emailChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;

            entry_confirmPassword.Unfocused += fn_confirmPasswordChanged;
            entry_confirmPassword.TextChanged += fn_confirmPasswordChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.activityIndicator.IsRunning = true;
            developerManager = DeveloperManager.DefaultManager;
            this.activityIndicator.IsRunning = false;
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

                string __email = entry_email.Text;
                if (_emailValid)
                    _emailValid = !(__email == "");

                string __confirmPassword = entry_confirmPassword.Text;
                if (_passwordsValid)
                    _passwordsValid = !(__confirmPassword == "");


                if (_nameValid && _passwordsValid && _emailValid)
                {
                    this.activityIndicator.IsRunning = true;
                    Developer developer = new Developer()
                    {
                        Name = __name,
                        Email = __email,
                        Password = __password,
                        IsVerified = false
                    };

                    Constants.DataCheckErrors check = await developerManager.CheckEmailAsync(__email);

                    if (check == Constants.DataCheckErrors.DN_EXISTS)
                    {
                        EmailVerificationPage emailVerificationPage = new EmailVerificationPage() { BindingContext = developer };
                        await Navigation.PushAsync(emailVerificationPage);
                    }
                    else if(check == Constants.DataCheckErrors.EXISTS)
                    {
                        entry_email.ErrorText = "An account with this email already exists";
                        _emailValid = false;
                    }
                    else if(check == Constants.DataCheckErrors.ERROR)
                    {
                        await DisplayAlert("Connection Error", "Please check your internet connection", "OK");
                    }

                    this.activityIndicator.IsRunning = false;
                }
                else
                {
                    fn_nameChanged(entry_name, e);
                    fn_passwordChanged(entry_password, e);
                    fn_confirmPasswordChanged(entry_confirmPassword, e);
                    fn_emailChanged(entry_email, e);
                    this.activityIndicator.IsRunning = false;
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
                if (entry_confirmPassword.Text == null || entry_confirmPassword.Text == "")
                {
                    entry.ErrorText = "";
                    _passwordsValid = true;
                }
                else if (entry.Text == entry_confirmPassword.Text)
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