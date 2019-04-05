using AsapTasks.Data;
using AsapTasks.Managers;
using AsapTasks.Services;
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
        #region Private Variables

        string confirmButtonState;

        private bool _emailValid, _passwordsValid, _codeValid;

        private System.Diagnostics.Stopwatch _stopwatch;

        private bool _isTimerStart;

        private DeveloperManager developerManager;

        private Developer developer;

        private int _verificationCode;

        private TimeSpan _maxTime = new TimeSpan(0, 3, 0);
        //private TimeSpan _maxTime = new TimeSpan(0, 0, 10);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ForgotPasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            confirmButtonState = "Email";

            _codeValid = false;
            _emailValid = false;
            _passwordsValid = false;

            entry_email.Unfocused += fn_emailChanged;
            entry_email.TextChanged += fn_emailChanged;

            entry_password.Unfocused += fn_passwordChanged;
            entry_password.TextChanged += fn_passwordChanged;

            entry_confirmPassword.Unfocused += fn_confirmPasswordChanged;
            entry_confirmPassword.TextChanged += fn_confirmPasswordChanged;

            entry_verificationCode.Unfocused += fn_codeChanged;
            entry_verificationCode.TextChanged += fn_codeChanged;

            this.activityIndicator.IsRunning = true;
            developerManager = DeveloperManager.DefaultManager;
            this.activityIndicator.IsRunning = false;
        }

        /// <summary>
        /// Function called when the Confirm Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void fn_confirmClicked(object sender, EventArgs e)
        {
            switch (confirmButtonState)
            {
                case "Email":
                    {
                        try
                        {
                            string __email = entry_email.Text;
                            if (_emailValid)
                                _emailValid = !(__email == "");


                            if (_emailValid)
                            {
                                this.activityIndicator.IsRunning = true;

                                developer = await developerManager.CheckDeveloperEmailAsync(__email);

                                if (developer == null)
                                {
                                    label_error.Text = "No account found with this email";
                                    label_error.IsVisible = true;
                                    this.activityIndicator.IsRunning = false;
                                    return;
                                }
                                else
                                {
                                    entry_email.IsEnabled = false;

                                    button_confirm.Text = "Confirm";

                                    await fn_sendEmail();

                                    entry_verificationCode.IsVisible = true;
                                    label_instruction.IsVisible = true;
                                    label_time.IsVisible = true;
                                    label_error.Text = "Invalid Code";
                                    label_error.IsVisible = false;

                                    confirmButtonState = "Verify";

                                    label_title.Text = "Verify Account";
                                    this.activityIndicator.IsRunning = false;
                                }
                            }
                            else
                            {
                                fn_emailChanged(entry_email, e);
                                this.activityIndicator.IsRunning = false;
                                return;
                            }
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                            this.activityIndicator.IsRunning = false;
                        }
                        
                        break;
                    }
                case "Verify":
                    {
                        try
                        {
                            string __email = entry_password.Text;
                            if (_emailValid)
                                _emailValid = !(__email == "");

                            string __code = entry_verificationCode.Text;
                            if (_codeValid)
                                _codeValid = !(__code == "");


                            if (_emailValid && _codeValid)
                            {
                                if (__code == _verificationCode.ToString()){
                                    entry_email.IsEnabled = false;

                                    button_confirm.Text = "Change Password";

                                    entry_verificationCode.IsVisible = false;
                                    label_instruction.IsVisible = false;
                                    label_time.IsVisible = false;

                                    entry_password.IsVisible = true;
                                    entry_confirmPassword.IsVisible = true;

                                    label_title.Text = "Reset Password";

                                    confirmButtonState = "Reset";
                                    label_error.IsVisible = false;

                                    _isTimerStart = false;
                                    _stopwatch.Stop();
                                    button_resend.IsVisible = false;
                                }
                                else
                                {
                                    label_error.IsVisible = true;
                                }
                            }
                            else
                            {
                                fn_emailChanged(entry_email, e);
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

                            string __email = entry_email.Text;
                            if (_emailValid)
                                _emailValid = !(__email == "");

                            string __confirmPassword = entry_confirmPassword.Text;
                            if (_passwordsValid)
                                _passwordsValid = !(__confirmPassword == "");


                            if (_passwordsValid && _emailValid)
                            {
                                this.activityIndicator.IsRunning = true;
                                developer.Password = __password;
                                var check = await developerManager.SaveDeveloperAsync(developer);
                                if (check == Constants.DataEntryErrors.SUCCESS)
                                {
                                    await DisplayAlert("Reset Password", "Password Reset was sucessful for " + __email, "OK");
                                    await Navigation.PopAsync();
                                }
                                else
                                {
                                    await DisplayAlert("Connection Error", "Please check your internet connection", "OK");
                                }
                                this.activityIndicator.IsRunning = false;
                            }
                            else
                            {
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
                            this.activityIndicator.IsRunning = false;
                        }

                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        /// <summary>
        /// Function to send an Email
        /// </summary>
        /// <returns></returns>
        private async Task fn_sendEmail()
        {
            Random random = new Random();

            _verificationCode = random.Next(15432, 99999);

            await EmailService.SendEmail(developer, _verificationCode.ToString());

            System.Diagnostics.Debug.WriteLine(_verificationCode);

            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();

            _isTimerStart = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), fn_timeElapsed);
        }

        /// <summary>
        /// Function called when the Cancel Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void fn_cancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Function called when the Email Text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void fn_emailChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            var regex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

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

        /// <summary>
        /// Function Called when the password text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function called when the confirm password text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Function called when the code text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void fn_codeChanged(object sender, EventArgs e)
        {
            Xfx.XfxEntry entry = (Xfx.XfxEntry)sender;

            if (entry.Text == null)
            {
                entry.ErrorText = "Please enter the code";
                _codeValid = false;
                return;
            }

            entry.Text = entry.Text.Trim();
            System.Diagnostics.Debug.WriteLine("-" + entry.Text + "-");
            if (entry.Text == "")
            {
                entry.ErrorText = "Please enter the code";
                _codeValid = false;
            }
            else if (entry.Text.Length != 5)
            {
                entry.ErrorText = "Please enter a valid code";
                _codeValid = false;
            }
            else
            {
                entry.ErrorText = "";
                _codeValid = true;
            }
        }

        /// <summary>
        /// Function called when the Resend Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void fn_resendClicked(object sender, EventArgs e)
        {
            await EmailService.SendEmail(developer, _verificationCode.ToString());

            label_error.IsVisible = false;

            _stopwatch.Restart();

            _isTimerStart = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), fn_timeElapsed);

            button_resend.IsVisible = false;
        }

        /// <summary>
        /// Function called to implement Time Text updation
        /// </summary>
        /// <returns></returns>
        private bool fn_timeElapsed()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                TimeSpan time = _stopwatch.Elapsed;
                TimeSpan leftTime = _maxTime.Subtract(time);

                System.Diagnostics.Debug.WriteLine(time.ToString());

                label_time.Text = leftTime.Minutes.ToString("D2") + ":" + leftTime.Seconds.ToString("D2");

                if (time.TotalSeconds >= _maxTime.TotalSeconds)
                {
                    _stopwatch.Stop();
                    _isTimerStart = false;
                    button_resend.IsVisible = true;
                }
            });
            return _isTimerStart;
        }
    }
}