using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Renci.SshNet;
using Sobolev.Capstone.PreferencesStorage;

namespace Sobolev.Capstone.Connectors
{
    public static class SshConnector
    {
        private static string raspberryUser = InternalStorageProvider.CurrentPreferneces.RaspberryUser;
        public static string RaspberryUser
        {
            get => raspberryUser;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    raspberryUser = value;                    
                    Connect();
                }
                else
                    throw new Exception("Invalid user name for SSH connection.");
            }
        }

        private static string raspberryPwd = "raspberry";
        public static string RaspberryPwd
        {
            get => raspberryPwd;
            
            set
            {
                raspberryPwd = value;
                Connect();
            }
        }

        private static string RaspberryHost { get; set; } = InternalStorageProvider.CurrentPreferneces.RaspberryHost;

        public static SshClient SshReceiver { get; private set; } = null;

        public static void Connect(string RaspberryIP)
        {
            RaspberryHost = RaspberryIP;
            Connect();
        }

        private static void Connect()
        {
            //Debug.WriteLine($"{RaspberryHost}-------------");

            if (SshReceiver != null)
            {
                SshReceiver.Disconnect();
                SshReceiver.Dispose();
            }

            // --------------- exception on the start stage --------------- 
            SshReceiver = new SshClient(RaspberryHost, RaspberryUser, RaspberryPwd);
            SshReceiver.Connect();
        } 
    }
}