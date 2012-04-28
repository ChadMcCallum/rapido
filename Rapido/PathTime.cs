using System;

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
        public Int32 Rank { get; set; }
    }
}
