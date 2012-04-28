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
                    DistanceToStart =
                        Math.Acos(Math.Sin(x.Coordinates.First().Latitude) * Math.Sin(latitude) +
                        Math.Cos(x.Coordinates.First().Latitude) * Math.Cos(latitude) *
                        Math.Cos(longitude - x.Coordinates.First().Longitude)) * 6731
                })
                .OrderBy(x => x.DistanceToStart)
                .Select(x => x.Path);
        }
    }
}
