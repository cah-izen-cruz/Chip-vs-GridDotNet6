using GRID.Controls;
using GRIDLibraries.Libraries;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static GRID.MessagesBox;
using Constants = Microsoft.VisualBasic.Constants;

namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        GridLib grd = new GridLib();
        public Page3()
        {
            InitializeComponent();
      
        }

        Notifier notifier = new Notifier(msg =>
        {
            msg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            msg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            msg.Dispatcher = Application.Current.Dispatcher;
        });



       
    }
}
