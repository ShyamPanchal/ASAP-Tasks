using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AsapTasks.Pages;
using AsapTasks.Data;
using AsapTasks.Services;
using AsapTasks.Managers;
using System.Collections.Generic;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AsapTasks
{
    public partial class App : Application
    {
        public static Developer developer;

        public static EnrollmentManager enrollmentManager;

        public static ProjectManager projectManager;

        public static DeveloperManager developerManager;

        public static ProjectTaskManager projectTaskManager;

        public static IssueManager issueManager;

        public static Project selectedProject;

        public static ProjectTask selectedTask;

        public static Issue selectedIssue;

        public static Enrollment selectedEnrollment;

        public App()
        {
            InitializeComponent();

            developer = new Developer();

            NavigationPage mainNavigationPage;

            if (Settings.DeveloperId == string.Empty)
            {
                mainNavigationPage = new NavigationPage(new MainPage());
            }
            else
            {
                mainNavigationPage = new NavigationPage(new ProjectsHomePage());
            }

            //mainNavigationPage = new NavigationPage(new NewProjectPage());

            MainPage = mainNavigationPage;

        }

        protected override void OnStart()
        {
            enrollmentManager = EnrollmentManager.DefaultManager;

            projectManager = ProjectManager.DefaultManager;

            developerManager = DeveloperManager.DefaultManager;

            projectTaskManager = ProjectTaskManager.DefaultManager;

            issueManager = IssueManager.DefaultManager;
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
