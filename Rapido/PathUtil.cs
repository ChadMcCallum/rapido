using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Rapido
{
    public static class PathUtil
    {

        public static string FormatTimeSpan(TimeSpan timespan)
        {
            return string.Format("{0}:{1:00}.{2:00}", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
        }

        public static IEnumerable<Path> GetNearestPaths(Double latitude, Double longitude)
        {
            IEnumerable<Path> paths;

            using (var dataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rapido.PathData.xml"))
            {
                paths = PathParser.Parse(new StreamReader(dataStream).ReadToEnd());
            }

            return
                paths
                .Where(x => x.Length > 500.0)
                .Select(x => new
                {                    
                    Path = x,
                    DistanceToStart = GetDistance(x.Coordinates.First(), new Path.Coordinate { Latitude = latitude, Longitude = longitude })
                })
                .OrderBy(x => x.DistanceToStart)
                .Select(x => x.Path);
        }

        public static Double GetDistance(Path.Coordinate c1, Path.Coordinate c2)
        {
            return
                        Math.Acos(Math.Sin(c1.Latitude) * Math.Sin(c2.Latitude) +
                        Math.Cos(c1.Latitude) * Math.Cos(c2.Latitude) *
                        Math.Cos(c2.Longitude - c1.Longitude)) * 6731;

            
        }

        public static IEnumerable<Path.Coordinate> GetSplits(Path path, Int32 splitCount)
        {
            var coordinates = path.Coordinates.ToList();
            var segments = new List<Segment>();

            for (Int32 i = 0; i < coordinates.Count - 2; i++)
            {
                var s = new Segment();
                s.Start = coordinates[i];
                s.End = coordinates[i + 1];
                s.Distance = Math.Sqrt((s.Start.Latitude - s.End.Latitude) * (s.Start.Latitude - s.End.Latitude) + (s.Start.Longitude - s.End.Longitude) * (s.Start.Longitude - s.End.Longitude));

                segments.Add(s);
            }

            var totalDistance = segments.Sum(x => x.Distance);
            var splitDistance = totalDistance / splitCount;
            var splits = new List<Path.Coordinate>();
            var nextSplitDistance = 0.0;
            var distanceTraversed = 0.0;
            var segmentIndex = 0;

            while (splits.Count < splitCount)
            {
                nextSplitDistance += splitDistance;

                while (distanceTraversed < nextSplitDistance)
                {
                    distanceTraversed += segments[segmentIndex].Distance;
                    segmentIndex++;
                    if (segmentIndex == segments.Count)
                    {
                        segmentIndex--;
                        break;
                    }
                }

                splits.Add(segments[segmentIndex].End);                
            }                

            return splits;
        }

        private class Segment
        {
            public Path.Coordinate Start { get; set; }
            public Path.Coordinate End { get; set; }
            public Double Distance { get; set; }
        }
    }
}
