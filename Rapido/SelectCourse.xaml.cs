using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Rapido
{
    public partial class SelectCourse : PhoneApplicationPage
    {
        public SelectCourse()
        {
            InitializeComponent();
            var context = new CourseModel {Courses = new object[10]};
            this.DataContext = context;
        }

        private void Grid_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PreviewCourse.xaml", UriKind.Relative));
        }
    }

    public class CourseModel
    {
        public object[] Courses { get; set; }
    }
}