using Sobolev.Capstone.Connectors;
using System.Threading.Tasks;

namespace Sobolev.Capstone.Commands
{
    public static class SshGPIOCommand
    {
        public static void Run(string commandText)
        {
            if (!HttpConnector.IsRaspberryAvailable) return;

            Task.Run(() =>
            {
                SshConnector.SshReceiver.RunCommand(commandText);
            });
        }
    }
}
