using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace Rapido
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();


            //PathParser.Parse(File.OpenText("MapData.xml").ReadToEnd());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PreviewCourse.xaml", UriKind.Relative));
        }
    }
}