﻿using System;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Policy;
using System.IO;

namespace FlightMobileApp.Client
{
    public class FlightGearTcpClient : ITcpClient
    {
        private TcpClient _client;
        private NetworkStream stream;
        // set controls to default double values
        private Hashtable properties = new Hashtable{
            {"/flight/aileron" , 0.0 },
            {"/flight/elevator" , 0.0 },
            {"/flight/rudder" , 0.0 },
            {"/engines/current-engine/throttle" , 0.0 }
        };
        public FlightGearTcpClient(string server, int port)
        {
            _client = new TcpClient();
            Connect(server, port);
        }

        private void Connect(string server, int port)
        {
            _client.Connect(server, port);
            string format = "data\n";
            Byte[] data = Encoding.ASCII.GetBytes(format);
            stream = _client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public bool SetProperty(string property, double value)
        {
            // if value is unchanged, no need to send any request and waste time
            if ((double)properties[property] == value) return true;
            
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

            byte[] answerInBytes = new byte[256];
            stream.Read(answerInBytes, 0, 256);
            string answer = Encoding.ASCII.GetString(answerInBytes, 0, answerInBytes.Length);

            if (!Double.TryParse(answer, out returnedValue))
                return false;
            if (returnedValue != value)
                return false;

            properties[property] = returnedValue;
            return true;
        }

        public async Task<byte[]> GetScreenshot()
        {

            string ScreenshotAddress = "http://localhost:8080/screenshot";
            string ScreenshotTemp = "./statusImg.jpg";
            byte[] screenshotBytes;
            HttpWebRequest getImage = (HttpWebRequest)WebRequest.Create(new Uri(ScreenshotAddress));
            WebResponse RecvImage =  getImage.GetResponse();
            Stream response =  RecvImage.GetResponseStream();
            using(BinaryReader bReader = new BinaryReader(response))
            {
                screenshotBytes = bReader.ReadBytes(500000);
                bReader.Close();
            }
            response.Close();
            RecvImage.Close();
            FileStream ImgFile = new FileStream(ScreenshotTemp, FileMode.Create);
            BinaryWriter ImageWriter = new BinaryWriter(ImgFile);
            try
            {
                ImageWriter.Write(screenshotBytes);
            }
            finally
            {
                ImgFile.Close();
                RecvImage.Close();
            }

            return screenshotBytes;
        }
    }


}
