using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Timers;
using Sobolev.Capstone.PreferencesStorage;

// create timer and check availability of connection and check IP address
// if value of address is changed or connection is unavailable - fire appropriate events

namespace Sobolev.Capstone.Connectors
{
    public sealed class ConnectionStatusEventArgs : EventArgs
    {
        public bool IsRaspberryAvailable { get; set; } = false;
    }

    public static class HttpConnector
    {
        private static readonly string FirebaseAddress = InternalStorageProvider.CurrentPreferneces.FirebaseAddress;
        
        private static readonly int DefaultInterval = InternalStorageProvider.CurrentPreferneces.DefaultInterval;
        public static readonly int MinInterval = InternalStorageProvider.CurrentPreferneces.MaxtInterval;
        public static readonly int MaxtInterval = InternalStorageProvider.CurrentPreferneces.MinInterval;

        private static int Interval = 0;
        private static bool CurrentStatus = false;

        private static Timer updateTimer = new Timer();
        private static Ping RaspberryPinger = new Ping();
        public static event EventHandler<ConnectionStatusEventArgs> ConnectionStateChanged;

        static HttpConnector()
        {
            //Debug.WriteLine("HttpConnector() is created!!");

            Interval = DefaultInterval;
            CurrentStatus = IsRaspberryAvailable;
            updateTimer.Interval = Interval;
            updateTimer.Enabled = true;
            updateTimer.Elapsed += UpdateConnectionStatus;
            updateTimer.Start();
        }
        
        /// <summary>
        /// Event happens every {Interval} secs.
        /// </summary>
        private static void UpdateConnectionStatus(object sender, ElapsedEventArgs e)
        {
            if (CurrentStatus != IsRaspberryAvailable)
            {
                CurrentStatus = IsRaspberryAvailable;

                var status = new ConnectionStatusEventArgs();
                status.IsRaspberryAvailable = CurrentStatus;
                ConnectionStateChanged?.Invoke(new object(), status);
            }
        }        

        public static int IntervalOfUpdate
        {
            set
            {
                if (value >= MinInterval && value <= MaxtInterval)
                {
                    Interval = value;
                }
                else
                {
                    throw new Exception($"Update interval is invalid (should be between {MinInterval} and {MaxtInterval}.)");
                }
            }

            get
            {
                return Interval;
            }
        }

        /// <summary>
        /// Check availability of net connection
        /// </summary>
        private static bool IsConnectionAvailable
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }

        /// <summary>
        /// Checks availability of connection to remote Raspberry PI computer
        /// </summary>
        public static bool IsRaspberryAvailable
        {
            get
            {
                string IP = RaspberryIPAdderss;

                if (IsConnectionAvailable)
                {
                    if (IP.Length > 0)
                    {
                        if (RaspberryPinger.Send(IP).Status == IPStatus.Success)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;                    
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Returns actual IP LAN address of RaspberryPI
        /// (Raspberry reports this address to Firebase DB in real time)
        /// </summary>
        public static string RaspberryIPAdderss
        {
            get
            {
                if (IsConnectionAvailable)
                {
                    var raspIpClient = new WebClient();
                    string JsonStr = raspIpClient.DownloadString(new Uri(FirebaseAddress));

                    string IP = GetIPFromFireBaseJSONString(JsonStr);

                    PingReply reply;

                    try
                    {
                        reply = RaspberryPinger.Send(IP);                        
                    }
                    catch
                    {
                        return string.Empty;
                    }

                    if (reply.Status == IPStatus.Success)
                        return IP;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// async version of RaspberryIPAdderss
        /// </summary>
        public static async Task<string> RaspberryIPAdderssAsync()
        {
            if (IsConnectionAvailable)
            {
                var raspIpClient = new WebClient();
                var JsonStr = await raspIpClient.DownloadStringTaskAsync(new Uri(FirebaseAddress));

                string IP = GetIPFromFireBaseJSONString(JsonStr);

                PingReply reply;

                try
                {
                    reply = RaspberryPinger.Send(IP);
                }
                catch
                {
                    return string.Empty;
                }

                if (reply.Status == IPStatus.Success)
                    return IP;
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Reads data from Firebase database in JSON format
        /// </summary>
        /// <param name="resourceAddress"></param>
        /// <returns></returns>
        private static string GetIPFromFireBaseJSONString(string resourceAddress)
        {
            using (var jreader = new JsonTextReader(new StringReader(resourceAddress)))
            {
                jreader.Read();
                jreader.Read();
                jreader.Read();
                return jreader.Value.ToString();
            }
        }
    }
}