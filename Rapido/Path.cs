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

namespace Rapido
{
    public class Path
    {
        public String Key { get; set; }
        public String Description { get; set; }
        public IEnumerable<Coordinate> Coordinates { get; set; }

        public class Coordinate
        {
            public Double Latitude { get; set; }
            public Double Longitude { get; set; }
        }
    }
}
