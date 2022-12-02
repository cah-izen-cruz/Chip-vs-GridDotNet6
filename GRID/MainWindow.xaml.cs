using Woof.SystemEx;
using GRID.Pages;
using GRIDLibraries.Libraries;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Telerik.Windows.Documents.Model.Drawing.Charts;
using Convert = System.Convert;
using System.ComponentModel;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Constants = Microsoft.VisualBasic.Constants;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using DataGridTextColumn = System.Windows.Controls.DataGridTextColumn;

namespace GRID
{
    public partial class MainWindow : Window
    {
        string UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        FileVersionInfo GridMainVersion;

        GridLib grd = new GridLib();
        int TabCtr;
        DataTable dtCompleted;
        GridData gridData { get; set; } = GridData.gridDataStore;

        public SeriesCollection seriesCollection { get; set; }

        gridPerformance PerfObjUpdateElapsedTime;

        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        private PerspectiveCamera _perspectiveCamera = default;
        private double _angle = 0d;


        private int idleTotal;

        public bool ActivateWorkQueueTab = false;

        public int MinuteTimer = 0;

        public List<int> NotifIDsTriad = null;
        public List<int> NotifIDsCoaching = null;

        
        bool IsMainWindowInitializing = false;
        int LoadMainWindowCtr = 0;
        bool IsMainWindowLoaded = false;

        public string LiveActivityTab;

        DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer _timer = default;

        private bool BlinkOn = false;

        public DispatcherTimer ActivityElapsedTimer;
        DispatcherTimer LoadMainWindowTimer;
        private DispatcherTimer CurrentTime;
        private DispatcherTimer ForceLogOffTimer;
        private int ForceLogOffCtr = 0;


        public int idleCtr;

        bool IsDoingOvertime = false;
        long OvertimeCounter = 0L;

        public bool WithProActiveFaxQueue = false;
        public bool WithRxProActiveFaxQueue = false;
        public bool WithAuditQueue = false;
        public bool WithCSAuditQueue = false;


        int rndCtrPU = 1800;
        int rndCtrPilot = 300;
        int TimerCtrPU = 0;
        int TimerCtrPILOT = 0;
        bool UserForceLogOff = false;
        bool IsUpdateElapsedTimeEveryMin = false;

        private int NotifCtrPU = 0;
        private int NotifCtrPilot = 0;
        public bool IsDarkTheme { get; set; }

        #region "Ticks Timer"
        private void ForceLogOffTimer_Tick(object sender, EventArgs e)
        {

            if (ForceLogOffCtr >= 5)
            {
                Environment.Exit(0);
            }
            else
            {
                ForceLogOffCtr = ForceLogOffCtr + 1;
            }

        }

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {

            DateTime timeEndNow = Conversions.ToDate(Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MM/dd/yyyy h:mm:ss tt"));

            //lblCurrentDateTime.Content = Strings.Format(timeEndNow, "MMM. dd - h:mm tt");

            //if (grd.grdData.CurrentUser.ActualTagging | Conversion.Val(lblCompletedActivity.Content) == 0 & Conversion.Val(lblOpenActivity.Content) == 0)
            //{

            if (grd.grdData.CurrentActivity.Id <= 0)
            {

                lblTimeElapsed.Content = "00:00:00";


                grd.grdData.CurrentActivity.TimeStart = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet).ToString();
                grd.grdData.CurrentActivity.TimeEnd = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet).ToString();
                grd.grdData.CurrentActivity.TimeStart2 = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
                grd.grdData.CurrentActivity.TimeEnd2 = Convert.ToDateTime(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet));

                BorderTotalTime.Background = System.Windows.Media.Brushes.Transparent;

                lblStartTime.Content = "00:00:00";


                return;
            }

            else
            {
                lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            }

            //}

            if (IsDoingOvertime == true)
            {
                if (lblActivityName.Content.ToString().ToUpper() == "PRODUCTIVE")
                {
                    OvertimeCounter += 1;
                }
            }



            if (grd.grdData.CurrentActivity.Started == true)
            {

                var curElapsedTime = Conversions.ToDate(lblTimeElapsed.Content).AddSeconds(1d);

                lblTimeElapsed.Content = curElapsedTime.ToString("HH:mm:ss");

                if (grd.grdData.CurrentActivity.IsPaused == false)
                {
                    lblTimeElapsed.Content = grd.MGetTimeElapsed(grd.grdData.CurrentActivity.TimeStart2, timeEndNow, lblTimeElapsed.Content.ToString());
                }
                else
                {
                    lblTimeElapsed.Content = grd.MGetTimeElapsedPaused(grd.grdData.CurrentActivity.TimeEnd2, timeEndNow, lblTimeElapsed.Content.ToString());
                }

                grd.grdData.CurrentActivity.TimeEnd = Strings.Format(timeEndNow, "MM/dd/yyyy h:mm:ss tt");
                grd.grdData.CurrentActivity.TimeEnd2 = timeEndNow;

                CheckIfOverAHT();
            }

            else
            {
                lblTimeElapsed.Content = "00:00:00";
            }



        }

        private void CurrentTime_Tick(object sender, EventArgs e)
        {

            if (grd.grdData.CurrentUser.OnShore == false)
            {

                TimerCtrPU += 1;

                TimerCtrPILOT += 1;


                NotifCtrPilot = 0;

                if (!(NotifIDsCoaching == null))
                {
                    if (NotifIDsCoaching.Count > 0)
                    {
                        NotifCtrPilot = NotifCtrPilot + NotifIDsCoaching.Count;
                    }
                }

                if (!(NotifIDsTriad == null))
                {
                    if (NotifIDsTriad.Count > 0)
                    {
                        NotifCtrPilot = NotifCtrPilot + NotifIDsTriad.Count;
                    }
                }

                //btnNotifCount.Content = NotifCtrPU + NotifCtrPilot;
                //btnProductUPCount.Content = NotifCtrPU;

                if (NotifCtrPU > 0)
                {
                    //btnProductUPCount.Visibility = Visibility.Visible;
                }
                else
                {
                    //btnProductUPCount.Visibility = Visibility.Hidden;
                }

                //if (NotifCtrPilot > 0)
                //{
                //    btnPilotCount.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    btnPilotCount.Visibility = Visibility.Hidden;
                //}

                //if (main.Height > 450)
                //{
                //    btnNotifCount.Visibility = Visibility.Hidden;
                //    btnExpandDown.Visibility = Visibility.Hidden;
                //    btnExpandDown2.Visibility = Visibility.Hidden;
                //    btnExpandUp.Visibility = Visibility.Visible;
                //}


                //else if (NotifCtrPU + NotifCtrPilot > 0)
                //{
                //    btnNotifCount.Visibility = Visibility.Visible;
                //    btnExpandDown.Visibility = Visibility.Hidden;
                //    btnExpandUp.Visibility = Visibility.Hidden;
                //    btnExpandDown2.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    btnNotifCount.Visibility = Visibility.Hidden;
                //    btnExpandDown2.Visibility = Visibility.Hidden;
                //    btnExpandUp.Visibility = Visibility.Hidden;
                //    btnExpandDown.Visibility = Visibility.Visible;

                //}

                if (TimerCtrPU > rndCtrPU)
                {
                    rndCtrPU -= 30;
                    TimerCtrPU = 0;
                }


                if (TimerCtrPILOT > rndCtrPilot)
                {
                    rndCtrPilot -= 30;
                    TimerCtrPILOT = 0;
                }
            }



            this.ShowLunchTimeReminder();

            if (lblTimeElapsed.Content.ToString() != "00:00:00")
            {
                //this.CheckIfIdle();
            }
            else
            {
                idleCtr = 0;
            }


            DateTime date1;
            DateTime date2;
            DateTime date3;

            if (IsUpdateElapsedTimeEveryMin == false & grd.grdData.CurrentActivity.Id > 0 & grd.grdData.CurrentUser.AutoSynch)
            {

                if (grd.grdData.CurrentUser.LogOut.Date == Conversions.ToDate("1/1/0001"))
                    grd.grdData.CurrentUser.LogOut = grd.grdData.CurrentActivity.TimeEnd2;
                int Ctr = (int)DateAndTime.DateDiff(DateInterval.Second, grd.grdData.CurrentUser.LogOut, grd.grdData.CurrentActivity.TimeEnd2);



                if (Ctr >= 90 | Ctr <= -90)
                {

                    IsUpdateElapsedTimeEveryMin = true;

                    PerfObjUpdateElapsedTime = new gridPerformance();

                    {
                        var withBlock = PerfObjUpdateElapsedTime;
                        withBlock.Id = grd.grdData.CurrentActivity.Id;
                        withBlock.TimeEnd2 = grd.grdData.CurrentActivity.TimeEnd2;
                        withBlock.TimeElapsed = lblTimeElapsed.Content.ToString();
                        withBlock.IdleTime = grd.grdData.CurrentActivity.IdleTime;
                    }

                    try
                    {
                        //var bwUpdateElapsedTime = new BackgroundWorker();
                        //bwUpdateElapsedTime.DoWork += UpdateElapsedTimeEveryMin;
                        //bwUpdateElapsedTime.RunWorkerCompleted += UpdateElapsedTimeEveryMinComplete;
                        //bwUpdateElapsedTime.RunWorkerAsync();
                    }

                    catch (Exception ex)
                    {
                        IsUpdateElapsedTimeEveryMin = false;
                    }


                    grd.grdData.CurrentUser.LogOut = grd.grdData.CurrentActivity.TimeEnd2;

                }
            }


            if (DateTime.Now.TimeOfDay.Seconds == grd.grdData.CurrentUser.LogIn.Second)
            {

                if (!((Environment.UserName.ToUpper() ?? "") == ("armand.zablan".ToUpper() ?? "")))
                {

                    if (UserForceLogOff == false)
                    {
                        //try
                        //{
                        //    var bwForceLogOff = new BackgroundWorker();
                        //    bwForceLogOff.DoWork += CheckUserForceLogOff;
                        //    bwForceLogOff.RunWorkerCompleted += this.CheckUserForceLogOffComplete;
                        //    bwForceLogOff.RunWorkerAsync();
                        //}

                        //catch (Exception ex)
                        //{
                        //    this.CheckUserForceLogOffComplete();
                        //}
                    }


                    else
                    {

                        try
                        {

                            date2 = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);

                            if (grd.grdData.CurrentUser.SchedLogOut < date2)
                            {
                                if (DateAndTime.DateDiff(DateInterval.Hour, grd.grdData.CurrentUser.SchedLogOut, date2) > 11L)
                                {
                                    if (grd.grdData.CurrentUser.WithShifdateOption == false)
                                    {
                                        ForceLogOffTimer.Start();
                                        Interaction.MsgBox("Session timeout please login again!", Constants.vbExclamation, grd.AppName);
                                        return;
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
            }

            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(DateTime.Now.TimeOfDay.Seconds, grd.grdData.CurrentUser.LogIn.Second > 30 ? grd.grdData.CurrentUser.LogIn.Second - 30 : grd.grdData.CurrentUser.LogIn.Second + 29, false)))
            {
                //if (IsPopulateAgentMetrics == true)
                //{
                //    IsPopulateAgentMetrics = false;
                //    try
                //    {
                //        this.PopulateAgentMetrics();
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}


            }





        }

        private void ShowLunchTimeReminder()
        {
            //imgBreakAlert.Visibility = Visibility.Hidden;
            //lblBreakTime.Content = "";
            //lblBreakTime.Visibility = Visibility.Hidden;

            grd.grdData.ScrContent.IsBreaktime = false;


            DateTime date1;
            DateTime date2;
            DateTime date3;


            try
            {

                date2 = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

                date2 = Convert.ToDateTime(date2.Date.ToShortDateString() + " " + (grd.gridData.CurrentUser.FirstBreak).ToString());

                date2 = TimeZoneInfo.ConvertTime(date2, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                date3 = date2.AddMinutes(15d);

                date1 = date2.AddMinutes(-5);


                //if (DateTime.Now >= date1 & DateTime.Now <= date2)
                //{
                //    var Result = date2 - DateTime.Now;
                //    //grd.grdData.ScrContent.IsBreaktime = true;
                //    //grd.grdData.ScrContent.BreaktimeName = "First Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "First Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //}

                //else if (DateTime.Now >= date2 & DateTime.Now <= date3)
                //{
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "First Break!";
                //}
            }


            catch (Exception)
            {
            }

            try
            {

                date2 = Conversions.ToDate(DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

                date2 = TimeZoneInfo.ConvertTime(date2, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

                date2 = Convert.ToDateTime(date2.Date.ToShortDateString() + " " + (grd.grdData.CurrentUser.LunchBreak).ToString());

                date2 = TimeZoneInfo.ConvertTime(date2, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                date3 = date2.AddMinutes(60d);

                date1 = date2.AddMinutes(-5);



                //if (DateTime.Now >= date1 & DateTime.Now <= date2)
                //{
                //    var Result = date2 - DateTime.Now;
                //    //grd.grdData.ScrContent.IsBreaktime = true;
                //    //grd.grdData.ScrContent.BreaktimeName = "Lunch Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "Lunch Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //}


                //else if (DateTime.Now >= date2 & DateTime.Now <= date3)
                //{   ////txtEmpBreak.Text = "Lunch Break!";                
                //    //grd.grdData.ScrContent.IsBreaktime = true;
                //    //grd.grdData.ScrContent.BreaktimeName = "Lunch Break!";
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "Lunch Break!";
                //}
            }

            catch (Exception ex)
            {

            }

            try
            {


                date2 = Conversions.ToDate(DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

                date2 = TimeZoneInfo.ConvertTime(date2, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

                date2 = Convert.ToDateTime(date2.Date.ToShortDateString() + " " + (grd.gridData.CurrentUser.SecondBreak).ToString());

                date2 = TimeZoneInfo.ConvertTime(date2, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                date3 = date2.AddMinutes(15d);

                date1 = date2.AddMinutes(-5);


                //if (DateTime.Now >= date1 & DateTime.Now <= date2)
                //{
                //    var Result = date2 - DateTime.Now;
                //    //grd.grdData.ScrContent.IsBreaktime = true;
                //    //grd.grdData.ScrContent.BreaktimeName = "Second Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "Second Break in " + Result.Minutes.ToString() + ":" + Strings.Format(Result.Seconds, "00").ToString();
                //}

                //else if (DateTime.Now >= date2 & DateTime.Now <= date3)
                //{
                //    //grd.grdData.ScrContent.IsBreaktime = true;
                //    //grd.grdData.ScrContent.BreaktimeName = "Second Break!";
                //    grd.grdData.ScrContent.IsBreaktime = true;
                //    imgBreakAlert.Visibility = Visibility.Visible;
                //    //lblBreakTime.Visibility = Visibility.Visible;
                //    lblBreakTime.Content = "Second Break!";

                //}
            }


            catch (Exception ex)
            {

            }


            try
            {


                date2 = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);

                if (grd.grdData.CurrentUser.SchedLogOut < date2)
                {
                    grd.grdData.ScrContent.IsBreaktime = true;
                    //imgBreakAlert.Visibility = Visibility.Visible;
                    //lblBreakTime.Visibility = Visibility.Visible;
                    //lblBreakTime.Content = "Overtime!";
                    //grd.grdData.ScrContent.IsBreaktime = true;
                    //grd.grdData.ScrContent.BreaktimeName = "Overtime!";
                }
            }


            catch (Exception ex)
            {

            }
        }

        private void CheckIfOverAHT()
        {
            DateTime curElapsedTime = Conversions.ToDate(lblTimeElapsed.Content);
            DateTime curActivityAHT = Conversions.ToDate(lblAHT.Content);

            // convert to seconds
            int icurElapsedTime_inSec = curElapsedTime.Hour * 60 * 60 + curElapsedTime.Minute * 60 + curElapsedTime.Second;
            int icurActivityAHT_inSec = curActivityAHT.Hour * 60 * 60 + curActivityAHT.Minute * 60 + curActivityAHT.Second;

            int volumeCount = 1;

            double ratioElapsedToAHT = icurElapsedTime_inSec * 1 / (double)(icurActivityAHT_inSec * volumeCount);

            if (ratioElapsedToAHT > 1d)
            {
                BorderTotalTime.Background = System.Windows.Media.Brushes.Red; // G_red
            }
            else if (ratioElapsedToAHT >= 0.8d)
            {
                BorderTotalTime.Background = System.Windows.Media.Brushes.Orange; // G_Orange
            }
            else
            {
                BorderTotalTime.Background = System.Windows.Media.Brushes.Green; // G_Green
            }


            if (grd.grdData.CurrentActivity.Id <= 0)
            {
                BorderTotalTime.Background = System.Windows.Media.Brushes.Red; // G_red
            }

        }
        #endregion

        #region "OpenCompletedActivities"
        private void LoadOpenActivity()
        {

            lvOpenActivities.ItemsSource = null;

            var AddRF = new List<string>();
            int ActId = 0;

            if (cmbOpenActName.SelectedIndex == 0)
            {
                AddRF = grd.GetConfigFieldName(0);
            }
            else
            {
                try
                {
                    gridActivity act = (gridActivity)cmbOpenActName.SelectedItem;
                    ActId = act.Id;
                    AddRF = grd.GetConfigFieldName(act.Id);
                }

                catch (Exception ex)
                {

                }

            }

            var gView = new GridView();

            //gView.AllowsColumnReorder = true;
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "ID", DisplayMemberBinding = new Binding("Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "ActId", DisplayMemberBinding = new Binding("Activity.Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Activity Name", DisplayMemberBinding = new Binding("Activity.ActName") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Started", DisplayMemberBinding = new Binding("TimeStart") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Duration", DisplayMemberBinding = new Binding("TimeElapsed") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Type", DisplayMemberBinding = new Binding("Activity.Type") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Process", DisplayMemberBinding = new Binding("Activity.Process") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Ref No.", DisplayMemberBinding = new Binding("ReferenceId") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Item Id", DisplayMemberBinding = new Binding("LOBItemId"), Width = 0 });


            if (!(AddRF == null))
            {
                if (AddRF.Count > 0)
                {

                    for (int i = 0, loopTo = AddRF.Count - 1; i <= loopTo; i++)
                        gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = AddRF[i].ToString(), DisplayMemberBinding = new Binding("PerfConfigData.RF" + Strings.Format(i + 1, "00")), Width = 100 });

                }
            }

            lvOpenActivities.View = gView;

            lvOpenActivities.ItemsSource = grd.GetPerformances(ActId, 1);



            TabItem1.Header = "(" + lvOpenActivities.Items.Count + ") " + " Open";

        }

        private void LoadCompletedActivity()
        {


            dtCompleted = new DataTable();

            lvClosedActivities.ItemsSource = null;

            //lblTotalAudit.Visibility = Visibility.Hidden;
            //btnExportCompleted.Visibility = Visibility.Hidden;
            var AddRF = new List<string>();

            int ActId = 0;
            if (cmbCompletedActName.SelectedIndex == 0)
            {
                AddRF = grd.GetConfigFieldName(0);
            }
            else
            {
                try
                {
                    gridActivity act = (gridActivity)cmbCompletedActName.SelectedItem;
                    ActId = act.Id;
                    AddRF = grd.GetConfigFieldName(act.Id);
                }
                catch (Exception ex)
                {

                }

            }

            var gView = new GridView();

            dtCompleted.Columns.Add("Id");
            dtCompleted.Columns.Add("Activity.ActName");
            dtCompleted.Columns.Add("Activity.Type");
            dtCompleted.Columns.Add("Activity.Process");
            dtCompleted.Columns.Add("TimeStart");
            dtCompleted.Columns.Add("TimeEnd");
            dtCompleted.Columns.Add("TimeElapsed");
            dtCompleted.Columns.Add("ReferenceId");


            gView.AllowsColumnReorder = true;
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Id", DisplayMemberBinding = new Binding("Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Description", DisplayMemberBinding = new Binding("Activity.ActName") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Type", DisplayMemberBinding = new Binding("Activity.Type") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Process", DisplayMemberBinding = new Binding("Activity.Process") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Started", DisplayMemberBinding = new Binding("TimeStart") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Ended", DisplayMemberBinding = new Binding("TimeEnd") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Duration", DisplayMemberBinding = new Binding("TimeElapsed") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Ref No.", DisplayMemberBinding = new Binding("ReferenceId") });



            if (!(AddRF == null))
            {
                if (AddRF.Count > 0)
                {

                    for (int i = 0, loopTo = AddRF.Count - 1; i <= loopTo; i++)
                    {
                        try
                        {
                            dtCompleted.Columns.Add(AddRF[i].ToString());
                        }
                        catch (Exception ex)
                        {

                        }


                        gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = AddRF[i].ToString(), DisplayMemberBinding = new Binding("PerfConfigData.RF" + Strings.Format(i + 1, "00")), Width = 100 });
                    }

                }
            }



            lvClosedActivities.View = gView;

            lvClosedActivities.ItemsSource = grd.GetPerformances(ActId, 2);

            tabItem2.Header = "(" + lvClosedActivities.Items.Count + ")" + " Completed";
        }

        #endregion

        #region "Window Buttons"   
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            this.DashMyData.Visibility = Visibility.Collapsed;
            //DoubleAnimation Animate = new DoubleAnimation();

            //Animate.From = 0;
            //Animate.To = 800;
            //Animate.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(0.5));

            fContainer.Visibility = Visibility.Visible;
            //fContainer.BeginAnimation(WidthProperty, Animate);
            fContainer.Navigate(new Logout());
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region "Navigation Buttons"  
        private void btnMyData_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnMyData;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "My Data  ";
            }
        }
        private void btnMyData_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }
        private void btnMyActivities_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnMyActivities;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "My Activities";
            }
        }
        private void btnMyActivities_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }
        private void btnBreak_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnBreak;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Take a Break!";
            }
        }
        private void btnBreak_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }
        private void btnLogout_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnLogout;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Until next time!";
            }
        }
        private void btnLogout_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }
    
        private void btnSettings_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnSettings;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Settings";
            }
        }
        private void btnSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }


        #endregion

        #region "Agent Metrics"
        private void PopulateOpenActivitiesMetrics()
        {
            //if ((int)lblOpenActivity.Content > 0)
            //{
            //    OpenAct.Background = System.Windows.Media.Brushes.Red; // G_red
            //    OpenAct.Background = System.Windows.Media.Brushes.Green; // G_Green
            //}

        }

        private void PopulateOpenAndCloseActivities()
        {
            //lblOpenActivity.Content = grd.GetOpenPerformancesLocalCount();
            this.PopulateOpenActivitiesMetrics();

            //lblCompletedActivity.Content = grd.GetClosedPerformancesLocalCount();
            this.PopulateCompletedActivitiesMetrics();
        }

        private void PopulateCompletedActivitiesMetrics()
        {

            //if ((int)lblCompletedActivity.Content > 0)
            //{
            //    RadCompleted.Background = System.Windows.Media.Brushes.Green; // G_Green
            //}
            //else
            //{
            //    RadCompleted.Background = System.Windows.Media.Brushes.Red; // G_red
            //}

        }

        private void PopulateTransDateMetrics()
        {

            if (grd.grdData.CurrentUser.IsLate == true)
            {
                //BorderTransDate.Background = System.Windows.Media.Brushes.Red; // G_red
            }
            else
            {
                //BorderTransDate.Background = System.Windows.Media.Brushes.Green; // G_Green
            }

        }

        private void PopulateEfficiencyMetrics()
        {


            //lblEfficiency.Content = Interaction.IIf(grd.grdData.MainAgentMetrics.Efficiency > 0, Strings.Format(grd.grdData.MainAgentMetrics.Efficiency, "#0.##%"), "0.00%");

            lblEfficiency.Foreground = System.Windows.Media.Brushes.White;
            //lblEfficiency.Foreground = Brushes.White;

            //if (grd.grdData.MainAgentMetrics.Efficiency >= 1.2d)
            //{
            //    //radButtonEff.Background = Brushes.Orange; // G_Orange
            //    lblEfficiency.Foreground = System.Windows.Media.Brushes.Black;
            //    lblEfficiency.Foreground = System.Windows.Media.Brushes.Orange;
            //}
            //else if (grd.grdData.MainAgentMetrics.Efficiency >= 0.95d)
            //{
            //    //radButtonEff.Background = Brushes.Green; // G_Green
            //    lblEfficiency.Foreground = System.Windows.Media.Brushes.Green;
            //}
            //else
            //{
            //    //radButtonEff.Background = Brushes.Red; // G_red
            //    lblEfficiency.Foreground = System.Windows.Media.Brushes.Red;
            //} 

        }

        private void PopulateProdtimeMetrics()
        {

            //lblProductiveTime.Content = grd.grdData.MainAgentMetrics.ProductionTime;


            //lblProductiveTime.Foreground = Brushes.White;
            //lblProductiveTime.Foreground = Brushes.White;
            if (grd.grdData.MainAgentMetrics.ProdTimeSec >= 24660)
            {

                //BorderProdTime.Background = System.Windows.Media.Brushes.Green; // G_Green
            }


            else if (grd.grdData.MainAgentMetrics.ProdTimeSec >= 23427)
            {
                //BorderProdTime.Background = System.Windows.Media.Brushes.Orange; // G_Orange
                //lblProductiveTime.Foreground = Brushes.Black;
                //lblProductiveTime.Foreground = Brushes.Black;
            }
            else
            {
                //BorderProdTime.Background = System.Windows.Media.Brushes.Red; // G_red

            }



        }

        private void PopulateBreaktimeMetrics()
        {

            //lblBreaktime.Content = grd.grdData.MainAgentMetrics.BreakTime;

            //txtBreakTime.Foreground = Brushes.White;
            //lblBreakTime.Foreground = Brushes.White;


            if (grd.grdData.MainAgentMetrics.BreakTimeSec > 5400)
            {
                //BorderBreaktime.Background = System.Windows.Media.Brushes.Red; // G_red
            }
            // ElseIf grd.griddata.MainAgentMetrics.BreakTimeSec >= 4860 Then
            // txtBreakTimebg.Fill = G_Orange
            else if (grd.grdData.MainAgentMetrics.BreakTimeSec >= 1)
            {
                //BorderBreaktime.Background = System.Windows.Media.Brushes.Green; // G_Green
            }
            else
            {
                //BorderBreaktime.Background = System.Windows.Media.Brushes.Orange; // G_Orange

                //txtBreakTime.Foreground = Brushes.Black;
                //lblBreakTime.Foreground = Brushes.Black;

            }





        }

        private void PopulateShrinktimeMetrics()
        {

            //lblShrinkageTime.Content = grd.grdData.MainAgentMetrics.ShrinkageTime;


            //if (grd.grdData.MainAgentMetrics.ShrinkTimeSec > 2340)
            // {
            //BorderShrinkage.Background = System.Windows.Media.Brushes.Red; // G_red
            //}

            // ElseIf grd.griddata.MainAgentMetrics.ShrinkTimeSec >= 1 Then
            // txtShrinkageTimebg.Fill = Windows.Media.Brushes.Green
            //else
            //{
            //BorderShrinkage.Background = System.Windows.Media.Brushes.Green; // G_Green
            //} 


        }

        public void PopulateAgentMetrics()
        {

            //if (Conversion.Val(txtQuality.Text) > 95)
            //{
            //    radButtonQuality.Background = Brushes.Green; // G_Green 'Windows.Media.Brushes.Green
            //}
            //else if (Val(txtQuality.Text) < 96 & Val(txtQuality.Text) > 89)
            //{
            //    radButtonQuality.Background = Brushes.Orange; // G_Orange
            //}
            //else
            //{
            //    radButtonQuality.Background = Brushes.Red; // G_red
            //} 




            this.PopulateOpenAndCloseActivities();

            //grd.UpdateMainAgentMetrics(grd.grdData.CurrentUser.TransactionDate); commented sept 6 2022


            //this.PopulateEfficiencyMetrics();

            //this.PopulateProdtimeMetrics();

            //this.PopulateBreaktimeMetrics();

            //this.PopulateTransDateMetrics();

            //var _totalShrinkTime = DateTime.Parse("0001-01-01");

            //long _tempTotalTime = grd.grdData.MainAgentMetrics.ProdTimeSec + grd.grdData.MainAgentMetrics.BreakTimeSec + grd.grdData.MainAgentMetrics.ShrinkTimeSec;
            //long _tempTotalLogTime = GetTotalLogTime(Conversions.ToDate(grd.grdData.CurrentUser.LogIn).ToString(), Conversions.ToDate(grd.grdData.CurrentUser.LogOut).ToString());

            //if (_tempTotalTime < _tempTotalLogTime)
            //{
            //    grd.grdData.MainAgentMetrics.ShrinkTimeSec = _tempTotalLogTime - grd.grdData.MainAgentMetrics.ProdTimeSec - grd.grdData.MainAgentMetrics.BreakTimeSec;
            //    var newTime = new TimeSpan(0, 0, (int)grd.grdData.MainAgentMetrics.ShrinkTimeSec);

            //    //_totalShrinkTime.Add(newTime);
            //    grd.grdData.MainAgentMetrics.ShrinkageTime = newTime.ToString();


            //}
            //if (grd.grdData.MainAgentMetrics.ProdTimeSec > 0)
            //{
            //    grd.grdData.MainAgentMetrics.Utilization = grd.grdData.MainAgentMetrics.ProdTimeSec / _tempTotalLogTime;
            //}
            //else
            //{
            //    grd.grdData.MainAgentMetrics.Utilization = 0;
            //}


            //// MsgBox(CDate(grd.griddata.CurrentUser.ActualLogIn))
            //lblProductiveTime.Content = grd.grdData.MainAgentMetrics.ProductionTime;
            //lblShrinkageTime.Content = grd.grdData.MainAgentMetrics.ShrinkageTime;
            //lblBreakTime.Content = grd.grdData.MainAgentMetrics.BreakTime;
            //this.PopulateUtilizationMetrics();
            //this.PopulateShrinktimeMetrics();

        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            GridMainVersion = FileVersionInfo.GetVersionInfo(Environment.CurrentDirectory + @"\GRID.exe");
            //lblFileVer.Content = GridMainVersion.FileVersion + Environment.NewLine + GridMainVersion.LegalCopyright;
            iconInfo.ToolTip = GridMainVersion.FileVersion + Environment.NewLine + GridMainVersion.LegalCopyright;

            ImgUser.ImageSource = new BitmapImage(new Uri(SysInfo.GetUserPicturePath(UserName)));

            grd.grdData.CurrentActivity.TimeStart = grd.grdData.CurrentUser.LogOut.ToString();
            grd.grdData.CurrentActivity.TimeEnd = grd.grdData.CurrentUser.LogOut.ToString();
            grd.grdData.CurrentActivity.TimeStart2 = grd.grdData.CurrentUser.LogOut;
            grd.grdData.CurrentActivity.TimeEnd2 = Convert.ToDateTime(grd.grdData.CurrentUser.LogOut);
            grd.grdData.CurrentActivity.TransDate2 = grd.grdData.CurrentUser.TransactionDate2;
            grd.grdData.TeamInfo.DBName = grd.grdData.TeamInfo.DBName;

            grd.conString = "Data Source=DESKTOP-A0R75AD;" + "Initial Catalog=" + grd.grdData.TeamInfo.DBName + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";

            idleCtr = 0;
            idleTotal = 0;


            try
            {
                DisplayUserInfoToScreen();
                LoadMainWindowTimer = new DispatcherTimer();
                LoadMainWindowTimer.Tick += LoadMainWindow_Tick;
                LoadMainWindowTimer.Interval = new TimeSpan(0, 0, 1);
            }
            catch (Exception)
            {
            }
           
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            this.DashMyData.Visibility = Visibility.Visible;

            btnMyData.IsChecked = true;
            //fContainer.Navigate(new System.Uri("Pages/Page1.xaml", UriKind.RelativeOrAbsolute));

            Style style = new Style();
            style.TargetType = typeof(GridViewColumnHeader);
            style.Setters.Add(new Setter(GridViewColumnHeader.HeightProperty, 30d));
            GvCompleted.ColumnHeaderContainerStyle = style;

        }

        private void DisplayUserInfoToScreen()
        {
            this.lblTower.Content = grd.grdData.TeamInfo.Tower;
            this.lblDepartment.Content = grd.grdData.TeamInfo.Cluster;
            this.lblSegment.Content = grd.grdData.TeamInfo.Segment;
            this.lblUserName.Content = grd.grdData.CurrentUser.LastName + ", " + grd.grdData.CurrentUser.FirstName; /*+ " " + grd.grdData.CurrentUser.MiddleName;*/
          
            try
            {
                cmbCompletedActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 2);
                cmbCompletedActName.SelectedIndex = 0;
                cmbOpenActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 1);
                cmbOpenActName.SelectedIndex = 0;

                this.LoadOpenActivity();
                this.LoadCompletedActivity();

                if (TabCtr == 1)
                {
                    TabControl1.SelectedIndex = 1;
                }
                else
                {
                    TabControl1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void LoadActivitySetup(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                grd.CreateCopyOfPendingActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2);
            }
            catch (Exception ex)
            {
            }


            try
            {
                grd.MIPopulateActList();
            }
            catch (Exception ex)
            {

            }

            try
            {
                LoadActivityList();
            }
            catch (Exception ex)
            {

            }


            try
            {
                grd.MICurrentActivitiesToOpeMain(grd.grdData.CurrentUser.EmpNo.ToString());
            }
            catch (Exception ex)
            {

            }


            try
            {
                grd.MISavePerfToLocal();
            }
            catch (Exception ex)
            {

            }

            //QA Questionnaire Data
            try
            {
                grd.MIPopulateQAList();
            }
            catch (Exception ex)
            {

            }

            try
            {  
                grd.grdData.QuestionForm.dtLOB = grd.GetQALob();
                grd.grdData.QuestionForm.dtQAQuestionnaire = grd.GetQAQuestionnaires();
                grd.grdData.QuestionForm.dtQASelection = grd.GetQASelection();
                grd.grdData.QuestionForm.dtQAMarkdownSelection = grd.GetQAMarkdown();
                //grd.grdData.QuestionForm.dtQAContainers = grd.GetQAContainers();
            }
            catch (Exception ex)
            {

            }

            IsMainWindowInitializing = false;

        }
        private void LoadActivityList()
        {
            grd.grdData._lstMyActivitiesOrig = grd.GetFavoriteActivities();
            grd.grdData._lstProductivityOrig = new List<gridActivity>();
            grd.grdData._lstBreakOrig = new List<gridActivity>();

            if (!(grd.grdData.ActivityList == null))
            {
                if (grd.grdData.ActivityList.Count > 0)
                {
                    for (int i = 0, loopTo = grd.grdData.ActivityList.Count - 1; i <= loopTo; i++)
                    {
                        switch (grd.grdData.ActivityList[i].Type.ToUpper())
                        {
                            case var @case when @case == "Productive".ToUpper():
                                {
                                    grd.grdData._lstProductivityOrig.Add(grd.grdData.ActivityList[i]);
                                 
                                    break;
                                }
                            case var case1 when case1 == "Shrinkage".ToUpper():
                                {
                                    //_lstShrinkageOrig.Add(grd.grdData.ActivityList[i]);
                                    break;
                                }
                            case var case2 when case2 == "Admin".ToUpper():
                                {
                                    //_lstAdminTaskOrig.Add(grd.grdData.ActivityList[i]);
                                    break;
                                }
                            case var case3 when case3 == "Break".ToUpper():
                                {
                                    grd.grdData._lstBreakOrig.Add(grd.grdData.ActivityList[i]);
                                    break;
                                }
                            case var case4 when case4 == "PILOT".ToUpper():
                                {
                                    // _lstPilotOrig.Add(grd.grdData.ActivityList[i]);
                                    break;
                                }
                        }
                    }
                }
            }

            //lvMyActivities.ItemsSource = null;
            //GvProductivity.ItemsSource = null;
            //lvBreak.ItemsSource = null;
            //GvProductivity.ItemsSource = grd.grdData._lstProductivityOrig;           
            //lvMyActivities.ItemsSource = grd.grdData._lstMyActivitiesOrig;
            //lvBreak.ItemsSource = grd.grdData._lstBreakOrig;

            //this.PopulateProcessAndSubProcessMyActivities();
            //this.PopulateProcessAndSubProcessProductivity();
        }
        private void LoadMainWindow_Tick(object sender, EventArgs e)
        {
            if (IsMainWindowLoaded == true)
            {
                if (IsMainWindowInitializing == false)
                {

                    LoadMainWindowTimer.Stop();

                    this.InitializeMainWindow();

                    BusyIndicator.IsBusy = false;

                }
            }

            else
            {

                LoadMainWindowCtr += 1;

                if (LoadMainWindowCtr > 2)
                {

                    IsMainWindowLoaded = true;

                    IsMainWindowInitializing = true;

                    BusyIndicator.IsBusy = true;
                    var bw = new BackgroundWorker();
                    bw.DoWork += this.LoadActivitySetup;
                    bw.RunWorkerCompleted += DoneImport;

                    bw.RunWorkerAsync();
                  
                }
            }

        }
        private void DoneImport(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(UpdateUI));
            }
            catch (Exception ex)
            {
            }
        }
        private void UpdateUI()
        {

        }
        private void InitializeMainWindow()
        {
            var ids1 = new List<int>();
            var Ids2 = new List<int>();
            var Ids3 = new List<int>();
            var Ids4 = new List<int>();
            var Ids5 = new List<int>();
            var Ids6 = new List<int>();
            var Ids7 = new List<int>();

            var IdsMain = new List<int>();

            //this.PopulateAgentMetrics();
            //this.DisplayButtonsPerPrivelege();


            var random = new Random();
            rndCtrPU = random.Next(300, 1800);
            rndCtrPilot = random.Next(60, 300);

            this.InitializeTimers();

            this.ResetValues();

            lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            grd.grdData.CurrentActivity.Id = 0;
            grd.grdData.CurrentActivity.Started = false;
            grd.grdData.CurrentActivity.IsPaused = false;
            grd.grdData.CurrentActivity.IdleTime = 0;
            lblTimeElapsed.Content = "00:00:00";
            MinuteTimer = 0;
            ActivityElapsedTimer.Start();


            grd.gridData.MainWindowAction = "";

            grd.gridMainDbConnectionState = true;
        }
        private void InitializeTimers()
        {
            ForceLogOffTimer = new DispatcherTimer();
            ForceLogOffTimer.Tick += ForceLogOffTimer_Tick;
            ForceLogOffTimer.Interval = new TimeSpan(0, 0, 1);
            ForceLogOffTimer.Stop();

            ActivityElapsedTimer = new DispatcherTimer();
            ActivityElapsedTimer.Tick += ElapsedTimer_Tick;
            ActivityElapsedTimer.Interval = new TimeSpan(0, 0, 1);

            CurrentTime = new DispatcherTimer();
            CurrentTime.Tick += CurrentTime_Tick;
            CurrentTime.Interval = new TimeSpan(0, 0, 1);
            CurrentTime.Start();
        }
        public void ResetValues()
        {
            grd.grdData.CurrentActivity.Reset();
            lblAHT.Content = "00:00:00";
            lblActivityName.Content = "";
        }

        private void home_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)

        {
            Tg_Btn.IsChecked = false;
        }
      
        private void btnMyData_Click(object sender, RoutedEventArgs e)
        {
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            this.DashMyData.Visibility = Visibility.Visible;
            fContainer.Visibility = Visibility.Collapsed;
            //fContainer.Navigate(new Page1());         
        }

        private void btnMyActivities_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Visibility = Visibility.Visible;
            //DoubleAnimation Animate = new DoubleAnimation();

            //Animate.From = 0;
            //Animate.To = 800;
            //Animate.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(0.5));

            this.DashMyActivities.Visibility = Visibility.Visible;
            this.DashMyData.Visibility = Visibility.Collapsed;
            //this.DashMyActivities.BeginAnimation(WidthProperty, Animate);
            fContainer.Navigate(new MyActivities());
        }

        private void btnBreak_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Visibility = Visibility.Visible;
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            this.DashMyData.Visibility = Visibility.Collapsed;
            fContainer.Navigate(new BreakTime());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Visibility = Visibility.Visible;
            this.DashMyData.Visibility = Visibility.Collapsed;
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            fContainer.Navigate(new Settings());
        }


        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMainWindowTimer.Start();
            BusyIndicator.IsBusy = true;
        }

        private void btnHammer_Click(object sender, RoutedEventArgs e)
        {
            //DoubleAnimation Animate = new DoubleAnimation();

            //Animate.From = 0;
            //Animate.To = 800;
            //Animate.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(0.5));

            

            fContainer.Visibility = Visibility.Visible;
            this.DashMyData.Visibility = Visibility.Collapsed;
            this.DashMyActivities.Visibility = Visibility.Collapsed;
            fContainer.Navigate(new QAQuestionnaire());

            //this.fContainer.BeginAnimation(WidthProperty, Animate);
        }

        private void btnHammer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnNotification;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "GRID Level 5";
            }
        }

        private void btnHammer_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnNotification_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnNotification;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Notifications";
            }
        }

        private void btnNotification_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnCompletedRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCompletedActivity();
        }

        private void btnCompletedRefresh_Click_1(object sender, RoutedEventArgs e)
        {
            LoadCompletedActivity();
        }
    }
}
