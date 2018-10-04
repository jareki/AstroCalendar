using AstroCalendar.Dialogs;
using AstroCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AstroCalendar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DateTime date;
        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                this.RegisterBackgroundTask();
            }
            catch { }
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Split.IsPaneOpen = !Split.IsPaneOpen;
        }

        private void IconList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (IconList.SelectedIndex)
            {
                case 0: //Daily
                    BottomList.Visibility = Visibility.Visible;
                    if (BottomList.SelectedIndex==0) //Sun
                        MainFrame.Navigate(typeof(SunDailyPage), date);
                    if (BottomList.SelectedIndex == 1) //Moon
                        MainFrame.Navigate(typeof(MoonDailyPage), date);
                    break;
                case 1: //Monthly
                    BottomList.Visibility = Visibility.Collapsed;
                    MainFrame.Navigate(typeof(MonthlyPage));
                    break;
            }
            
            Split.IsPaneOpen = false;
        }
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                var param = (MainNavParam)(e.Parameter);
                date = param.Date;
                DateTxt.Text = $"{LocationManager.Geoposition.Name} (GMT{TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone).GetUtcOffset(DateTime.UtcNow).Hours})";
                BottomList.SelectedIndex = param.IsSun ? 0 : 1;
            }
            catch (InvalidCastException)
            {
                date = DateTime.Now;
                DateTxt.Text = $"{LocationManager.Geoposition.Name} (GMT{TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone).GetUtcOffset(DateTime.UtcNow).Hours})";
                BottomList.SelectedIndex = 0;
            }
            catch (ArgumentNullException)
            {
                BottomList.SelectedIndex = 0;
                DateTxt.Text = "Error";
                date = DateTime.Now;
            }
            catch { }
            finally
            {
                IconList.SelectedIndex = 0;
                base.OnNavigatedTo(e);
            }
        }

        private async void CommandList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommandList.SelectedIndex == -1) return;
            switch (((ListBoxItem)CommandList.SelectedItem).Name)
            {
                case "Locations":
                    Frame thisFrame = Window.Current.Content as Frame;
                    thisFrame.Navigate(typeof(LocationsPage));
                    break;
                case "About" :
                    AboutDialog dlg = new AboutDialog();
                    await dlg.ShowAsync();
                    break;
            }
            Split.IsPaneOpen = false;
            CommandList.SelectedIndex = -1;
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage),string.Empty);
        }

        private async void RegisterBackgroundTask()
        {
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                    backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                {
                    foreach (var task in BackgroundTaskRegistration.AllTasks)
                    {
                        if (task.Value.Name == "TileUpdateTask")
                        {
                            task.Value.Unregister(true);
                        }
                    }

                    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                    taskBuilder.Name = "TileUpdateTask";
                taskBuilder.SetTrigger(new TimeTrigger(60, false));
                taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                    
                    var registration = taskBuilder.Register();
            }
        }
    }
}
