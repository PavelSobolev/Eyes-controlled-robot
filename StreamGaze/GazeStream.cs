using System.Diagnostics;
using System.Drawing;
using Tobii.Interaction.Framework;
using Tobii.Interaction;
using Sobolev.Capstone.Extensions;

namespace Sobolev.Capstone.Streams
{    
    public sealed class GazeStream
    {
        private FixationDataStream FixationInfo = null;
        private Host gazeHost = null;
        private EngineStateObserver<UserPresence> userPresenceObserver;
        public bool StreamAvailable { get; set; } = true;

        private Point gazePoint = new Point();
        private Point fixationPoint = new Point();

        public bool IsUserPresent { get; private set; } = false;

        public bool IsFixation { get; private set; } = false;

        public double GazeMobilitySensetivity { get; set; } = 1.0;

        public Point FixationPoint
        {
            get => fixationPoint;
        }

        public Point GazePoint
        {
            get => gazePoint;
        }

        public GazeStream(System.Drawing.Rectangle rectangle)
        {
            gazeHost = new Host();
            FixationInfo = gazeHost.Streams.CreateFixationDataStream(FixationDataMode.Slow);
            userPresenceObserver = gazeHost.States.CreateUserPresenceObserver();            

            FixationInfo.Next += FixationHappened;
            userPresenceObserver.Changed += UserPresenceChanged;
        }

        private void UserPresenceChanged(object sender, EngineStateValue<UserPresence> e)
        {
            //Debug.WriteLine("UserPresenceChanged!!!!!!!");

            if (e.IsValid)
            {
                switch(e.Value)
                {
                    case UserPresence.Present:
                        IsUserPresent = true;
                        break;
                    case UserPresence.Unknown:
                    case UserPresence.NotPresent:
                        IsUserPresent = false;
                        break;
                }
            }
            else
            {
                IsUserPresent = false;
            }
        }

        private void FixationHappened(object sender, StreamData<FixationData> e)
        {
            if (!StreamAvailable) return;
            if (!IsUserPresent) return;

            switch (e.Data.EventType)
            {
                case FixationDataEventType.Begin:                    
                case FixationDataEventType.End:
                    return;

                case FixationDataEventType.Data:

                    Point newPoint = new Point((int)e.Data.X, (int)e.Data.Y);

                    if (!gazePoint.IsNear(newPoint, GazeMobilitySensetivity))
                        gazePoint = new Point((int)e.Data.X, (int)e.Data.Y);

                    break;
            }
        }
    }
}
