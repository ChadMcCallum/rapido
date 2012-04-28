﻿using System;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace Rapido
{
    public partial class Race : PhoneApplicationPage
    {
        private GeoCoordinateWatcher watcher;
        private bool racing = false;
        private Path currentCourse;
        private DateTime? start;
        private DateTime? end;
        private DateTime? last;
        private DispatcherTimer timer;
        private GeoCoordinate lastCoordinate;

        public Race()
        {
            InitializeComponent();
            StartWatcher();
            currentCourse = ((App)Application.Current).CurrentCourse;
            InitMap();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.1);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (racing)
            {
                var timespan = DateTime.Now.Subtract(start.Value);
                Elapsed.Text = string.Format("{0}:{1:00}.{2:##}", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
            }
        }

        private void InitMap()
        {
            var polyline = new MapPolyline
            {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 5,
                Opacity = 0.7,
                Locations = new LocationCollection()
            };

            foreach (var coordinate in currentCourse.Coordinates)
            {
                polyline.Locations.Add(new GeoCoordinate(coordinate.Latitude, coordinate.Longitude));
            }
            RaceMap.Children.Add(polyline);
        }

        private void StartWatcher()
        {
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 0;
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.Start();
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            RaceMap.SetView(e.Position.Location, 16);
            lastCoordinate = e.Position.Location;
            if (!racing)
            {
                CheckForStart(e.Position.Location);
            }
            else
            {
                CheckForEnd(e.Position.Location);
            }
        }

        private void CheckForEnd(GeoCoordinate location)
        {
            var distance = PathUtil.GetDistance(new Path.Coordinate(location.Latitude, location.Longitude), currentCourse.Coordinates.Last());
            if (distance < Math.Max(location.HorizontalAccuracy, location.VerticalAccuracy))
            {
                Stats.Visibility = Visibility.Collapsed;
                Finished.Visibility = Visibility.Visible;
                FinishRace();
            }
            else
            {
                UpdateStats(location);
            }
        }

        private void FinishRace()
        {
            timer.Stop();
            end = DateTime.Now;
        }

        private void CheckForStart(GeoCoordinate location)
        {
            var distance = PathUtil.GetDistance(new Path.Coordinate(location.Latitude, location.Longitude), currentCourse.Coordinates.First());
            if (distance < Math.Max(location.HorizontalAccuracy, location.VerticalAccuracy))
            {
                Distance.Visibility = Visibility.Collapsed;
                StartRace.Visibility = Visibility.Visible;
            }
            else
            {
                Distance.Text = string.Format("You are {0} m away from the start position", distance);
            }
        }

        private void UpdateStats(GeoCoordinate location)
        {
            if (last.HasValue)
            {
                var distance = PathUtil.GetDistance(new Path.Coordinate(lastCoordinate.Latitude, lastCoordinate.Longitude),
                                                    new Path.Coordinate(location.Latitude, location.Longitude));
                var time = DateTime.Now.Subtract(last.Value);

                var kmh = distance / time.TotalSeconds;
                Speed.Text = string.Format("{0} km/h", kmh);
            }
            last = DateTime.Now;
        }

        private void StartRace_Click(object sender, RoutedEventArgs e)
        {
            StartRace.Visibility = Visibility.Collapsed;
            Stats.Visibility = Visibility.Visible;
            start = DateTime.Now;
            racing = true;
            timer.Start();
        }
    }
}