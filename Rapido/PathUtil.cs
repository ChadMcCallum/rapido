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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Rapido
{
    public static class PathUtil
    {
        public static IEnumerable<Path> GetNearestPaths(Double latitude, Double longitude)
        {
            IEnumerable<Path> paths;

            using (var dataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rapido.PathData.xml"))
            {
                paths = PathParser.Parse(new StreamReader(dataStream).ReadToEnd());
            }

            return
                paths
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
    }
}
