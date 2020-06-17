using System;
using System.Threading.Tasks;

namespace FlightMobileApp.Client
{
    public interface ITcpClient
    {
        bool SetProperty(string propertyName, double value);
        Task<byte[]> GetScreenshot();
    }
}
