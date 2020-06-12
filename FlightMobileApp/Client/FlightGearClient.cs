using System;
using System.Text;
using System.Net.Sockets;
using System.Collections;

namespace FlightMobileApp.Client
{
    public class FlightGearClient : IFlighGearClient
    {
        private TcpClient _client;
        // set controls to default double values
        private Hashtable properties = new Hashtable{
            {"/flight/aileron" , 0 },
            {"/flight/elevator" , 0 },
            {"/flight/rudder" , 0 },
            {"/engines/current-engine/throttle" , 0 }
        };
        public FlightGearClient(string server, int port)
        {
            _client = new TcpClient(server, port);
            Connect();
        }

        private void Connect()
        {
            string format = "data\n";

            Byte[] data = Encoding.ASCII.GetBytes(format);
            NetworkStream stream = _client.GetStream();
            stream.Write(data, 0, data.Length);

            stream.Close();
        }

        public bool SetProperty(string property, double value)
        {
            // if value is unchanged, no need to send any request and waste time
            if ((double)properties[property] == value) return true;
            
            NetworkStream stream = _client.GetStream();
            string path = "/controls" + property;
            double returnedValue;

            // send request
            string setProperty = "set " + path + " " + value.ToString() + "\n";
            byte[] setPropertyAsBytes = Encoding.ASCII.GetBytes(setProperty);
            stream.Write(setPropertyAsBytes, 0, setPropertyAsBytes.Length);
            
            // check if value was updated
            string getProperty = "get " + path + "\n";
            byte[] getPropertyAsBytes = Encoding.ASCII.GetBytes(getProperty);
            stream.Write(getPropertyAsBytes, 0, getPropertyAsBytes.Length);

            byte[] answerInBytes = new byte[1024];
            stream.Read(answerInBytes, 0, 1024);
            string answer = Encoding.ASCII.GetString(answerInBytes, 0, answerInBytes.Length);

            if (!Double.TryParse(answer, out returnedValue))
                return false;
            if (returnedValue != value)
                return false;

            properties[property] = returnedValue;
            stream.Close();
            return true;
        }
    }
}
