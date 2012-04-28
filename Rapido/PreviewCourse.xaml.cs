using System;
using System.Device.Location;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;

namespace Rapido
{
    public partial class PreviewCourse : PhoneApplicationPage
    {
        public PreviewCourse()
        {
            InitializeComponent();
            InitMap();
        }

        private void InitMap()
        {
            MapPreview.SetView(new GeoCoordinate(50.454662, -104.584351), 12);
            var polyline = new MapPolyline
                               {
                                   Stroke = new SolidColorBrush(Colors.Red),
                                   StrokeThickness = 5,
                                   Opacity = 0.7,
                                   Locations = new LocationCollection
                                                   {
                                                       new GeoCoordinate(50.4280902094894, -104.606322081034),
                                                       new GeoCoordinate(50.4279601062483, -104.606063309396),
                                                       new GeoCoordinate(50.4279284965882, -104.605979504129),
                                                       new GeoCoordinate(50.4280354449858, -104.605845510811),
                                                       new GeoCoordinate(50.4281911909059, -104.605585009679)
                                                   }
                               };
            MapPreview.Children.Add(polyline);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Race.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            MapPreview.SetView(new GeoCoordinate(50.4545, -104.584351), 14);
        }
    }
}