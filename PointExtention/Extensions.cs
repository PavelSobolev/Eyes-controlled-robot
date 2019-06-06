using System;
using System.Drawing;
using Newtonsoft.Json;
using Sobolev.Capstone.Commands;

namespace Sobolev.Capstone.Extensions
{
    public static class PointExtention
    {
        /// <summary>
        /// Calculates distance between this point and point of the argument of the method
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(this Point p1, Point p2)
        {
            double xd = p1.X - p2.X;
            double yd = p1.Y - p1.Y;
            
            return Math.Sqrt(xd * xd - yd * yd);
        }

        /// <summary>
        /// Is this point is close enough to another point
        /// Two points are close if the distance between them is less then distance (second parameter)
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="distance">Distance of close proximity</param>
        /// <returns></returns>
        public static bool IsNear(this Point p1, Point p2, double distance)
        {
            if (distance <= 0)
            {
                throw new ArgumentException($"Invalid argument value '{distance}'. Distance must be non-negative.");                
            }

            if (p1.Distance(p2) <= distance)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// Class extends JsonTextReader
    /// </summary>
    public static class NewtonJsonReaderExtention
    {
        
        /// <summary>
        /// Method allows to execute Read method of JsonTextReader class N times
        /// </summary>
        /// <param name="Origin">Reader's objects</param>
        /// <param name="Times">Amount of repetitions of the Read command</param>
        public static void ReadNTimes(this JsonTextReader Origin, int Times)
        {
            for(int i=1; i<=Times; i++)
            {
                Origin.Read();
            }
        }
    }

    /// <summary>
    /// Class adds Execute method to string literals and variables
    /// </summary>
    public static class StringToRobotCommandExtention
    {
        /// <summary>
        /// Method executes a GPIO command which is contained in the origin object
        /// </summary>
        /// <param name="Origin">Object should contain GPIO command</param>
        /// <returns></returns>
        public static string Execute(this string Origin)
        {
            SshGPIOCommand.Run(Origin);
            return Origin;
        }
    }
}
