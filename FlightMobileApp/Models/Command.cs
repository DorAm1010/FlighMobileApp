namespace FlightMobileApp.Models
{
    public class Command
    {
        public Command(double aileron, double rudder, double elevator, double throttle)
        {
            Aileron = aileron;
            Rudder = rudder;
            Elevator = elevator;
            Throttle = throttle;
        }

        public Command()
        {
        }

        public double Aileron { get; set; }
        public double Rudder { get; set; }
        public double Elevator { get; set; }
        public double Throttle { get; set; }
    }
}
