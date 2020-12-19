using System;
using System.ComponentModel;
using TrainControlApp.Models;
using TrainControlApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainControlApp.Views
{
    public partial class TrainPage : ContentPage
    {
        public IDataStore<Train> DataStore => DependencyService.Get<IDataStore<Train>>();

        public TrainPage()
        {
            InitializeComponent();
        }
    }
}
