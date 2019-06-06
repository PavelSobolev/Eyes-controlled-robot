#region fold
/*using System;
using static System.Console;
using Streams;

namespace ConsoleTester
{
    class ConsoleTesterProgram
    {        
        public static void Main()
        {
            BrainWaveEventArgs bbbb = new BrainWaveEventArgs();
            string JSONBrainData_Correct = "{\"eSense\":{\"attention\":47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":89}";

            bbbb.GetDataFromJSONString(JSONBrainData_Correct);
            WriteLine(bbbb.JSON);
            WriteLine();
            WriteLine(bbbb.AttentionLevel);
            WriteLine(bbbb.MeditationLevel);
            WriteLine(bbbb.PoorSignalLevel);

            ReadKey();
        }

    }
}
*/

// ???????????? ??????????? ???????
using System;

using static System.Console;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using Sobolev.Capstone.Commands;

/*
namespace _3._2__1__
{
    class Program
{
    readonly static string address = "192.168.0.103";

        [STAThreadAttribute]
        public static void Main(string[] args)
        {

            TcpClient client;
            Stream stream;
            byte[] buffer = new byte[2048];
            int bytesRead;
            // Building command to enable JSON output from ThinkGear Connector (TGC)
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");


            //try
            //{
            //    client = new TcpClient("127.0.0.1", 13854);
            //    stream = client.GetStream();

            //    System.Threading.Thread.Sleep(5000);
            //    client.Close();
            //}
            //catch (SocketException se) { }


            try
            {
                client = new TcpClient("127.0.0.1", 13854);
                stream = client.GetStream();

                // Sending configuration packet to TGC
                if (stream.CanWrite)
                {
                    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
                }

                System.Threading.Thread.Sleep(5000);
                client.Close();
            }
            catch (SocketException se) { }

            try
            {
                client = new TcpClient("127.0.0.1", 13854);
                stream = client.GetStream();

                // Sending configuration packet to TGC
                if (stream.CanWrite)
                {
                    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
                }

                new Thread(()=>
                {
                    using (StreamWriter streamWriter = new StreamWriter(@"c:\datafiles\braindata.txt", true))

                        if (stream.CanRead)
                        {
                            WriteLine("reading bytes");

                            // This should really be in it's own thread
                            int i = 0;
                            DateTime Finish = DateTime.Now.AddMinutes(15);
                            while (i <= 10)
                            //while (DateTime.Now <= Finish)
                            {
                                bytesRead = stream.Read(buffer, 0, 2048);
                                //WriteLine(new String(buffer));
                                string[] packets = Encoding.UTF8.GetString(buffer, 0, bytesRead).Split('\r');
                                foreach (string s in packets)
                                {
                                    //ParseJSON(s.Trim());
                                    if (s.Length > 30)
                                        WriteLine($"{s}\n=====================");
                                    //streamWriter.WriteLine(s);
                                }
                            }

                        }
                }).Start();




                System.Threading.Thread.Sleep(5000);

                client.Close();

            }
            catch (SocketException se) { }


        WriteLine("the end");
        ReadLine();


    }
}
}
*/
#endregion


using Sobolev.Capstone.PreferencesStorage;
class Tester
{
    public static void Main()
    {

        int x = InternalStorageProvider.CurrentPreferneces.MaxPower;

        WriteLine($"{x}");

        var rr = new DataPreferences();
        rr.MaxPower = -6532;

        InternalStorageProvider.Update(rr);

        x = InternalStorageProvider.CurrentPreferneces.MaxPower;
        WriteLine($"{x}");

        ReadKey();
    }
}
