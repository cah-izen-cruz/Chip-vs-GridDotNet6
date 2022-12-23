using GRIDLibraries.Libraries;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for BreakTime.xaml
    /// </summary>
    public partial class BreakTime : Page
    {
        GridLib grd = new GridLib();
        gridActivity curAct = default;
        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        public BreakTime()
        {
            InitializeComponent();
            lvBreak.ItemsSource = null;
            lvBreak.ItemsSource = grd.grdData._lstBreakOrig;
            //grd.conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=" + grd.grdData.TeamInfo.DBName + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";
            grd.conString = "Data Source=DESKTOP-A0R75AD;" + "Initial Catalog=TestDB" + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";
            this.GridBreakList.Visibility = Visibility.Visible;

        }

        [Obsolete]
        public void StartActivity()
        {
            grd.grdData.MainWindowAction = "";

            //this.ClearWrapPanel();

            this.MainScrn.idleCtr = 0;

            var newPerf = new gridPerformance();

            newPerf.Id = 1;
            newPerf.UserId = grd.grdData.CurrentUser.EmpNo;
            newPerf.TransDate = Convert.ToDateTime(grd.grdData.CurrentUser.TransactionDate2).ToString("MM/dd/yyyy");
            newPerf.ActivityId = grd.grdData.CurrentActivity.ActivityId;
            newPerf.TimeStart = Strings.Format(grd.grdData.CurrentActivity.TimeStart2, "MM/dd/yyyy h:mm:ss tt").ToString();
            newPerf.TimeEnd = Strings.Format(grd.grdData.CurrentActivity.TimeEnd2, "MM/dd/yyyy h:mm:ss tt").ToString();
            newPerf.TransDate2 = grd.grdData.CurrentUser.TransactionDate2;
            newPerf.TimeStart2 = grd.grdData.CurrentActivity.TimeStart2;
            newPerf.TimeEnd2 = grd.grdData.CurrentActivity.TimeEnd2;
            newPerf.TimeElapsed = Strings.LTrim(Strings.RTrim((string)MainScrn.lblTimeElapsed.Content));
            newPerf.IdleTime = grd.grdData.CurrentActivity.IdleTime;
            newPerf.Status = "0";
            newPerf.IsPaused = false;
            newPerf.ReferenceId = grd.grdData.CurrentActivity.ReferenceId;
            newPerf.PerfStatus = 0;

            newPerf.Id = grd.AddPerformanceInfo(newPerf);

            if (newPerf.Id > 0)
            {
                try
                {
                    grd.grdData.PerformanceList.Add(newPerf);
                }
                catch (Exception)
                {
                }
            }

            grd.grdData.CurrentActivity.Id = newPerf.Id;

            MainScrn.lblActivityName.Content = grd.grdData.CurrentActivity.Activity.ActName;
            MainScrn.lblAHT.Content = grd.grdData.CurrentActivity.Activity.AHT;
            MainScrn.lblProcessName.Content = grd.grdData.CurrentActivity.Activity.Process;
            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();

            MainScrn.MinuteTimer = 0;
            MainScrn.idleCtr = 0;
        }

        private void lvBreak_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvBreak.SelectedItem is not null)
            {
                grd.gridData.LOBProcess = "";
                curAct = (gridActivity)lvBreak.SelectedItem;


                if (!(curAct == null))
                {
                    grd.grdData.CurrentActivity.Started = true;

                    if (grd.grdData.CurrentActivity.Id > 0)
                    {
                        grd.grdData.MainWindowAction = "CHANGE";
                    }

                    else
                    {
                        grd.grdData.MainWindowAction = "START";
                    }

                    var withBlock = grd.grdData.CurrentActivity;

                    withBlock.Activity = curAct;
                    withBlock.ActivityId = curAct.Id;
                    withBlock.LOBId = curAct.LOBId;
                    withBlock.LOBItemId = 0;

                }

                this.StartActivity();


                this.MainScrn.btnMyActivities.IsChecked = true;
                this.MainScrn.DashMyActivities.Visibility = Visibility.Visible;
                this.GridBreakList.Visibility = Visibility.Collapsed;
                this.MainScrn.btnBreak.IsChecked = false;

                grd.grdData.ScrContent.IsActivityRunning = true;
                grd.grdData.ScrContent.IsBreakStarted = true;
                grd.grdData.ScrContent.IsMyDataChanged = true;

                //DoubleAnimation Animate = new DoubleAnimation();
                //Animate.From = 0;
                //Animate.To = 800;
                //Animate.Duration = new Duration(TimeSpan.FromSeconds(0.7));

                //this.MainScrn.fContainer.Visibility = Visibility.Visible;
                //this.MainScrn.fContainer.BeginAnimation(WidthProperty, Animate);
                this.MainScrn.fContainer.Navigate(new MyActivities());
            }
        }

        private void btnCloseMyBreak_Click(object sender, RoutedEventArgs e)
        {
            this.GridBreakList.Visibility = Visibility.Collapsed;
            this.MainScrn.btnBreak.IsChecked = false;
        }
    }
}
