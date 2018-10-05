using AstroCalendar.Models;
using AstroCalendar.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AstroCalendar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SunDailyPage : Page
    {
        private double distX;

        public SunDailyPage()
        {
            this.InitializeComponent();
            DataContext = new SunDailyViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        
        private void Page_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            distX = e.Cumulative.Translation.X;
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(distX) <= 3) return;
            if (distX > 0)
                (DataContext as SunDailyViewModel).BackwardCommand?.Execute(null);
            else if (distX < 0)
                (DataContext as SunDailyViewModel).ForwardCommand?.Execute(null);
        }
    }
}
