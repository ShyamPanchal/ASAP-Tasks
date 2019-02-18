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
	public partial class ProjectsHomePage : ContentPage
	{
        List<string> demoList;
		public ProjectsHomePage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);

            listview_projectList.ItemSelected += fn_onItemSelected;

            demoList = new List<string>();
            demoList.Add("asdf");
            demoList.Add("asdf");
            demoList.Add("asdf");
            demoList.Add("asdf");
            demoList.Add("asdf");
            demoList.Add("asdf");

            listview_projectList.ItemsSource = demoList;
        }

        public void fn_onItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            listview_projectList.SelectedItem = null;           
        }

        private void Btn_Logout_Clicked(object sender, EventArgs e)
        {

        }
    }
}