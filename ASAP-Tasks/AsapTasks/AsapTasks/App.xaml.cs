using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AsapTasks.Pages;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AsapTasks
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            NavigationPage mainNavigationPage = new NavigationPage(new MainPage());
            MainPage = mainNavigationPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
