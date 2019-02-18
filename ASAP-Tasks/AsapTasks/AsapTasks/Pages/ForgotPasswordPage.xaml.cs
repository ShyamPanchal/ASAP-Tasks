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
	public partial class ForgotPasswordPage : ContentPage
	{
		public ForgotPasswordPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void Btn_phoneNumber_Clicked(object sender, EventArgs e)
        {

        }
    }
}