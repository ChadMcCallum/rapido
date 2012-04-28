using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Rapido
{
    public partial class PreviewCourse : PhoneApplicationPage
    {
        public Path CurrentCourse;
        public List<Path> courses;
        private GeoCoordinateWatcher watcher;
        private GeoCoordinate currentPosition;

        public PreviewCourse()
        {
            InitializeComponent();
            GetCurrentPosition();
            DataContext = this;            
        }

        private void GetCurrentPosition()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.Start();
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            currentPosition = watcher.Position.Location;
            watcher.Stop();
            GetCourses(currentPosition);
        }

        private void GetCourses(GeoCoordinate position)
        {
            courses = PathUtil.GetNearestPaths(position.Latitude, position.Longitude).ToList();
            CurrentCourse = courses.First();

            DrawCourse();
        }

        private void DrawCourse()
        {
            MapPreview.Children.Clear();

            var polyline = new MapPolyline
                               {
                                   Stroke = new SolidColorBrush(Colors.Red),
                                   StrokeThickness = 5,
                                   Opacity = 0.7,
                                   Locations = new LocationCollection()
                               };
            foreach(var coordinate in CurrentCourse.Coordinates)
            {
                polyline.Locations.Add(new GeoCoordinate(coordinate.Latitude, coordinate.Longitude));
            }
            MapPreview.Children.Add(polyline);

            //var courseRect = new LocationRect();
            //courseRect.North = CurrentCourse.Coordinates.Max(c => c.Latitude);
            //courseRect.South = CurrentCourse.Coordinates.Min(c => c.Latitude);
            //courseRect.East = CurrentCourse.Coordinates.Min(c => c.Longitude);
            //courseRect.West = CurrentCourse.Coordinates.Max(c => c.Longitude);

            var center = new GeoCoordinate();
            center.Latitude = CurrentCourse.Coordinates.Average(c => c.Latitude);
            center.Longitude = CurrentCourse.Coordinates.Average(c => c.Longitude);

            MapPreview.SetView(center, 15);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Race.xaml", UriKind.Relative));
        }

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentIndex = courses.IndexOf(CurrentCourse);
            if (currentIndex >= courses.Count)
                currentIndex = 0;
            CurrentCourse = courses[currentIndex];
            DrawCourse();
        }

        private void Previous_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentIndex = courses.IndexOf(CurrentCourse);
            if (currentIndex < 0)
                currentIndex = courses.Count - 1;
            CurrentCourse = courses[currentIndex];
            DrawCourse();
        }
    }
}