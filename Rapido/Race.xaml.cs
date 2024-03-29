﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace Rapido
{
    class SplitTime
    {
        public Path.Coordinate Coordinate { get; set; }
        public Pushpin Pin { get; set; }
        public TimeSpan? Time { get; set; }
    }

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
        private IEnumerable<Path.Coordinate> splits;
        private List<SplitTime> splitTimes;
        private Pushpin myPushpin;
        private TransportationMode mode;

        public Race()
        {
            InitializeComponent();
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            StartWatcher();
            var app = ((App)Application.Current);
            currentCourse = app.CurrentCourse;
            mode = app.CurrentMode;
            SetupSplits();
            InitMap();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.1);
        }

        private void SetupSplits()
        {
            splitTimes = new List<SplitTime>();
            splits = PathUtil.GetSplits(currentCourse, 3);

            foreach (var split in splits)
            {
                var pin = new Pushpin();
                pin.Template = Resources["pinSplit"] as ControlTemplate;
                pin.Location = new GeoCoordinate(split.Latitude, split.Longitude);
                RaceMap.Children.Add(pin);
                splitTimes.Add(new SplitTime { Coordinate = split, Pin = pin, Time = null });
            }
        }

        private void SetMyLocation(GeoCoordinate location)
        {
            myPushpin.Location = location;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (racing)
            {
                var timespan = DateTime.Now.Subtract(start.Value);
                Elapsed.Text = PathUtil.FormatTimeSpan(timespan);
            }
        }


        private void InitMap()
        {
            myPushpin = new Pushpin();
            myPushpin.Template = Resources["pinMyLoc"] as ControlTemplate;
            RaceMap.Children.Add(myPushpin);
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
            RaceMap.SetView(e.Position.Location, 18);
            lastCoordinate = e.Position.Location;
            SetMyLocation(e.Position.Location);
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
            foreach(var split in splitTimes.Where(s => !s.Time.HasValue))
            {
                var splitDistance = PathUtil.GetDistance(new Path.Coordinate(location.Latitude, location.Longitude), split.Coordinate);
                if (splitDistance < Math.Max(location.HorizontalAccuracy, location.VerticalAccuracy))
                {
                    split.Time = DateTime.Now.Subtract(start.Value);
                    split.Pin.Template = Resources["pinSplitPassed"] as ControlTemplate;
                    UpdateStats(location);
                    break;
                }
            }
        }

        private void FinishRace()
        {
            timer.Stop();
            end = DateTime.Now;
            splitTimes.Last().Time = end.Value.Subtract(start.Value);
            racing = false;
            watcher.Stop();
            FinishTime.Text = PathUtil.FormatTimeSpan(end.Value.Subtract(start.Value));
            var result = new PathTime
                             {
                                 PathKey = currentCourse.Key,
                                 Mode = mode,
                                 User = "Chad",
                                 SplitTimes = splitTimes.Where(s => s.Time.HasValue).Select(s => s.Time.Value).ToArray(),
                                 TotalTime = end.Value.Subtract(start.Value)
                             };
            DataAccess.PostPathTime(result);
            DataAccess.Save();
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
                Distance.Text = string.Format("You are {0:0} m away from the start position", distance);
            }
        }

        private void UpdateStats(GeoCoordinate location)
        {
            var lastSplit = splitTimes.OrderByDescending(s => s.Time).First();
            var index = splitTimes.Count(c => c.Time.HasValue);
            if(lastSplit.Time.HasValue)
            {
                SplitTime.Text = PathUtil.FormatTimeSpan(lastSplit.Time.Value);
                var result = DataAccess.GetNextFastestTime(currentCourse.Key, lastSplit.Time.Value, index - 1, mode);
                if(result != null) 
                {
                    LeaderName.Text = result.User;
                    var diff = lastSplit.Time.Value.Subtract(result.SplitTimes[index]);
                    LeaderTime.Text = "+" + PathUtil.FormatTimeSpan(diff);
                }
                else
                {
                    LeaderName.Text = "N/A";
                    LeaderTime.Text = "N/A";
                }

                result = DataAccess.GetNextSlowestTime(currentCourse.Key, lastSplit.Time.Value, index, mode);
                if(result != null)
                {
                    FollowerName.Text = result.User;
                    var diff = result.SplitTimes[index].Subtract(lastSplit.Time.Value);
                    FollowerTime.Text = "-" + PathUtil.FormatTimeSpan(diff);    
                }
                else
                {
                    FollowerName.Text = "N/A";
                    FollowerTime.Text = "N/A";
                }
            }
        }

        private void StartRace_Click(object sender, RoutedEventArgs e)
        {
            StartRace.Visibility = Visibility.Collapsed;
            Stats.Visibility = Visibility.Visible;
            start = DateTime.Now;
            racing = true;
            timer.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PreviewCourse.xaml", UriKind.Relative));
        }
    }
}