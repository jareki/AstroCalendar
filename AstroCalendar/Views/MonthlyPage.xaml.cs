using AstroCalendar.Models;
using AstroCalendar.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class MonthlyPage : Page
    {
        private double distX;

        public MonthlyPage()
        {
            this.InitializeComponent();
            DataContext = new MonthlyViewModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TypeCombo.SelectedIndex = 0;
        }

        private void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeCombo.SelectedIndex == 5) //Moon phases
            {
                //switch to PhasesGrid
                CalendarGrid.Visibility = Visibility.Collapsed;
                PhaseGrid.Visibility = Visibility.Visible;
            }

            else
            {
                //switch to CalendarGrid
                CalendarGrid.Visibility = Visibility.Visible;
                PhaseGrid.Visibility = Visibility.Collapsed;
            }
        }


        private void CalendarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarGrid.SelectedItem != null)
                (DataContext as MonthlyViewModel).ShowSunCommand?.Execute(CalendarGrid.SelectedItem);
        }

        private void PhaseGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhaseGrid.SelectedItem != null)
                (DataContext as MonthlyViewModel).ShowMoonCommand?.Execute(PhaseGrid.SelectedItem);
        }

        private void CalendarGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            distX = e.Cumulative.Translation.X;
        }

        private void CalendarGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(distX) <= 3) return;
            if (distX > 0)
                (DataContext as MonthlyViewModel).BackwardCommand?.Execute(null);
            else if (distX < 0)
                (DataContext as MonthlyViewModel).ForwardCommand?.Execute(null);
        }
    }
}
