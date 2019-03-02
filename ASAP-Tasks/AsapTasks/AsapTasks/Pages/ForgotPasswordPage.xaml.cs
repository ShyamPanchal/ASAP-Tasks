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
    public partial class ForgotPasswordPage : ContentPage
    {
        string confirmButtonState;
        bool _phoneValid, _passwordsValid, _codeValid;

        public ForgotPasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            confirmButtonState = "Phone";

            _codeValid = false;
            _phoneValid = false;
            _passwordsValid = false;

            entry_phone.Unfocused += fn_phoneChanged;
            entry_phone.TextChanged += fn_phoneChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;

            entry_confirmPassword.Unfocused += fn_confirmPasswordChanged;
            entry_confirmPassword.TextChanged += fn_confirmPasswordChanged;

            entry_verificationCode.Unfocused += fn_codeChanged;
            entry_verificationCode.TextChanged += fn_codeChanged;
        }

        private async void fn_confirmClicked(object sender, EventArgs e)
        {
            switch (confirmButtonState)
            {
                case "Phone":
                    {
                        try
                        {
                            string __phone = entry_password.Text;
                            if (_phoneValid)
                                _phoneValid = !(__phone == "");


                            if (_phoneValid)
                            {
                                entry_phone.IsEnabled = false;

                                button_confirm.Text = "Confirm";

                                entry_verificationCode.IsVisible = true;

                                confirmButtonState = "Verify";

                                label_title.Text = "Verify Account";
                            }
                            else
                            {
                                fn_phoneChanged(entry_phone, e);
                                return;
                            }
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                        
                        break;
                    }
                case "Verify":
                    {
                        try
                        {
                            string __phone = entry_password.Text;
                            if (_phoneValid)
                                _phoneValid = !(__phone == "");

                            string __code = entry_verificationCode.Text;
                            if (_codeValid)
                                _codeValid = !(__code == "");


                            if (_phoneValid && _codeValid)
                            {
                                entry_phone.IsEnabled = false;

                                button_confirm.Text = "Change Password";

                                entry_verificationCode.IsVisible = false;

                                entry_password.IsVisible = true;
                                entry_confirmPassword.IsVisible = true;

                                label_title.Text = "Reset Password";

                                confirmButtonState = "Reset";
                            }
                            else
                            {
                                fn_phoneChanged(entry_phone, e);
                                fn_codeChanged(entry_verificationCode, e);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                        
                        break;
                    }
                case "Reset":
                    {
                        try
                        {
                            string __password = entry_password.Text;
                            if (_passwordsValid)
                                _passwordsValid = !(__password == "");

                            string __phone = entry_phone.Text;
                            if (_phoneValid)
                                _phoneValid = !(__phone == "");

                            string __confirmPassword = entry_confirmPassword.Text;
                            if (_passwordsValid)
                                _passwordsValid = !(__confirmPassword == "");


                            if (_passwordsValid && _phoneValid)
                            {
                                await DisplayAlert("Reset Password", "Password Reset was sucessfull for " + __phone, "OK");
                                await Navigation.PopAsync();
                            }
                            else
                            {
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

                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private async void fn_cancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void fn_phoneChanged(object sender, EventArgs e)
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

        public void fn_codeChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Enter the Verification Code";
                _codeValid = false;
                return;
            }

            entry.Text = entry.Text.TrimStart();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Enter the Verification Code";
                _codeValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _codeValid = true;
            }
        }
    }
}