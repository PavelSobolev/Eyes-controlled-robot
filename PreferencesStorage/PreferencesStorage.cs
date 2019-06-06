using System;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

  
namespace Sobolev.Capstone.PreferencesStorage
{
    [Serializable]
    public class DataPreferences
    {
        public int MaxPower = 1023;
        public int MinPower = 500;
        public int NormalPower = 1000;

        public int GPIOPwmPortNumber = 18;
        public int MaxPortNumber = 27;
        public int FirstRightPortNumber = 17;
        public int SecondRightPortNumber = 27;
        public int FirstLeftPortNumber = 3;
        public int SecondLeftPortNumber = 4;

        public string FirebaseAddress = "https://gazeinteractor.firebaseio.com/.json";
        public int DefaultInterval = 1000 * 30 * 60;
        public int MinInterval = 1000;
        public int MaxtInterval = 20000;

        public string RaspberryUser = "pi";
        public string RaspberryHost = "192.198.0.120";

        public int PictureWidth = 1024;
        public int PictureHeight = 768;
        public int PictureTriggerSize = 100;

        public int AttentionThresholdValue = 30;
        public int MeditationThresholdValue = 80;
        public int LowLevelCounterThresholdMS = 10_000;
        public int MaxMindDataValue = 100;

        public int ThinkGearPort = 13854;
        public string ThinkGearIPAdress = "127.0.0.1";


        public int PictureUpdatingIntervalMS = 20;

        public string TextTriggerStop = "STOP\nGAZE";
        public string TextTriggerStart = "START\nGAZE";

        public string PictureFontName = "Tahoma";
        public int PictureFontSize = 25;

        public int TextShifht = 5;
        public int PointShift = 20;

        public int TransparancyLevel = 50;

        public string StartColor = "Orange";
        public string StopColor = "Red";
        public string RightLeftColor = "Blue";
        public string BaсkColor = "Yellow";
        public string ForwardColor = "Green";
        public string StartStopTriggerColor = "White";

        public string BackPhraze = "back";
        public string ForwardPhraze = "forward";
        public string LeftPhraze = "left";
        public string RightPhraze = "right";
        public string StopPhraze = "stop";
        public string PassivePhraze = "passive";
        public string StartPhraze = "";
    }     
   
    public static class InternalStorageProvider
    {
        private static readonly string FileName;
        private static readonly IsolatedStorageFile PreferenceFile;        
        private static IsolatedStorageFileStream FileStream;

        public static DataPreferences CurrentPreferneces { get; set; }

        static InternalStorageProvider()
        {
            FileName = "Sobolev.Capstone.Prefernces.xml";
            CurrentPreferneces = new DataPreferences();


            PreferenceFile = IsolatedStorageFile.GetStore(
                IsolatedStorageScope.Assembly | IsolatedStorageScope.User, 
                null, null);            

            if (!PreferenceFile.FileExists(FileName))                            
                WriteDefault(); // save default data for the next launching of the app            
            else                            
                Read();
        }

        public static void WriteDefault()
        {
            FileStream = PreferenceFile.OpenFile(FileName, System.IO.FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(DataPreferences));
            serializer.Serialize(FileStream, CurrentPreferneces);
            FileStream.Close();
        }

        public static void Read()
        {
            FileStream = PreferenceFile.OpenFile(FileName, System.IO.FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(DataPreferences));
            CurrentPreferneces = (DataPreferences)serializer.Deserialize(FileStream);
            FileStream.Close();
        }

        public static void Update(DataPreferences newPref)
        {            
            CurrentPreferneces = newPref;
            WriteDefault();            
        }
    }
}
