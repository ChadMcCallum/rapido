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
using System.Xml.Linq;
using System.Linq;

namespace Rapido
{
    public class PathParser
    {
        public static IEnumerable<Path> Parse(String pathKml)
        {
            var xDoc = XDocument.Parse(pathKml);

            foreach (var placemarkElem in xDoc.Descendants("Placemark"))
            {
                var coordinatesElem = placemarkElem.Descendants("coordinates").Single();
                var descriptionElem = placemarkElem.Descendants("description").Single();

                var coordinatePairs = coordinatesElem.Value.Split(' ');

                var coordinateList = new List<Path.Coordinate>();

                foreach (var coordiate in coordinatePairs)
                {
                    coordinateList.Add (new Path.Coordinate { Latitude = Double.Parse(coordiate.Split(',')[0]), Longitude = Double.Parse(coordiate.Split(',')[1]) } );
                }

                var path = new Path
                {
                     Description = descriptionElem.Value,
                     Coordinates = coordinateList
                };

                yield return path;
            }
        }
    }
}
