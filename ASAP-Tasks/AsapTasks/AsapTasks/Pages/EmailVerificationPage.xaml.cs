using AsapTasks.Data;
using AsapTasks.Managers;
using AsapTasks.Services;
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
    public partial class EmailVerificationPage : ContentPage
    {
        #region Private Variables
        private int _verificationCode;

        private bool _codeValid;

        private System.Diagnostics.Stopwatch _stopwatch;

        private bool _isTimerStart;

        private DeveloperManager developerManager;

        private Developer developer;

        //private TimeSpan _maxTime = new TimeSpan(0, 3, 0);
        private TimeSpan _maxTime = new TimeSpan(0, 0, 10);
        #endregion

        public EmailVerificationPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _codeValid = false;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            this.activityIndicator.IsRunning = true;

            developerManager = DeveloperManager.DefaultManager;

            developer = BindingContext as Developer;

            Random random = new Random();

            _verificationCode = random.Next(15432, 99999);

            await EmailService.SendEmail(developer, _verificationCode.ToString());

            System.Diagnostics.Debug.WriteLine(_verificationCode);

            _stopwatch = new System.Diagnostics.Stopwatch();
            _stopwatch.Start();

            _isTimerStart = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), fn_timeElapsed);

            this.activityIndicator.IsRunning = false;
        }

        public async void fn_submitClicked(object sender, EventArgs e)
        {
            try
            {
                this.activityIndicator.IsRunning = true;

                string __code = entry_code.Text;
                if (_codeValid)
                    _codeValid = !(__code == "");

                if (_codeValid)
                {
                    if (entry_code.Text == _verificationCode.ToString())
                    {
                        developer.IsVerified = true;

                        var check = await developerManager.SaveDeveloperAsync(developer);

                        if (check == Constants.DataEntryErrors.SUCCESS)
                        {
                            this.activityIndicator.IsRunning = false;

                            await DisplayAlert("Registration", "Verification and Registration Successful", "OK");
                            await Navigation.PopToRootAsync();
                        }
                        else
                        {
                            this.activityIndicator.IsRunning = false;
                            await DisplayAlert("Connection Error", "Please check your internet connection", "OK");
                        }
                    }
                    else
                    {
                        this.activityIndicator.IsRunning = false;
                        await DisplayAlert("Registration Failed", "Verification Code did not match.", "RETRY");
                        return;
                    }
                }
                else
                {
                    this.activityIndicator.IsRunning = false;
                    fn_codeChanged(entry_code, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.activityIndicator.IsRunning = false;
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public async void fn_resendClicked(object sender, EventArgs e)
        {
            this.activityIndicator.IsRunning = true;
            await EmailService.SendEmail(developer, _verificationCode.ToString());

            label_error.IsVisible = false;

            _stopwatch.Restart();

            _isTimerStart = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), fn_timeElapsed);

            button_resend.IsVisible = false;
            this.activityIndicator.IsRunning = false;
        }

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

        public async void fn_cancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

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