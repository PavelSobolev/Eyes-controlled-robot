using System.Collections.Generic;
using Sobolev.Capstone.PreferencesStorage;

namespace Sobolev.Capstone.Enumerations
{
    public static class EnumStrings
    {
        public static Dictionary<MoveGoalRectangles, string> MoveDirectionNames = 
            new Dictionary<MoveGoalRectangles, string>();

        static EnumStrings()
        {
            MoveDirectionNames[MoveGoalRectangles.BackRect] = InternalStorageProvider.CurrentPreferneces.BackPhraze;
            MoveDirectionNames[MoveGoalRectangles.ForwardRect] = InternalStorageProvider.CurrentPreferneces.ForwardPhraze;
            MoveDirectionNames[MoveGoalRectangles.StartRect] = InternalStorageProvider.CurrentPreferneces.StartPhraze;
            MoveDirectionNames[MoveGoalRectangles.LeftRect] = InternalStorageProvider.CurrentPreferneces.LeftPhraze;
            MoveDirectionNames[MoveGoalRectangles.RightRect] = InternalStorageProvider.CurrentPreferneces.RightPhraze;
            MoveDirectionNames[MoveGoalRectangles.StopRect] = InternalStorageProvider.CurrentPreferneces.StopPhraze;
            MoveDirectionNames[MoveGoalRectangles.Passive] = InternalStorageProvider.CurrentPreferneces.PassivePhraze;
        }
    }

    public enum MoveGoalRectangles
    {
        LeftRect, ForwardRect, RightRect, BackRect, StartRect, StopRect, Passive
    }
}
