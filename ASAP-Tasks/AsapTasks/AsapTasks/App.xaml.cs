using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AsapTasks.Pages;
using AsapTasks.Data;
using AsapTasks.Services;
using AsapTasks.Managers;
using System.Collections.Generic;
using Plugin.Connectivity;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AsapTasks
{
    public partial class App : Application
    {
        #region Application Variables

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

        #endregion

        /// <summary>
        /// Application Constructor
        /// </summary>
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

        /// <summary>
        /// Function Called when application starts
        /// </summary>
        protected override void OnStart()
        {
            enrollmentManager = EnrollmentManager.DefaultManager;

            projectManager = ProjectManager.DefaultManager;

            developerManager = DeveloperManager.DefaultManager;

            projectTaskManager = ProjectTaskManager.DefaultManager;

            issueManager = IssueManager.DefaultManager;
        }
    }
}
