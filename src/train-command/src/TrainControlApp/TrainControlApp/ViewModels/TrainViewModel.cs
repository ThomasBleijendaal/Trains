using System;
using System.Collections.Generic;
using System.Windows.Input;
using TrainControlApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TrainControlApp.ViewModels
{
    public class TrainViewModel : BaseViewModel
    {
        public IReadOnlyList<Train> Trains { get; private set; }

        public TrainViewModel()
        {
            Title = "Trains";

            UpdateTrains();
        }

        private void UpdateTrains()
        {
            Trains = DataStore.GetItems();
        }

        public ICommand StopCommand
        {
            get
            {
                return new Command((e) =>
                {
                    var item = (e as Train);

                    item.CurrentSpeed = 0;

                    DataStore.UpdateItem(item);
                    UpdateTrains();
                });
            }
        }

        public ICommand FasterCommand
        {
            get
            {
                return new Command((e) =>
                {
                    var item = (e as Train);

                    item.CurrentSpeed += 100;

                    DataStore.UpdateItem(item);
                    UpdateTrains();
                });
            }
        }
        public ICommand SlowerCommand
        {
            get
            {
                return new Command((e) =>
                {
                    var item = (e as Train);

                    item.CurrentSpeed -= 100;

                    DataStore.UpdateItem(item);
                    UpdateTrains();
                });
            }
        }
    }
}
