﻿using AsapTasks.Data;
using AsapTasks.Managers;
using AsapTasks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsapTasks.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProjectsHomePage : ContentPage
	{
        private List<Project> _projects;

        public List<ProjectObject> _projectObjects;

        public ProjectObject selectedProject;

        public int countOpen, countClosed;

		public ProjectsHomePage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);

            listview_projectList.ItemSelected += fn_onItemSelected;

            listview_projectList.ItemTapped += fn_itemClicked;

            listview_projectList.RefreshCommand = RefreshCommand;
        }

        protected async override void OnAppearing()
        {
            #region Getting developer

            if (App.developer.Id == null || App.developer.Id == string.Empty)
            {
                if(Settings.DeveloperId == string.Empty)
                {
                    // Error !!
                    Settings.DeveloperId = string.Empty;

                    App.developer = new Developer();

                    await Navigation.PushAsync(new MainPage());
                    Navigation.RemovePage(this);
                }
                else
                {
                    Developer developer = await App.developerManager.GetDeveloperFromIdAsync(Settings.DeveloperId);

                    App.developer = developer;
                }
            }

            #endregion

            text_userName.Text = App.developer.Name;

            #region Get Projects List 

            await fn_refreshData();

            #endregion

            listview_projectList.ItemsSource = _projectObjects;
            listview_projectList.SelectionMode = ListViewSelectionMode.Single;

            base.OnAppearing();
        }

        public void fn_onItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            selectedProject = listview_projectList.SelectedItem as ProjectObject;
        }

        private async void fn_logoutClicked(object sender, EventArgs e)
        {
            Settings.DeveloperId = string.Empty;

            App.developer = new Developer();

            await Navigation.PushAsync(new MainPage());
            Navigation.RemovePage(this);
        }

        private async void fn_itemClicked(object sender, EventArgs e)
        {
            foreach(var p in _projects)
            {
                if(p.Id == selectedProject.Id)
                {
                    App.selectedProject = p;
                    break;
                }
            }
            
            await Navigation.PushAsync(new ProjectViewTabbedPage());
        }

        public async void fn_NewProjectClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewProjectPage());
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    listview_projectList.IsRefreshing = true;

                    await fn_refreshData();

                    listview_projectList.IsRefreshing = false;
                });
            }
        }

        public async Task fn_refreshData()
        {
            _projects = new List<Project>();

            _projectObjects = new List<ProjectObject>();

            countOpen = countClosed = 0;

            if (App.developer.Id != string.Empty)
            {
                List<Enrollment> enrollments = await App.enrollmentManager.GetEnrollmentFromIdAsync(App.developer.Id);
                //List<Enrollment> enrollments = await App.enrollmentManager.GetAllEnrollments();

                foreach (var x in enrollments)
                {
                    Project __project = await App.projectManager.GetProjectFromIdAsync(x.ProjectId);

                    _projects.Add(__project);

                    ProjectObject projectObject = new ProjectObject();

                    if (__project.Description.Length <= 50)
                    {
                        projectObject.Description = __project.Description;
                    }
                    else
                    {
                        projectObject.Description = __project.Description.Substring(0, 47) + "...";
                    }

                    projectObject.Id = __project.Id;

                    projectObject.CompletionStatus = __project.OpenStatus;

                    projectObject.AcceptStatus = x.AcceptStatus;

                    if (x.AcceptStatus)
                    {
                        if (__project.OpenStatus)
                        {
                            projectObject.Color = Color.White;
                            projectObject.Name = __project.Name;
                        }
                        else
                        {
                            projectObject.Color = Color.WhiteSmoke;
                            projectObject.Name = "[Closed] " + __project.Name;
                        }
                    }
                    else
                    {
                        projectObject.Name = "[Invite] " + __project.Name;

                        projectObject.Color = Color.FromHex("AFFFAF");
                    }

                    if (__project.OpenStatus)
                        countOpen++;
                    else
                        countClosed++;

                    _projectObjects.Add(projectObject);
                }
            }

            listview_projectList.ItemsSource = _projectObjects;

            label_count.Text = countOpen + " open projects and " + countClosed + " closed projects";
            label_count.HorizontalOptions = LayoutOptions.Center;
        }
    }
}