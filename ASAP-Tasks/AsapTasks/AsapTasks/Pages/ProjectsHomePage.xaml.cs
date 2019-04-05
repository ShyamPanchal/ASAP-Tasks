using AsapTasks.Data;
using AsapTasks.Managers;
using AsapTasks.ViewModel;
using Plugin.Connectivity;
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
        #region Private Variables

        private List<Project> _projects;

        public List<ProjectObject> _projectObjects;

        private ProjectObject selectedProject;

        private int countOpen, countClosed;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectsHomePage ()
		{
			InitializeComponent ();

            NavigationPage.SetHasNavigationBar(this, false);

            listview_projectList.ItemSelected += fn_onItemSelected;

            listview_projectList.ItemTapped += fn_itemClicked;

            listview_projectList.RefreshCommand = RefreshCommand;
        }

        /// <summary>
        /// Function called when the page components are ready to be rendered
        /// </summary>
        protected async override void OnAppearing()
        {

            if (!CrossConnectivity.Current.IsConnected)
            {
                Settings.DeveloperId = string.Empty;

                App.developer = new Developer();

                await Navigation.PushAsync(new MainPage());

                Navigation.RemovePage(this);
            }

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
                    this.activityIndicator.IsRunning = true;

                    Developer developer = await App.developerManager.GetDeveloperFromIdAsync(Settings.DeveloperId);

                    this.activityIndicator.IsRunning = false;

                    App.developer = developer;
                }
            }

            #endregion

            text_userName.Text = App.developer.Name;

            #region Get Projects List 
            this.activityIndicator.IsRunning = true;

            await fn_refreshData();

            this.activityIndicator.IsRunning = false;
            #endregion

            listview_projectList.ItemsSource = _projectObjects;
            listview_projectList.SelectionMode = ListViewSelectionMode.Single;

            base.OnAppearing();
        }

        /// <summary>
        /// Function called when list item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void fn_onItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            selectedProject = listview_projectList.SelectedItem as ProjectObject;
        }

        /// <summary>
        /// Function called when Logout Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void fn_logoutClicked(object sender, EventArgs e)
        {
            Settings.DeveloperId = string.Empty;

            App.developer = new Developer();

            await Navigation.PushAsync(new MainPage());

            Navigation.RemovePage(this);
        }

        /// <summary>
        /// Function called when List Item is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (selectedProject.enrollment.AcceptStatus)
            {
                await Navigation.PushAsync(new ProjectViewTabbedPage());
            }
            else
            {
                App.selectedEnrollment = selectedProject.enrollment;
                await Navigation.PushAsync(new ProjectInvitePage());
            }
        }

        /// <summary>
        /// Function called when New Project Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void fn_NewProjectClicked(object sender, EventArgs e)
        {
            App.selectedProject = null;

            await Navigation.PushAsync(new NewProjectPage());
        }

        /// <summary>
        /// Command to Refresh the List
        /// </summary>
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

        /// <summary>
        /// Function that refreshed the List Data
        /// </summary>
        /// <returns></returns>
        public async Task fn_refreshData()
        {
            _projects = new List<Project>();

            _projectObjects = new List<ProjectObject>();

            List<ProjectObject> __unsorted = new List<ProjectObject>();

            countOpen = countClosed = 0;

            if (App.developer.Id != string.Empty)
            {
                List<Enrollment> enrollments = await App.enrollmentManager.GetEnrollmentFromIdAsync(App.developer.Id);

                foreach (var x in enrollments)
                {
                    Project __project = await App.projectManager.GetProjectFromIdAsync(x.ProjectId);

                    _projects.Add(__project);

                    ProjectObject projectObject = new ProjectObject();

                    projectObject.enrollment = x;

                    if (__project.Description.Length <= 50)
                    {
                        projectObject.Description = __project.Description;
                    }
                    else
                    {
                        projectObject.Description = __project.Description.Substring(0, 47) + "...";
                    }

                    projectObject.Id = __project.Id;

                    projectObject.OpenStatus = __project.OpenStatus;

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

                    __unsorted.Add(projectObject);
                }
            }

            // Add project invites
            foreach(var po in __unsorted)
            {
                if (!po.AcceptStatus)
                {
                    _projectObjects.Add(po);
                }
            }

            // Add open projects
            foreach (var po in __unsorted)
            {
                if (po.AcceptStatus && po.OpenStatus)
                {
                    _projectObjects.Add(po);
                }
            }

            // Add closed projects
            foreach (var po in __unsorted)
            {
                if (po.AcceptStatus && !po.OpenStatus)
                {
                    _projectObjects.Add(po);
                }
            }

            listview_projectList.ItemsSource = _projectObjects;

            label_count.Text = countOpen + " open projects and " + countClosed + " closed projects";
            label_count.HorizontalOptions = LayoutOptions.Center;
        }
    }
}