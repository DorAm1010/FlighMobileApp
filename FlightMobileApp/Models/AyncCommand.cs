using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public enum Result { Ok = 200, NotOk = 301 }
    public class AsyncCommand
    {
        public Command Command { get; private set; }
        public Task<Result> Task { get => Completion.Task; }
        public TaskCompletionSource<Result> Completion { get; private set; }

        public AsyncCommand(Command cmd)
        {
            Command = cmd;
            Completion = new TaskCompletionSource<Result>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
