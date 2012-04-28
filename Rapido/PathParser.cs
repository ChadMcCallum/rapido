using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rapido
{
    public class PathParser
    {
        public static IEnumerable<Path> Parse(String pathKml)
        {
            var xDoc = XDocument.Parse(pathKml);

            foreach (var placemarkElem in xDoc.Descendants("{http://www.opengis.net/kml/2.2}Placemark"))
            {
                var coordinatesElem = placemarkElem.Descendants("{http://www.opengis.net/kml/2.2}coordinates").First();
                var descriptionElem = placemarkElem.Descendants("{http://www.opengis.net/kml/2.2}description").First();

                var description = descriptionElem.Value;

                var match = Regex.Match(description, @"[\d\.]+");
                var length = Double.Parse(match.Value);
                if (length < 100.0 )
                {
                    continue;
                }

                var coordinatePairs = coordinatesElem.Value.Split(' ');

                var coordinateList = new List<Path.Coordinate>();

                foreach (var coordiate in coordinatePairs)
                {
                    if (String.IsNullOrWhiteSpace(coordiate))
                        continue;
                    coordinateList.Add (new Path.Coordinate { Latitude = Double.Parse(coordiate.Split(',')[1]), Longitude = Double.Parse(coordiate.Split(',')[0]) } );
                }

                var path = new Path
                {
                    Length = length,
                    Key = String.Format("{0},{1}", coordinateList.First().Latitude.ToString(), coordinateList.First().Longitude.ToString()),
                    Description = descriptionElem.Value,
                    Coordinates = coordinateList
                };

                yield return path;
            }
        }
    }
}
