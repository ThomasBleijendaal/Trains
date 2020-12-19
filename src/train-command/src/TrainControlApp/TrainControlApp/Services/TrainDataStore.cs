using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SharpOSCCore;
using TrainControlApp.Models;

namespace TrainControlApp.Services
{
    public class TrainDataStore : IDataStore<Train>
    {
        readonly UDPSender _sender = new UDPSender("192.168.2.255", 15670);

        readonly List<Train> _trains;

        public TrainDataStore()
        {
            _trains = new List<Train>()
            {
                new Train { Id = "train/1", CurrentSpeed = 0, MaxSpeed = 1023, MinSpeed = 500 },
                new Train { Id = "train/2", CurrentSpeed = 0, MaxSpeed = 1023, MinSpeed = 600 },
            };

            Update();
        }

        public bool UpdateItem(Train item)
        {
            var savedItem = _trains.Where((Train arg) => arg.Id == item.Id).FirstOrDefault();

            savedItem.CurrentSpeed = item.CurrentSpeed;

            if (savedItem.CurrentSpeed > savedItem.MaxSpeed)
            {
                savedItem.CurrentSpeed = savedItem.MaxSpeed;
            }
            if (savedItem.CurrentSpeed < savedItem.MinSpeed && savedItem.CurrentSpeed > 0)
            {
                savedItem.CurrentSpeed = savedItem.MinSpeed;
            }
            if (savedItem.CurrentSpeed < 0)
            {
                savedItem.CurrentSpeed = 0;
            }

            Update();

            return true;
        }

        public IReadOnlyList<Train> GetItems(bool forceRefresh = false)
        {
            return _trains;
        }

        private void Update()
        {
            foreach (var train in _trains)
            {
                var message = new OscMessage(train.Id, new IPEndPoint(IPAddress.Parse("192.168.2.10"), 15670), train.CurrentSpeed);
                _sender.Send(message);
            }
        }
    }
}
