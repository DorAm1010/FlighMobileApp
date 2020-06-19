using FlightMobileApp.Models;
using System;
using System.Threading.Tasks;

namespace FlightMobileApp.Client
{
    public interface ITcpClient
    {
        Task<bool> Execute(Command command);
        Task<byte[]> GetScreenshot();
    }
}
