using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Sobolev.Capstone.Extensions;
using Sobolev.Capstone.PreferencesStorage;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Sobolev.Capstone.Streams
{

    public static class BrainActivityValues
    {
        public static int AttentionThresholdValue { get; set; } = InternalStorageProvider.CurrentPreferneces.AttentionThresholdValue;
        public static int MeditationThresholdValue { get; set; } = InternalStorageProvider.CurrentPreferneces.MeditationThresholdValue;
        public static readonly int LowLevelCounterThresholdMS = InternalStorageProvider.CurrentPreferneces.LowLevelCounterThresholdMS; // if level of less than AttentionThresholdValue is lasting more than LowLevelCounterThresholdMS times motor should be turned off
        public static readonly int MaxMindWaveValueConst = InternalStorageProvider.CurrentPreferneces.MaxMindDataValue;
    }

    public sealed class BrainWaveEventArgs : EventArgs
    {
        public int MeditationLevel { internal set; get; } = 0;
        public int AttentionLevel { set; get; } = 0;
        public int PoorSignalLevel { set; get; } = 0;
        public string JSON = string.Empty;
        public bool BadDataReceived { get; internal set; } = false;
        
        public bool NoDevice
        {
            get
            {
                if (AttentionLevel == 0 | AttentionLevel == -1)
                    return true;
                else
                    return false;
            }
        }

        public BrainWaveEventArgs()
        {
            MeditationLevel = 0;
            AttentionLevel = 0;
            PoorSignalLevel = 0;
        }

        public void GetDataFromJSONString(string JSONData)
        {
            using (var jsonreader = new JsonTextReader(new StringReader(JSONData)))
            {
                try
                {
                    JSON = JSONData;

                    jsonreader.ReadNTimes(5);                    
                    AttentionLevel =  NormalizeData(jsonreader.Value.ToString());

                    jsonreader.ReadNTimes(2);
                    MeditationLevel = NormalizeData(jsonreader.Value.ToString());                    

                    jsonreader.ReadNTimes(2);
                    jsonreader.Skip();
                    jsonreader.ReadNTimes(2);
                    PoorSignalLevel = NormalizeData(jsonreader.Value.ToString());
                }
                catch
                {
                    //Debug.WriteLine($"From GetDataFromJSONString! - {JSONData}");
                    BadDataReceived = true;
                    MeditationLevel = -1;
                    AttentionLevel = -1;
                    PoorSignalLevel = -1;
                    return;
                }
            }
        }

        private int NormalizeData(string StrData)
        {
            int result = 0;
            int.TryParse(StrData, out result);            

            return (result >= 0 & result <= BrainActivityValues.MaxMindWaveValueConst) ? result : 0;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (BrainWaveEventArgs Left, (int Att, int Med) Right) =>
            Left.AttentionLevel == Right.Att & Left.MeditationLevel == Right.Med;

        public static bool operator != (BrainWaveEventArgs Left, (int Att, int Med) Right) => !(Left == Right);


        public static bool operator ! (BrainWaveEventArgs Operand)
        {
            if (Operand.AttentionLevel == -1)
                return false;
            else
                return (Operand.AttentionLevel < BrainActivityValues.AttentionThresholdValue);
        }

    }

    public sealed class MindStream
    {
        private readonly int PortNumber = InternalStorageProvider.CurrentPreferneces.ThinkGearPort;
        private readonly string IPAdress = InternalStorageProvider.CurrentPreferneces.ThinkGearIPAdress;

        private TcpClient Client = null;
        private Stream StreamFromMindWave = null;
        private int BytesRead = 0;

        private CancellationTokenSource mindDataCanclellationTokenSource;
        private CancellationToken MindDataReaderCancelToken;
        private Task mindDataReaderTask;

        public BrainWaveEventArgs BrainDataPacket { get; set; } = new BrainWaveEventArgs();

        #region event for getting data from brainwave data
        public event EventHandler<BrainWaveEventArgs> BrainWaveChanged;
        public void OnBrainDataChanged(BrainWaveEventArgs bwea) => BrainWaveChanged?.Invoke(this, bwea);
        #endregion

        // command to enable JSON output from ThinkGear Connector (TGC)
        private readonly byte[] EnableOutputCommand = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": false, ""format"": ""Json""}");

        public bool ConnectionStatus { get; set; } = false;

        public MindStream()
        {            
            SendEnablingCommand();
            //Debug.WriteLine($"Commands are enabled:{ConnectionStatus}");
        }

        

        private async void SendEnablingCommand()
        {
            try
            {
                Client = new TcpClient(IPAdress, PortNumber);
                StreamFromMindWave = Client.GetStream();

                if (StreamFromMindWave.CanWrite)
                    await StreamFromMindWave.WriteAsync(EnableOutputCommand, 0, EnableOutputCommand.Length);

                Client.Close();
                ConnectionStatus = true;
            }
            catch (SocketException)
            {
                ConnectionStatus = false;
            }
        }

        private byte[] Buffer;
        private string[] PacketOfJSONData;
        private (int Attention, int Mediatation) LastBrainState = (0, 0);

        public void StopReadingBrainWaveData()
        {
            mindDataCanclellationTokenSource.Cancel();
        }

        public void StartReadingBrainWaveData()
        {
            //Debug.WriteLine("started");            

            mindDataCanclellationTokenSource = new CancellationTokenSource();
            MindDataReaderCancelToken = mindDataCanclellationTokenSource.Token;

            try
            {
                //Debug.WriteLine($"before try {BrainDataPacket.AttentionLevel}");

                Client = new TcpClient(IPAdress, PortNumber);
                StreamFromMindWave = Client.GetStream();

                mindDataReaderTask = Task.Run(async () =>
                {
                    //Debug.WriteLine($"inside task {BrainDataPacket.AttentionLevel}");

                    TcpClient Client = new TcpClient(IPAdress, PortNumber);
                    Stream StreamFromMindWave = Client.GetStream();
                    Stopwatch lowAttentionWatcher = new Stopwatch();

                    // Sending configuration packet to TGC
                    if (StreamFromMindWave.CanWrite)
                    {
                        StreamFromMindWave.Write(EnableOutputCommand, 0, EnableOutputCommand.Length); // await???? - not sure 
                    }

                    //Debug.WriteLine($"before try {BrainDataPacket.AttentionLevel} and {StreamFromMindWave.CanRead}");

                    if (StreamFromMindWave.CanRead)
                    {
                        Buffer = new byte[2048];
                        while (ConnectionStatus)
                        {
                            BytesRead = await StreamFromMindWave.ReadAsync(Buffer, 0, 2048);
                            PacketOfJSONData = Encoding.UTF8.GetString(Buffer, 0, BytesRead).Split('\r');
                            foreach (string element in PacketOfJSONData)
                            {
                                if (element.Trim().Length < 45)
                                {
                                    BrainDataPacket.BadDataReceived = true;
                                    continue;
                                }
                                else
                                    BrainDataPacket.BadDataReceived = false;

                                //Debug.WriteLine($"Loop of packets: {s}");
                                BrainDataPacket.GetDataFromJSONString(element);

                                //Debug.WriteLine(BrainDataPacket.AttentionLevel);
                                //OnBrainDataChanged(BrainDataPacket);

                                if (BrainDataPacket.AttentionLevel != -1)
                                {
                                    if (BrainDataPacket != LastBrainState)
                                    {
                                        //Debug.WriteLine($"inside the loop {BrainDataPacket.AttentionLevel}");

                                        LastBrainState = (BrainDataPacket.AttentionLevel, BrainDataPacket.MeditationLevel);

                                        // event is fired only if attention level falls below predefined level of brain activity 
                                        // (during some significant period of time)
                                        // and in this case robot should be stopped (this is handled by robot's command classes)

                                        if (!BrainDataPacket)
                                        {
                                            if (!lowAttentionWatcher.IsRunning)
                                            {
                                                lowAttentionWatcher.Start();
                                                //Debug.WriteLine("-------- Stop watcher started");
                                            }
                                            else
                                            {
                                                if (lowAttentionWatcher.ElapsedMilliseconds >=
                                                    BrainActivityValues.LowLevelCounterThresholdMS)
                                                {
                                                    //Debug.WriteLine($"-------- Event sent when time is {lowAttentionWatcher.ElapsedMilliseconds}");
                                                    OnBrainDataChanged(BrainDataPacket);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lowAttentionWatcher.Reset();
                                            lowAttentionWatcher.Stop();
                                            //Debug.WriteLine("-------- Stop watcher --- stopped!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }, MindDataReaderCancelToken);

                Client.Close();
                ConnectionStatus = true;

            }
            catch (SocketException)
            {
                ConnectionStatus = false;                
            }
        }        
    }
}
