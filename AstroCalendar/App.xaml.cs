using AstroCalendar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Windows.Services.Maps;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;

namespace AstroCalendar
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        internal static ResourceLoader res;
        internal static bool BackgroundCanceled = false;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            try
            {
                res = new ResourceLoader();
                MapService.ServiceToken = res.GetString("MapToken");
            }
            catch { }
            using (var db = new LocationContext())
                {
                    db.Database.Migrate();
                }
            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                if (!await LocationManager.GetCoordinates())
                {
                    var dlg = new MessageDialog("Location is disabled. Please turn it on.");
                    dlg.Commands.Add(new UICommand { Label = "Redetect location", Id = 0 });
                    dlg.Commands.Add(new UICommand { Label = "Use it later", Id = 1 });

                    var res = await dlg.ShowAsync();
                    if ((int)res?.Id == 0) //yes                    
                        await LocationManager.GetCoordinates();
                }
            }
            catch { }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(Views.MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            BackgroundCanceled = true;
            deferral.Complete();
        }

        protected async override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            // Get a deferral, to prevent the task from closing prematurely
            // while asynchronous code is still running.
            base.OnBackgroundActivated(args);
            BackgroundTaskDeferral deferral = args.TaskInstance.GetDeferral();
            args.TaskInstance.Canceled += TaskInstance_Canceled;


            //await LocationManager.GetCoordinates();
            // Update the live tile
            try
            {
                await Task.Run(() =>
                {
                    UpdateTile();
                });
            }
            catch { }
            finally
            {
                // Inform the system that the task is finished.
                //args.TaskInstance.Canceled -= TaskInstance_Canceled;
                deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            var deferral = sender.GetDeferral();
            BackgroundCanceled = true;
            deferral.Complete();
        }

        private  void UpdateTile()
        {
            if (BackgroundCanceled) return;
            // Create a tile update manager
                   var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            if (BackgroundCanceled) return;
            updater.EnableNotificationQueue(true);
            if (BackgroundCanceled) return;
            updater.Clear();
            if (BackgroundCanceled) return;

            var sun = new Sun(DateTime.Now, SettingsManager.Position.Latitude, SettingsManager.Position.Longitude,
                                TimeZoneInfo.FindSystemTimeZoneById(SettingsManager.Position.TimeZone));
            if (BackgroundCanceled) return;
            var moon = new Moon(DateTime.Now, SettingsManager.Position.Latitude, SettingsManager.Position.Longitude,
                                TimeZoneInfo.FindSystemTimeZoneById(SettingsManager.Position.TimeZone));
            if (BackgroundCanceled) return;

            XmlDocument sunxml = new XmlDocument();
            if (BackgroundCanceled) return;
            sunxml.LoadXml(XmlTiles.GetSunXml(sun));
            if (BackgroundCanceled) return;

            // Create a new tile notification.
            updater.Update(new TileNotification(sunxml));
            if (BackgroundCanceled) return;

            XmlDocument moonxml = new XmlDocument();
            if (BackgroundCanceled) return;
            moonxml.LoadXml(XmlTiles.GetMoonXml(moon, sun));
            if (BackgroundCanceled) return;
            updater.Update(new TileNotification(moonxml));
            if (BackgroundCanceled) return;
        }
    }
}
