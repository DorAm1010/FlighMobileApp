﻿using System;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using FlightMobileApp.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace FlightMobileApp.Client
{
    public class FlightGearTcpClient : ITcpClient
    {
        private readonly string aileron = "/controls/flight/aileron";
        private readonly string elevator = "/controls/flight/elevator";
        private readonly string rudder = "/controls/flight/rudder";
        private readonly string throttle = "/controls/engines/current-engine/throttle";
        private TcpClient _client;
        private NetworkStream _stream;
        private BlockingCollection<AsyncCommand> _queue;

        public FlightGearTcpClient(string server, int port)
        {
            _client = new TcpClient();
            _queue = new BlockingCollection<AsyncCommand>();
            Connect(server, port);
            Start();
        }

        private void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }

        private void Connect(string server, int port)
        {
            _client.Connect(server, port);
            _stream = _client.GetStream();
            string format = "data\r\n";
            Byte[] data = Encoding.ASCII.GetBytes(format);
            _stream.Write(data, 0, data.Length);
        }

        public Task<Result> Execute(Command command)
        {
            var asyncCommand = new AsyncCommand(command);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }

        private void ProcessCommands()
        {
            foreach (AsyncCommand command in _queue.GetConsumingEnumerable())
            {
                if(!SetProperty(aileron, command.Command.Aileron))
                    command.Completion.SetResult(Result.NotOk);
                if(!SetProperty(elevator, command.Command.Elevator))
                    command.Completion.SetResult(Result.NotOk);
                if(!SetProperty(rudder, command.Command.Rudder))
                    command.Completion.SetResult(Result.NotOk);
                if(!SetProperty(throttle, command.Command.Throttle))
                    command.Completion.SetResult(Result.NotOk);
                
                command.Completion.SetResult(Result.Ok);
            }
        }

        public bool SetProperty(string property, double value)
        {
            double returnedValue;
            // send request
            string setProperty = "set " + property + " " + value.ToString() + "\r\n";
            byte[] setPropertyAsBytes = Encoding.ASCII.GetBytes(setProperty);
            _stream.Write(setPropertyAsBytes, 0, setPropertyAsBytes.Length);

            // check if value was updated
            string getProperty = "get " + property + "\r\n";
            byte[] getPropertyAsBytes = Encoding.ASCII.GetBytes(getProperty);
            _stream.Write(getPropertyAsBytes, 0, getPropertyAsBytes.Length);

            byte[] answerInBytes = new byte[256];
            _stream.Read(answerInBytes, 0, 256);
            string answer = Encoding.ASCII.GetString(answerInBytes, 0, answerInBytes.Length);

            if (!Double.TryParse(answer, out returnedValue))
                return false;
            if (returnedValue != value)
                return false;
            return true;
        }
        public async Task<byte[]> GetScreenshot()
        {
            string ScreenshotAddress = "http://localhost:5000/screenshot";
            string ScreenshotTemp = "./statusImg.jpg";
            byte[] screenshotBytes;
            HttpWebRequest getImage = (HttpWebRequest)WebRequest.Create(new Uri(ScreenshotAddress));
            WebResponse RecvImage = getImage.GetResponse();
            Stream response = RecvImage.GetResponseStream();
            using (BinaryReader bReader = new BinaryReader(response))
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
