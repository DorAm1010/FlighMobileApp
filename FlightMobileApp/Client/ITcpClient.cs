using FlightMobileApp.Models;
using System;
using System.Threading.Tasks;

namespace FlightMobileApp.Client
{
    public interface ITcpClient
    {
        Task<Result> Execute(Command command);
        Task<byte[]> GetScreenshot();
    }
}
