using System;
using TrainControlApp.Services;
using TrainControlApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainControlApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<TrainDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
