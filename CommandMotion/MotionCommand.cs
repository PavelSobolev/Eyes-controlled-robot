using System;
using System.Diagnostics;
using Sobolev.Capstone.Extensions;
using Sobolev.Capstone.PreferencesStorage;

namespace Sobolev.Capstone.Commands
{
    /// <summary>
    /// MotionCommand represents one command sent to robot 
    /// command is stated in terms of motion (not electrical signals)
    /// class uses underlying GPIOSshCommand to encode commands to electrical GPIO representation
    /// </summary>

    public interface IGPIOSshMotion
    {
        void TurnLeft();
        void TurnRight();
        void GoForward();
        void GoBack();
        void GoForwardTime(int Seconds);
        void GoBackTime(int Seconds);
        void Stop();
        void Initialize();        
    }


    public sealed class MotionCommand : IGPIOSshMotion
    {
        public readonly int MaxPower = InternalStorageProvider.CurrentPreferneces.MaxPower; // 1023;
        public readonly int MinPower = InternalStorageProvider.CurrentPreferneces.MinPower; //500;

        private int power = InternalStorageProvider.CurrentPreferneces.NormalPower;
        public int Power
        {
            get => power;
            set
            {
                if (value <= MaxPower & value >= MinPower)
                    power = value;
                else
                    throw new ArgumentException($"Wrong value for wheel power. Must be between {MinPower} and {MaxPower}.");
            }
        }

        public string LastCommand { get; private set; }

        public MotionCommand()
        {
            Initialize();
        }

        private string InitializeCommand
        {
            get => GPIOCommand.ModeOut(GPIOWheelPort.LeftWheel.firstPort) +
                GPIOCommand.ModeOut(GPIOWheelPort.LeftWheel.secondPort) +
                GPIOCommand.ModeOut(GPIOWheelPort.RightWheel.firstPort) +
                GPIOCommand.ModeOut(GPIOWheelPort.RightWheel.secondPort) +
                GPIOCommand.ModePwm() +
                GPIOCommand.PwmOff() + 
                GPIOCommand.Write(GPIOWheelPort.LeftWheel.firstPort, true) +
                GPIOCommand.Write(GPIOWheelPort.LeftWheel.secondPort, false) +
                GPIOCommand.Write(GPIOWheelPort.RightWheel.firstPort, false) +
                GPIOCommand.Write(GPIOWheelPort.RightWheel.secondPort, true);
        }

        public void Initialize()
        {
            LastCommand = InitializeCommand.Execute();
            Debug.WriteLine(LastCommand);
        }

        public void Stop()
        {
            LastCommand = InitializeCommand.Execute();
        }

        private string LeftWheelForwardCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.LeftWheel.firstPort, true) +
                GPIOCommand.Write(GPIOWheelPort.LeftWheel.secondPort, false);
        }

        private string LeftWheelBackCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.LeftWheel.firstPort, false) +
                GPIOCommand.Write(GPIOWheelPort.LeftWheel.secondPort, true);
        }

        private string LeftWheelStopCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.LeftWheel.firstPort, false) +
                GPIOCommand.Write(GPIOWheelPort.LeftWheel.secondPort, false);
        }

        private string RightWheelForwardCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.RightWheel.firstPort, false) +
                GPIOCommand.Write(GPIOWheelPort.RightWheel.secondPort, true);
        }

        private string RightWheelBackCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.RightWheel.firstPort, true) +
                GPIOCommand.Write(GPIOWheelPort.RightWheel.secondPort, false);
        }

        private string RightWheelStopCommand
        {
            get => GPIOCommand.Write(GPIOWheelPort.RightWheel.firstPort, false) +
                GPIOCommand.Write(GPIOWheelPort.RightWheel.secondPort, false);
        }

        public void GoBack()
        {
            LastCommand = 
                (InitializeCommand + 
                LeftWheelBackCommand +
                RightWheelBackCommand +
                GPIOCommand.PwmPower(Power)).Execute();
        }

        public void GoBackTime(int seconds)
        {
            LastCommand = (GPIOCommand.PwmOff() +
                LeftWheelBackCommand +
                RightWheelBackCommand +
                GPIOCommand.PwmPower(Power) +
                GPIOCommand.Sleep(seconds) +
                GPIOCommand.PwmOff()).Execute(); ;
        }

        public void GoForward()
        {
            LastCommand = 
                (InitializeCommand + 
                GPIOCommand.PwmOff() +
                LeftWheelForwardCommand +
                RightWheelForwardCommand +
                GPIOCommand.PwmPower(MaxPower) +                 
                GPIOCommand.PwmPower(Power)).Execute();            
        }

        public void GoForwardTime(int seconds)
        {
            LastCommand =
                (InitializeCommand +
                GPIOCommand.PwmOff() +
                LeftWheelForwardCommand +
                RightWheelForwardCommand +
                GPIOCommand.PwmPower(Power) +
                GPIOCommand.Sleep(seconds) +
                GPIOCommand.PwmOff()).Execute();
        }
       

        public void TurnLeft()
        {
            LastCommand = 
                (InitializeCommand + 
                GPIOCommand.PwmPower(MaxPower) +
                LeftWheelStopCommand +
                RightWheelForwardCommand).Execute();
        }

        public void TurnRight()
        {
            LastCommand = 
                (InitializeCommand + 
                GPIOCommand.PwmPower(MaxPower) +
                LeftWheelForwardCommand +
                RightWheelStopCommand).Execute();                        
        }
    }
}
