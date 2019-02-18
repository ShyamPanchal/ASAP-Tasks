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
    public partial class ProjectViewTabbedPage : TabbedPage
    {
        public ProjectViewTabbedPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
 
            #region Pie Chart for Tasks Overview
            List<Microcharts.Entry> entries_Tasks = new List<Microcharts.Entry>();
            int tasks_completed = 12;
            int tasks_pending = 7;
            int tasks_progress = 2;
            int total_tasks = tasks_completed + tasks_pending + tasks_progress;
            entries_Tasks.Add(new Microcharts.Entry(tasks_completed)
            {
                Label = "Completed",
                Color = SkiaSharp.SKColor.Parse("#337357"),
                TextColor = SkiaSharp.SKColor.Parse("#337357"),
                ValueLabel = (((double)tasks_completed/total_tasks)*100).ToString("0.##") +"%"
            });
            entries_Tasks.Add(new Microcharts.Entry(tasks_pending)
            {
                Label = "Pending",
                Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                ValueLabel = (((double)tasks_pending / total_tasks)*100).ToString("0.##") + "%"
            });
            entries_Tasks.Add(new Microcharts.Entry(tasks_progress)
            {
                Label = "In Progress",
                Color = SkiaSharp.SKColor.Parse("#5FC45A"),
                TextColor = SkiaSharp.SKColor.Parse("#5FC45A"),
                ValueLabel = (((double)tasks_progress / total_tasks)*100).ToString("0.##") + "%"
            });

            var pieChart_Tasks = new Microcharts.DonutChart() {
                Entries = entries_Tasks,
                BackgroundColor = SkiaSharp.SKColor.Parse("#FFFFFF"),
                LabelTextSize = 30
            };

            this.chartView_Tasks.Chart = pieChart_Tasks;
            #endregion

            #region Pie Chart for Issues Region
            List<Microcharts.Entry> entries_Issues = new List<Microcharts.Entry>();
            int issues_closed = 12;
            int issues_open = 7;
            int total_issues = issues_closed + issues_open;
            entries_Issues.Add(new Microcharts.Entry(issues_closed)
            {
                Label = "Closed",
                Color = SkiaSharp.SKColor.Parse("#d8d8d8"),
                TextColor = SkiaSharp.SKColor.Parse("#686E77"),
                ValueLabel = (((double)issues_closed / total_issues) * 100).ToString("0.##") + "%"
            });
            entries_Issues.Add(new Microcharts.Entry(issues_open)
            {
                Label = "Open",
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
            #endregion
        }

        public async void fn_addTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewTaskPage());
        }

        public async void fn_addIssueClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewIssuePage());
        }
    }
}