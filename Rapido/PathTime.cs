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
    public enum TransportationMode
    {
        Bike,
        Walk,
        Blade,
        Skate, 
        Helicoptor
    }

    public class PathTime
    {
        public String PathKey { get; set; }
        public String User { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan[] SplitTimes { get; set; }
        public TransportationMode Mode { get; set; }
    }
}
