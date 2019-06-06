using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Sobolev.Capstone.Enumerations;
using Sobolev.Capstone.PreferencesStorage;

namespace Sobolev.Capstone.GazeInteractionsData
{
    public static class GazeRectangle
    {
        public static readonly int Width = InternalStorageProvider.CurrentPreferneces.PictureWidth;
        public static readonly int Height = InternalStorageProvider.CurrentPreferneces.PictureHeight;
        public static readonly int TriggerSize = InternalStorageProvider.CurrentPreferneces.PictureTriggerSize;

        public static readonly int LeftBorder = 0;
        public static readonly int RightBorder = 0;
        public static readonly int UpperBorder = 0;
        public static readonly int MiddleBorder = 0;
        
        public static readonly Rectangle ForwardArea = new Rectangle();
        public static readonly Rectangle StopArea = new Rectangle();
        public static readonly Rectangle LeftArea = new Rectangle();
        public static readonly Rectangle RightArea = new Rectangle();
        public static readonly Rectangle BackArea = new Rectangle();
        public static readonly Rectangle StartArea = new Rectangle();

        private static readonly int ElementWidth = 0;

        static GazeRectangle()
        {
            LeftBorder = (int)(Width / 3.0);
            RightBorder = (int)((2.0 * Width) / 3.0);
            UpperBorder = (int)(Height / 2.0);
            MiddleBorder = UpperBorder + (int)(Height / 6.0);

            ElementWidth = (int)(Width / 6.0);
           
            BackArea = new Rectangle(0, 0, 2 * ElementWidth + 1, Height / 6);
            StopArea = new Rectangle(LeftBorder, 0, 2 * ElementWidth, Height / 6);

            LeftArea = new Rectangle(0, UpperBorder, Width/3, Height / 2);
            ForwardArea = new Rectangle(LeftBorder, UpperBorder, 2 * ElementWidth, Height / 2);
            RightArea = new Rectangle(RightBorder, UpperBorder, Width / 3, Height / 2);
            
            StartArea = new Rectangle(Width - TriggerSize, TriggerSize, TriggerSize, TriggerSize);
        }


        public static MoveGoalRectangles TestGazePoint(Point gazePoint)
        {
            if (StartArea.Contains(gazePoint))
            {
                return MoveGoalRectangles.StartRect;
            }
            else
            {
                if (StopArea.Contains(gazePoint))
                {
                    return MoveGoalRectangles.StopRect;
                }
                else
                {
                    if (LeftArea.Contains(gazePoint))
                    {
                        return MoveGoalRectangles.LeftRect;
                    }
                    else
                    {
                        if (RightArea.Contains(gazePoint))
                        {
                            return MoveGoalRectangles.RightRect;
                        }
                        else
                        {
                            if (BackArea.Contains(gazePoint))
                            {
                                return MoveGoalRectangles.BackRect;
                            }
                            else
                            {
                                if (ForwardArea.Contains(gazePoint))
                                {
                                    return MoveGoalRectangles.ForwardRect;
                                }
                                else
                                {
                                    return MoveGoalRectangles.Passive;
                                }
                            }
                        }
                    }
                }
            }           
        }
    }
}
