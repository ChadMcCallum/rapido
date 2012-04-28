using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rapido
{
    public static class DataAccess
    {
        public static PathTime GetBestTimeForPath(String pathKey)
        {
            return new PathTime
            {
                User = "Phil",
                PathKey = pathKey,
                SplitTimes = new TimeSpan[] { TimeSpan.FromSeconds(20.0), TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(25.0) },
                TotalTime = TimeSpan.FromSeconds(75.0)
            };
        }

        public static PathTime GetBestTimeForPathByUser(String pathKey, String userName)
        {
            return new PathTime
            {
                User = "Chad",
                PathKey = pathKey,
                SplitTimes = new TimeSpan[] { TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(40.0), TimeSpan.FromSeconds(35.0) },
                TotalTime = TimeSpan.FromSeconds(105.0)
            };
        }

        public static void PostPathTime(PathTime pathTime)
        {

        }
    }
}
