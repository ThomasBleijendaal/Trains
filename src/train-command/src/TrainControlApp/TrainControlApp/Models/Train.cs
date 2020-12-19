using System;

namespace TrainControlApp.Models
{
    public class Train
    {
        public string Id { get; set; }
        public int MinSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public int CurrentSpeed { get; set; }
    }
}
