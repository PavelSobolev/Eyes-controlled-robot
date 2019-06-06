using static System.Math;
using Sobolev.Capstone.PreferencesStorage;

namespace Sobolev.Capstone.Commands
{
    /// <summary>
    /// class defines one command which is sent from client to robot
    /// using SSH protocol (with GPIO arguments and parameters)
    /// decodes commands from CommandMotionclass
    /// </summary>
    public static class GPIOCommand
    {
        private static readonly int PwmPort = InternalStorageProvider.CurrentPreferneces.GPIOPwmPortNumber;

        public static string ModeOut(int port) => $"gpio -g mode {port} out; ";
        public static string ModePwm() => $"gpio -g mode {PwmPort} pwm; ";
        public static string Write(int port, bool value) => $"gpio -g write {port} {(value ? 1 : 0)}; ";
        public static string PwmOff() => $"gpio -g pwm {PwmPort} 0; ";
        public static string PwmPower(int power) => $"gpio -g pwm {PwmPort} {power}; ";
        public static string Sleep(int sec) => $"sleep {sec}; ";
    }

    public static class GPIOWheelPort
    {
        public static int StartStopPort { get; private set; } = InternalStorageProvider.CurrentPreferneces.GPIOPwmPortNumber;
        private static readonly int MaxPort = InternalStorageProvider.CurrentPreferneces.MaxPortNumber;
        private static readonly int FR = InternalStorageProvider.CurrentPreferneces.FirstRightPortNumber;
        private static readonly int SR = InternalStorageProvider.CurrentPreferneces.SecondRightPortNumber;
        private static readonly int FL = InternalStorageProvider.CurrentPreferneces.FirstLeftPortNumber;
        private static readonly int SL = InternalStorageProvider.CurrentPreferneces.SecondLeftPortNumber;

        private static (int firstPort, int secondPort) innerRightPort;
        public static (int firstPort, int secondPort) RightWheel
        {
            get => innerRightPort;

            private set
            {
                if (value.firstPort != value.secondPort)
                {
                    if (Max(value.firstPort, value.secondPort) <= MaxPort)
                    {
                        innerRightPort = value;
                    }
                }
            }
        }

        private static (int firstPort, int secondPort) innerLeftPort;
        public static (int firstPort, int secondPort) LeftWheel
        {
            get => innerLeftPort;
            private set
            {
                if (value.firstPort != value.secondPort)
                {
                    if (Max(value.firstPort, value.secondPort) <= MaxPort)
                    {
                        innerRightPort = value;
                    }
                }
            }
        }


        static GPIOWheelPort()
        {
            innerRightPort = (FR, SR);
            innerLeftPort = (FL, SL);
        }
    }
}