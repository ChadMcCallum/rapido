using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Reflection;
using System.IO;

namespace Rapido
{
    public partial class PreviewCourse : PhoneApplicationPage
    {
        public Path CurrentCourse;
        public List<Path> courses;
        private GeoCoordinateWatcher watcher;
        private GeoCoordinate currentPosition;
        private TransportationMode currentMode;

        public PreviewCourse()
        {
            InitializeComponent();
            GetCurrentPosition();
            DataContext = this;
            DataAccess.Load();
            currentMode = TransportationMode.Bike;
            InvertButton(Bike);
        }

        private void ResetAllButtons()
        {
            var buttons = new[] { Bike, Run, Blade, Skate };
            foreach (var button in buttons)
            {
                button.Foreground = new SolidColorBrush(Colors.White);
                button.Background = new SolidColorBrush(Colors.Black);
            }
        }

        private void InvertButton(Button button)
        {
            ResetAllButtons();
            button.Foreground = new SolidColorBrush(Colors.Black);
            button.Background = new SolidColorBrush(Colors.White);
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

        private void SetCurrentCourse(Path path)
        {
            CurrentCourse = path;
            GetTimesForCourse();
        }

        private void GetTimesForCourse()
        {
            var times = DataAccess.GetBestTimeForPath(CurrentCourse.Key, currentMode);
            var personal = DataAccess.GetBestTimeForPathByUser(CurrentCourse.Key, "Chad", currentMode);
            if (times != null)
            {
                BestTime.Text = PathUtil.FormatTimeSpan(times.TotalTime);
            }
            else
            {
                BestTime.Text = "N/A";
            }
            if (personal != null)
            {
                PersonalTime.Text = PathUtil.FormatTimeSpan(personal.TotalTime);
            }
            else
            {
                PersonalTime.Text = "N/A";
            }
        }

        private void GetCourses(GeoCoordinate position)
        {
            courses = PathUtil.GetNearestPaths(position.Latitude, position.Longitude).ToList();
            SetCurrentCourse(courses.First());
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
            foreach (var coordinate in CurrentCourse.Coordinates)
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
            ((App)Application.Current).CurrentCourse = CurrentCourse;
            NavigationService.Navigate(new Uri("/Race.xaml", UriKind.Relative));
        }

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentIndex = courses.IndexOf(CurrentCourse);

            currentIndex += 2;

            if (currentIndex >= courses.Count)
                currentIndex = 0;
            SetCurrentCourse(courses[currentIndex]);
            DrawCourse();
        }

        private void Previous_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentIndex = courses.IndexOf(CurrentCourse);

            currentIndex -= 2;
            if (currentIndex < 0)
                currentIndex = courses.Count - 1;
            SetCurrentCourse(courses[currentIndex]);
            DrawCourse();
        }

        private void Bike_Click(object sender, RoutedEventArgs e)
        {
            InvertButton(Bike);
            currentMode = TransportationMode.Bike;
            GetTimesForCourse();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            InvertButton(Run);
            currentMode = TransportationMode.Walk;
            GetTimesForCourse();

        }

        private void Blade_Click(object sender, RoutedEventArgs e)
        {
            InvertButton(Blade);
            currentMode = TransportationMode.Blade;
            GetTimesForCourse();

        }

        private void Skate_Click(object sender, RoutedEventArgs e)
        {
            InvertButton(Skate);
            currentMode = TransportationMode.Skate;
            GetTimesForCourse();

        }
    }
}