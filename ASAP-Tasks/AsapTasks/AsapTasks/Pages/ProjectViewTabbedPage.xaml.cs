using AsapTasks.Data;
using AsapTasks.Services;
using AsapTasks.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsapTasks.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectViewTabbedPage : TabbedPage
    {

        #region Private Variables

        #region Task Region Variables

        private List<ProjectTask> _tasks;

        private List<TasksObject> _tasksObjects;

        private TasksObject selectedTask;

        #endregion

        #region Issues Region

        private List<Issue> _issues;

        private List<IssueObject> _issueObjects;

        private IssueObject selectedIssue;

        #endregion

        #region Details

        List<Developer> projectDevelopers;
        List<Developer> otherDevelopers;
        List<MembersModel> members;

        #endregion

        #endregion

        public ProjectViewTabbedPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            tasks_ProjectName.Text = App.selectedProject.Name;
            issues_ProjectName.Text = App.selectedProject.Name;
            insights_ProjectName.Text = App.selectedProject.Name;
            details_ProjectName.Text = App.selectedProject.Name;

            #region Tasks Region

            listview_tasksList.ItemSelected += fn_onTaskSelected;

            listview_tasksList.ItemTapped += fn_taskClicked;

            listview_tasksList.RefreshCommand = TasksRefreshCommand;

            #endregion

            #region Issues Region

            listview_issuesList.ItemSelected += fn_onIssueSelected;

            listview_issuesList.ItemTapped += fn_issueClicked;

            listview_issuesList.RefreshCommand = IssueRefreshCommand;

            #endregion            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            #region Tasks Section

            await fn_tasksRefreshData();

            #endregion

            #region Issues Section

            await fn_issuesRefreshData();

            #endregion

            #region Pie Chart for Tasks Overview
            GenerateTasksChart();
            #endregion

            #region Pie Chart for Issues Region
            GenerateIssuesChart();
            #endregion

            #region Details Page Section

            await fn_detailsRefreshData();

            #region Setting up Project Details

            det_name.Text = App.selectedProject.Name;
            det_desc.Text = App.selectedProject.Description;
            System.Diagnostics.Debug.WriteLine("=================> " + App.selectedProject.StartDate);
            det_date.Text = DateTime.Parse(App.selectedProject.StartDate).ToLongDateString();
            det_stat.Text = App.selectedProject.OpenStatus ? "Open" : "Closed";

            #endregion

            listview_detailsList.RefreshCommand = DetailsRefreshCommand;

            #endregion
        }

        public void fn_backClicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            App.selectedProject = null;
            Navigation.PopAsync();
        }

        #region Tasks Region

        public void fn_onTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            selectedTask = listview_tasksList.SelectedItem as TasksObject;
        }

        private async void fn_taskClicked(object sender, EventArgs e)
        {
            foreach (var t in _tasks)
            {
                if (t.Id == selectedTask.Id)
                {
                    App.selectedTask = t;
                    break;
                }
            }

            // Navigate to edit task
            await Navigation.PushAsync(new NewTaskPage());
        }

        public ICommand TasksRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    listview_tasksList.IsRefreshing = true;

                    await fn_tasksRefreshData();

                    listview_tasksList.IsRefreshing = false;
                });
            }
        }

        public async Task fn_tasksRefreshData()
        {
            _tasks = new List<ProjectTask>();

            _tasksObjects = new List<TasksObject>();

            List<TasksObject> unsorted = new List<TasksObject>();

            if (App.selectedProject.Id != string.Empty)
            {
                List<ProjectTask> tasks = await App.projectTaskManager.GetProjectTaskFromProjectIdAsync(App.selectedProject.Id);

                foreach (var x in tasks)
                {
                    //Task __task = await App.projectTaskManager.GetProjectTaskFromProjectIdAsync(x.ProjectId);

                    _tasks.Add(x);

                    TasksObject tasksObject = new TasksObject();

                    if (x.Description.Length <= 50)
                    {
                        tasksObject.Description = x.Description;
                    }
                    else
                    {
                        tasksObject.Description = x.Description.Substring(0, 47) + "...";
                    }

                    tasksObject.Id = x.Id;

                    tasksObject.CompletionStatus = x.CompletionStatus;

                    switch (x.CompletionStatus)
                    {
                        case "Incomplete":
                            tasksObject.Color = Color.White;
                            break;
                        case "In Progress":
                            tasksObject.Color = Color.FromHex("AFFFAF");
                            break;
                        case "Done":
                            tasksObject.Color = Color.WhiteSmoke;
                            break;
                    }

                    tasksObject.Name = "["+x.CompletionStatus+"] "+x.Name;

                    unsorted.Add(tasksObject);
                }

                // add in progress
                foreach(var t in unsorted)
                {
                    if (t.CompletionStatus == Constants.taskStatus[1])
                        _tasksObjects.Add(t);
                }

                foreach (var t in unsorted)
                {
                    if (t.CompletionStatus == Constants.taskStatus[0])
                        _tasksObjects.Add(t);
                }

                foreach (var t in unsorted)
                {
                    if (t.CompletionStatus == Constants.taskStatus[2])
                        _tasksObjects.Add(t);
                }
            }

            listview_tasksList.ItemsSource = _tasksObjects;
        }

        public async void fn_addTaskClicked(object sender, EventArgs e)
        {
            if (App.selectedProject.OpenStatus)
            {
                App.selectedTask = null;
                await Navigation.PushAsync(new NewTaskPage());
            }
            else
            {
                await DisplayAlert("Closed Project", "Cannot add task to a closed project.", "OK");
            }
        }

        #endregion

        #region Issues Regions

        public void fn_onIssueSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            selectedIssue = listview_issuesList.SelectedItem as IssueObject;
        }

        private async void fn_issueClicked(object sender, EventArgs e)
        {
            foreach (var t in _issues)
            {
                if (t.Id == selectedIssue.Id)
                {
                    App.selectedIssue = t;
                    break;
                }
            }

            // Navigate to edit task
            await Navigation.PushAsync(new NewIssuePage());
        }

        public async void fn_addIssueClicked(object sender, EventArgs e)
        {
            if (App.selectedProject.OpenStatus)
            {
                App.selectedIssue = null;
                await Navigation.PushAsync(new NewIssuePage());
            }
            else
            {
                await DisplayAlert("Closed Project", "Cannot add issues to a closed project.", "OK");
            }
        }

        public ICommand IssueRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    listview_issuesList.IsRefreshing = true;

                    await fn_issuesRefreshData();

                    listview_issuesList.IsRefreshing = false;
                });
            }
        }

        public async Task fn_issuesRefreshData()
        {
            _issues = new List<Issue>();

            _issueObjects = new List<IssueObject>();

            List<IssueObject> unsorted = new List<IssueObject>();

            if (App.selectedProject.Id != string.Empty)
            {
                List<Issue> issues = await App.issueManager.GetIssueFromProjectIdAsync(App.selectedProject.Id);

                foreach (var x in issues)
                {
                    _issues.Add(x);

                    IssueObject issueObject = new IssueObject();

                    if (x.Description.Length <= 50)
                    {
                        issueObject.Description = x.Description;
                    }
                    else
                    {
                        issueObject.Description = x.Description.Substring(0, 47) + "...";
                    }

                    issueObject.Id = x.Id;

                    issueObject.CompletionStatus = x.CompletionStatus;

                    if (!x.CompletionStatus)
                    {
                        issueObject.BackgroundColor = Color.FromHex("cc0000");
                        issueObject.TextColor = Color.White;

                        issueObject.Name = "[Pending] "+x.Name;
                    }
                    else
                    {
                        issueObject.BackgroundColor = Color.WhiteSmoke;
                        issueObject.TextColor = (Color)Application.Current.Resources["color_SpaceCadet"];

                        issueObject.Name = "[Resolved] " + x.Name;
                    }

                    unsorted.Add(issueObject);
                }
                foreach (var t in unsorted)
                {
                    if (!t.CompletionStatus)
                        _issueObjects.Add(t);
                }

                foreach (var t in unsorted)
                {
                    if (t.CompletionStatus)
                        _issueObjects.Add(t);
                }
            }

            listview_issuesList.ItemsSource = _issueObjects;
        }

        #endregion

        #region Insights Region

        public void GenerateTasksChart()
        {
            #region Generate Stats

            int tasks_completed = 0;
            int tasks_pending = 0;
            int tasks_progress = 0;

            foreach(var t in _tasks)
            {
                switch (t.CompletionStatus)
                {
                    case "Incomplete":
                        tasks_pending++;
                        break;
                    case "In Progress":
                        tasks_progress++;
                        break;
                    case "Done":
                        tasks_completed++;
                        break;
                }
            }

            int total_tasks = tasks_completed + tasks_pending + tasks_progress;

            #endregion

            if(total_tasks > 0)
            {
                List<Microcharts.Entry> entries_Tasks = new List<Microcharts.Entry>();

                entries_Tasks.Add(new Microcharts.Entry(tasks_completed)
                {
                    Label = "Completed",
                    Color = SkiaSharp.SKColor.Parse("#337357"),
                    TextColor = SkiaSharp.SKColor.Parse("#337357"),
                    ValueLabel = (((double)tasks_completed / total_tasks) * 100).ToString("0.##") + "%"
                });

                entries_Tasks.Add(new Microcharts.Entry(tasks_pending)
                {
                    Label = "Incomplete",
                    Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                    TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                    ValueLabel = (((double)tasks_pending / total_tasks) * 100).ToString("0.##") + "%"
                });

                entries_Tasks.Add(new Microcharts.Entry(tasks_progress)
                {
                    Label = "In Progress",
                    Color = SkiaSharp.SKColor.Parse("#5FC45A"),
                    TextColor = SkiaSharp.SKColor.Parse("#5FC45A"),
                    ValueLabel = (((double)tasks_progress / total_tasks) * 100).ToString("0.##") + "%"
                });

                var pieChart_Tasks = new Microcharts.DonutChart()
                {
                    Entries = entries_Tasks,
                    BackgroundColor = SkiaSharp.SKColor.Parse("#FFFFFF"),
                    LabelTextSize = 30
                };

                this.chartView_Tasks.Chart = pieChart_Tasks;
            }

            else
            {
                List<Microcharts.Entry> entries_Tasks = new List<Microcharts.Entry>();

                entries_Tasks.Add(new Microcharts.Entry(tasks_completed)
                {
                    Label = "Completed",
                    Color = SkiaSharp.SKColor.Parse("#337357"),
                    TextColor = SkiaSharp.SKColor.Parse("#337357"),
                    ValueLabel = (0).ToString("0.##") + "%"
                });

                entries_Tasks.Add(new Microcharts.Entry(tasks_pending)
                {
                    Label = "Incomplete",
                    Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                    TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                    ValueLabel = (0).ToString("0.##") + "%"
                });

                entries_Tasks.Add(new Microcharts.Entry(tasks_progress)
                {
                    Label = "In Progress",
                    Color = SkiaSharp.SKColor.Parse("#5FC45A"),
                    TextColor = SkiaSharp.SKColor.Parse("#5FC45A"),
                    ValueLabel = (0).ToString("0.##") + "%"
                });

                var pieChart_Tasks = new Microcharts.DonutChart()
                {
                    Entries = entries_Tasks,
                    BackgroundColor = SkiaSharp.SKColor.Parse("#FFFFFF"),
                    LabelTextSize = 30
                };

                this.chartView_Tasks.Chart = pieChart_Tasks;
            }
        }

        public void GenerateIssuesChart()
        {

            #region Generate Stats

            //int issues_closed = 12;
            //int issues_open = 7;
            //int total_issues = issues_closed + issues_open;

            int issues_closed = 0;
            int issues_open = 0;

            foreach(var i in _issues)
            {
                if (i.CompletionStatus)
                    issues_closed++;
                else
                    issues_open++;
            }

            int total_issues = issues_closed + issues_open;

            #endregion

            if (total_issues > 0)
            {

                List<Microcharts.Entry> entries_Issues = new List<Microcharts.Entry>();

                entries_Issues.Add(new Microcharts.Entry(issues_closed)
                {
                    Label = "Resolved",
                    Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                    TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                    ValueLabel = (((double)issues_closed / total_issues) * 100).ToString("0.##") + "%"
                });
                entries_Issues.Add(new Microcharts.Entry(issues_open)
                {
                    Label = "Pending",
                    Color = SkiaSharp.SKColor.Parse("#8A1A0C"),
                    TextColor = SkiaSharp.SKColor.Parse("#8A1A0C"),
                    ValueLabel = (((double)issues_open / total_issues) * 100).ToString("0.##") + "%"
                });

                var pieChart_Issues = new Microcharts.DonutChart()
                {
                    Entries = entries_Issues,
                    BackgroundColor = SkiaSharp.SKColor.Parse("#FFFFFF"),
                    LabelTextSize = 30
                };

                this.chartView_Issues.Chart = pieChart_Issues;

            }
            else
            {
                List<Microcharts.Entry> entries_Issues = new List<Microcharts.Entry>();

                entries_Issues.Add(new Microcharts.Entry(issues_closed)
                {
                    Label = "Resolved",
                    Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                    TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                    ValueLabel = (0).ToString("0.##") + "%"
                });
                entries_Issues.Add(new Microcharts.Entry(issues_open)
                {
                    Label = "Pending",
                    Color = SkiaSharp.SKColor.Parse("#8A1A0C"),
                    TextColor = SkiaSharp.SKColor.Parse("#8A1A0C"),
                    ValueLabel = (0).ToString("0.##") + "%"
                });

                var pieChart_Issues = new Microcharts.DonutChart()
                {
                    Entries = entries_Issues,
                    BackgroundColor = SkiaSharp.SKColor.Parse("#FFFFFF"),
                    LabelTextSize = 30
                };

                this.chartView_Issues.Chart = pieChart_Issues;
            }
        }

        public void fn_refreshClicked(object sender, EventArgs e)
        {
            GenerateTasksChart();
            GenerateIssuesChart();
        }

        #endregion

        #region Details Region

        public async void fn_detEditClicked(object sender, EventArgs e)
        {
            // Redirect to new Project Page
            await Navigation.PushAsync(new NewProjectPage());
        }

        public async void fn_inviteClicked(object sender, EventArgs e)
        {
            if(comboBox_email.Text == null || comboBox_email.Text == "")
            {
                return;
            }
            // check if developer exists
            Developer dev = await App.developerManager.CheckDeveloperEmailAsync(comboBox_email.Text);
            if(dev == null)
            {
                await DisplayAlert("No Developer found", "No Developer found with the email: \"" + comboBox_email.Text + "\"", "OK");
            }
            else
            {
                Enrollment enr = await App.enrollmentManager.CheckEnrollmentAsync(App.selectedProject.Id, dev.Id);

                if (enr == null)
                {

                    Enrollment enrollment = new Enrollment();
                    enrollment.AcceptStatus = false;
                    enrollment.DeveloperId = dev.Id;
                    enrollment.ProjectId = App.selectedProject.Id;

                    await App.enrollmentManager.SaveEnrollmentAsync(enrollment);

                    await EmailService.SendEmail(dev, App.selectedProject, App.developer);

                    await DisplayAlert("Invite Sent", "Invite Sent to: \"" + comboBox_email.Text + "\"", "OK");
                }
                else
                {
                    await DisplayAlert("Invalid Request", "Memeber \"" + comboBox_email.Text + "\" is already in the project", "OK");
                }
            }
        }

        public ICommand DetailsRefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    listview_detailsList.IsRefreshing = true;

                    await fn_detailsRefreshData();

                    listview_detailsList.IsRefreshing = false;
                });
            }
        }

        public async Task fn_detailsRefreshData()
        {
            #region Setting up auto-complete

            members = new List<MembersModel>();

            ObservableCollection<Developer> allDevelopers = await App.developerManager.GetDevelopersAsync();

            List<Enrollment> currentEnrollments = await App.enrollmentManager.GetEnrollmentFromProjectIdAsync(App.selectedProject.Id);

            projectDevelopers = new List<Developer>();
            otherDevelopers = new List<Developer>();
            List<string> emails = new List<string>();

            foreach (var dev in allDevelopers)
            {
                bool devSelected = false;
                foreach (var enr in currentEnrollments)
                {
                    if (enr.DeveloperId == dev.Id)
                    {
                        devSelected = true;
                        projectDevelopers.Add(dev);

                        MembersModel member = new MembersModel();
                        member.Name = dev.Name;
                        member.Status = enr.AcceptStatus ? "[Accepted]" : "[Pending]";
                        if (enr.AcceptStatus)
                        {
                            member.Color = (Color)Application.Current.Resources["color_White"];
                        }
                        else
                        {
                            member.Color = (Color)Application.Current.Resources["color_Error"];
                        }

                        members.Add(member);

                        break;
                    }
                }
                if (!devSelected)
                {
                    otherDevelopers.Add(dev);
                    emails.Add(dev.Email);
                }
            }

            ComboBoxViewModel viewModel = new ComboBoxViewModel();

            ComboBoxViewModel._emails = emails;

            comboBox_email.BindingContext = viewModel;

            #endregion

            #region Generating List View Model            

            listview_detailsList.ItemsSource = members;

            #endregion 
        }

        #endregion
    }
}