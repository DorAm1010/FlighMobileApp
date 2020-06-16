using System.Collections;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class AsyncCommand
    {
        public Command Command { get; private set; }
        public Task<bool> Task { get => Completion.Task; }
        public TaskCompletionSource<bool> Completion { get; private set; }

        public AsyncCommand(Command cmd)
        {
            Command = cmd;
            Completion = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
