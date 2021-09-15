using System.Windows;
using System.Windows.Input;

namespace Serato.Net.Tray.Models
{
    public class TrayViewModel
    {
        /// <summary>
        /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
        /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
        /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
        /// </summary>
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }
}