using GRID.Controls;
using GRIDLibraries.Libraries;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static GRID.MessagesBox;
using Constants = Microsoft.VisualBasic.Constants;
using Convert = System.Convert;


namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for MyActivities.xaml
    /// </summary>
    public partial class MyActivities : Page
    {
        GridLib grd = new GridLib();

        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        gridActivity curAct = default;

        public RadComboBox dd = new RadComboBox();
        public RadComboBox mdd = new RadComboBox();
        public RadWatermarkTextBox tb = new RadWatermarkTextBox();
        public TextBlock lb = new TextBlock();

        WrapPanel xx = new WrapPanel();

        public string Question;
        public string Category;

        DataTable dt = new DataTable();
        DataRow dr;

        private int _ConfigCtr = 0;
        public int _ScoreAverage { get; set; }

        private long PrevActId = 0L;

        private DispatcherTimer ActivityElapsedTimer;

        public bool IsMyActivitiesActivated = false;
        public bool ReStartActivity = false;

        public int idleCtr;
        public MyActivities()
        {
            InitializeComponent();

            this.MainScrn.ActivityElapsedTimer.Start();

            grd.grdData.TeamInfo.DBName = grd.grdData.TeamInfo.DBName;

            //grd.conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=" + grd.grdData.TeamInfo.DBName + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";
            grd.conString = "Data Source=DESKTOP-A0R75AD;" + "Initial Catalog=" + grd.grdData.TeamInfo.DBName + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";

            lvMyActivities.ItemsSource = null;
            lvProductivity.ItemsSource = null;
            cmbQAForm.ItemsSource = null;

            lvProductivity.ItemsSource = grd.grdData._lstProductivityOrig;
            lvMyActivities.ItemsSource = grd.grdData._lstMyActivitiesOrig;
            cmbQAForm.ItemsSource = grd.grdData._lstQAQuestions;

            this.PopulateProcessAndSubProcessMyActivities();
            this.PopulateProcessAndSubProcessProductivity();

            this.MainScrn.DashMyActivities.Visibility = Visibility.Visible;

            if (grd.grdData.ScrContent.IsActivityRunning)
            {
                btnStart.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Visible;
                btnPause.Visibility = Visibility.Visible;
                btnChange.Visibility = Visibility.Visible;

                if (!grd.grdData.ScrContent.IsBreakStarted)
                    MatDesActivityList.Visibility = Visibility.Collapsed;
            }
            else
            {
                grd.grdData.ScrContent.IsMyDataChanged = false;
                btnStart.Visibility = Visibility.Visible;
                btnStop.Visibility = Visibility.Collapsed;
                btnPause.Visibility = Visibility.Collapsed;
                btnChange.Visibility = Visibility.Collapsed;
            }

            ActivityElapsedTimer = new DispatcherTimer();
            ActivityElapsedTimer.Tick += ElapsedTimer_Tick;
            ActivityElapsedTimer.Interval = new TimeSpan(0, 0, 1);

            this.GridDynamicObjects.Visibility = Visibility.Collapsed;
            this.MatDesGridQA.Visibility = Visibility.Collapsed;
            this.AttContorls.Visibility = Visibility.Collapsed;
            grd.grdData._lstQAAttachments.Clear();
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

        #region "Populate Process and Subprocess combo boxes"
        private void PopulateProcessAndSubProcessMyActivities() // 11-13
        {

            // ======= My Activities ============
            var lstProcess = new List<string>();

            foreach (var act in grd.grdData._lstMyActivitiesOrig)


                lstProcess.Add(Conversions.ToString(act.Process));

            lstProcess = lstProcess.Distinct().ToList();

            cmbProcess.Items.Clear();

            foreach (var proc in lstProcess)
                cmbProcess.Items.Add(proc);


        }

        private void PopulateProcessAndSubProcessProductivity() // 11-13
        {

            var lstProcess = new List<string>();

            foreach (var act in grd.grdData._lstProductivityOrig)

                lstProcess.Add(Conversions.ToString(act.Process));

            lstProcess = lstProcess.Distinct().ToList();

            cmbProcessProd.Items.Clear();

            foreach (var proc in lstProcess)
                cmbProcessProd.Items.Add(proc);

        }

        #endregion

        #region "Populate Question Forms"
        private void PopulateQuestionForms()
        {
            //lstQAForm.Items.Clear();
            //string _Formula;
            //foreach (DataRow row in grd.grdData.QAQuestion.dtLOB.Rows)
            //{
            //    if (Convert.ToInt32(row["Formula"].ToString()) == 1)
            //        _Formula = "Sum";
            //    else
            //        _Formula = "Average";

            //    lstQAForm.Items.Add(new { Id = row["Id"], Name = row["Name"], Formula = _Formula, Target = row["Target"] });

            //}

            //lstQAForm.ItemsSource = grd.grdData.QAQuestion.dtLOB.DefaultView;
        }
        #endregion

        #region "Event Handlers"
        private void MainWindowActivated()
        {
            if (grd.grdData.CurrentActivity.Id <= 0)
            {

                switch (grd.grdData.MainWindowAction)
                {

                    case "START":
                        {

                            try
                            {
                                this.StartActivity();
                            }

                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }
                    case "STOP":
                        {

                            try
                            {
                                //this.ResumeActivity();
                            }

                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }

                    case "PAUSE":
                        {

                            try
                            {
                                //this.ResumeActivity();
                            }

                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }

                    case "RESUME":
                        {

                            try
                            {
                                this.ResumeActivity();
                            }

                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }

                    default:
                        {

                            return;
                        }

                }
            }

            else if (grd.grdData.CurrentActivity.Started == true & grd.grdData.CurrentActivity.Id > 0)
            {


                switch (grd.grdData.MainWindowAction)
                {

                    case "CHANGE":
                        {

                            try
                            {
                                this.ChangeActivity();
                            }
                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }

                    case "RESUME":
                        {

                            try
                            {
                                this.ResumeActivity();
                            }

                            catch (Exception ex)
                            {
                                _eMsgBox(ex.Message);
                            }

                            break;
                        }

                    default:
                        {

                            return;
                        }

                }


            }
        }
        #endregion

        #region "MainEvents"
        private void ClickGotoActivityList(object sender, RoutedEventArgs e)
        {


            if (grd.grdData.CurrentActivity.Id > 0)
            {
                Interaction.MsgBox("There's a running activity. Please pause if you want to start another.", Constants.vbInformation, grd.AppName);
                grd.grdData.MainWindowAction = "";
                return;
            }


            try
            {
                //GoToActivitityList();
            }
            catch (Exception ex)
            {
                _eMsgBox("clickGotoActivityList ", ex.Message);
            }
        }
        #endregion

        #region "MAIN PROCESS"
        [Obsolete]
        public void StartActivity()
        {
            grd.grdData.MainWindowAction = "";

            this.ClearWrapPanel();

            idleCtr = 0;

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


            if (!(grd.grdData.CurrentActivity.Activity.ConfigInfo == null))
            {
                if (grd.grdData.CurrentActivity.Activity.ConfigInfo.Count > 0)
                {

                    if (curAct.Id == 17)
                    {
                        this.MatDesGridQA.Visibility = Visibility.Visible;
                        this.QAWrapPanel.Visibility = Visibility.Visible;
                        this.MatDesActivityList.Visibility = Visibility.Collapsed;
                        this.GridDynamicObjects.Visibility = Visibility.Collapsed;
                        this.WrapPanelMain.Visibility = Visibility.Collapsed;
                        this.WrapPanelMain2.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.SetupDynamicConfigInfo();

                        this.MatDesGridQA.Visibility = Visibility.Collapsed;
                        this.MatDesActivityList.Visibility = Visibility.Collapsed;
                        this.GridDynamicObjects.Visibility = Visibility.Visible;
                        this.WrapPanelMain.Visibility = Visibility.Visible;
                        this.WrapPanelMain2.Visibility = Visibility.Visible;
                        this.WrapPanelMain.Width = 380;
                        this.WrapPanelMain2.Width = 380;
                        this.QAWrapPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }


            newPerf.Id = grd.AddPerformanceInfo(newPerf);

            if (newPerf.Id > 0)
            {
                try
                {
                    grd.grdData.PerformanceList.Add(newPerf);
                }
                catch (Exception ex)
                {
                }
            }

            grd.grdData.CurrentActivity.Id = newPerf.Id;

            MainScrn.lblActivityName.Content = grd.grdData.CurrentActivity.Activity.ActName;
            MainScrn.lblAHT.Content = grd.grdData.CurrentActivity.Activity.AHT;
            MainScrn.lblProcessName.Content = grd.grdData.CurrentActivity.Activity.Process;
            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            MainScrn.MinuteTimer = 0;
            idleCtr = 0;

            notifier.ShowInformation("  Your Activity Started @ " + this.MainScrn.lblStartTime.Content);
        }

        [Obsolete]
        public void StartSameActivity()
        {
            grd.grdData.MainWindowAction = "";

            this.ClearWrapPanel();

            idleCtr = 0;

            grd.grdData.CurrentActivity.ActivityId = (int)PrevActId;
            grd.grdData.CurrentActivity.Activity = null;

            var q = from p in grd.grdData.ActivityList
                    where p.Id == PrevActId
                    select p;

            if (!(q == null))
            {
                if (q.Count() > 0)
                {
                    grd.grdData.CurrentActivity.Activity = q.First();
                }
            }


            var newPerf = new gridPerformance();

            newPerf.Id = 1;
            newPerf.UserId = grd.grdData.CurrentUser.EmpNo;
            newPerf.TransDate = Strings.Format(grd.grdData.CurrentUser.TransactionDate2, "MM/dd/yyyy").ToString();
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

            newPerf.ReferenceId = "";
            newPerf.PerfStatus = 0;


            if (!(grd.grdData.CurrentActivity.Activity.ConfigInfo == null))
            {
                if (grd.grdData.CurrentActivity.Activity.ConfigInfo.Count > 0)
                {
                    this.SetupDynamicConfigInfo();
                }
            }



            for (int z = 0; z <= 4; z++)
            {

                if (newPerf.Id > 1)
                {
                    break;
                }
                else
                {
                    newPerf.Id = grd.AddPerformanceInfo(newPerf);
                }

                if (newPerf.Id > 1)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                }

            }


            if (newPerf.Id > 0)
            {


                try
                {
                    grd.grdData.PerformanceList.Add(newPerf);
                }
                catch (Exception ex)
                {
                }



                //if (grd.MAddPerformanceLocal(newPerf) == false)
                //{

                //    this.WrapPanelMain.Children.Clear();
                //    this.WrapPanelMain2.Children.Clear();
                //    grd.grdData.CurrentActivity.Reset();
                //    this.ResetValues();
                //    Interaction.MsgBox("Cannot start activity. Unable to connect to local database, please try again.", Constants.vbExclamation, grd.AppName);
                //    return;
                //}
            }
            else
            {

                this.WrapPanelMain.Children.Clear();
                this.WrapPanelMain2.Children.Clear();
                grd.grdData.CurrentActivity.Reset();
                this.ResetValues();
                Interaction.MsgBox("Cannot start activity. Unable to connect to the server, please check LAN connection.", Constants.vbExclamation, grd.AppName);
                return;

            }


            this.GridDynamicObjects.Visibility = Visibility.Visible;
            this.WrapPanelMain.Visibility = Visibility.Visible;
            this.WrapPanelMain2.Visibility = Visibility.Visible;
            this.WrapPanelMain.Width = 380;
            this.WrapPanelMain2.Width = 380;



            grd.grdData.CurrentActivity.Id = newPerf.Id;


            MainScrn.lblActivityName.Content = grd.grdData.CurrentActivity.Activity.ActName;
            MainScrn.lblAHT.Content = grd.grdData.CurrentActivity.Activity.AHT;
            MainScrn.lblProcessName.Content = grd.grdData.CurrentActivity.Activity.Process;
            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            MainScrn.MinuteTimer = 0;

            this.MainScrn.MinuteTimer = 0;

            this.changelblbg();

            idleCtr = 0;

        }

        public void StopActivity()
        {
            if (!(grd.grdData.CurrentActivity.Id > 0))
            {
                new MessagesBox("You have no running activity To Stop.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            var curStopPerf = new gridPerformance();
            curStopPerf.PerfConfigData = null;

            var withBlock = curStopPerf;
            withBlock.Id = grd.grdData.CurrentActivity.Id;
            withBlock.UserId = grd.grdData.CurrentUser.EmpNo;
            withBlock.ActivityId = grd.grdData.CurrentActivity.ActivityId;
            withBlock.TransDate = grd.grdData.CurrentUser.TransactionDate;
            withBlock.TimeStart = grd.grdData.CurrentActivity.TimeStart;
            withBlock.TimeEnd = grd.grdData.CurrentActivity.TimeEnd;
            withBlock.TransDate2 = grd.grdData.CurrentUser.TransactionDate2;
            withBlock.TimeStart2 = grd.grdData.CurrentActivity.TimeStart2;
            withBlock.TimeEnd2 = grd.grdData.CurrentActivity.TimeEnd2;
            withBlock.TimeElapsed = (string)MainScrn.lblTimeElapsed.Content;
            withBlock.IdleTime = grd.grdData.CurrentActivity.IdleTime;
            withBlock.Status = "2";
            withBlock.IsPaused = grd.grdData.CurrentActivity.IsPaused;
            withBlock.PerfStatus = 2;
            withBlock.ReferenceId = grd.grdData.CurrentActivity.ReferenceId;

            if (!(grd.grdData.CurrentActivity.Activity.ConfigInfo == null))
            {
                if (grd.grdData.CurrentActivity.Activity.ConfigInfo.Count > 0)
                {
                    if(curStopPerf.ActivityId == 17)
                    {
                        grd.grdData.CurrentPerfInfo = new gridPerfInfo();
                        grd.grdData.CurrentPerfInfo.PerfId = curStopPerf.Id;

                        if (!(grd.grdData.QAQuestion.ItemId == 0))
                        {
                            this.grd.UpdateQAItem(grd.grdData.QAQuestion.ItemId, grd.grdData.QAQuestion.ScoreAverage, grd.grdData.QAQuestion.ScoreMarkings, grd.grdData.CurrentPerfInfo.PerfId);
                            StopQAActivity();
                        }

                        else
                            return;
                        
                        
                    }
                    else
                    {
                        if (CheckDynamicConfigData() == false)
                            return;

                        curStopPerf.PerfConfigData = this.GetDynamicConfigData();
                        curStopPerf.PerfConfigData.PerfId = curStopPerf.Id;


                        if (grd.MEditPerformanceMain(curStopPerf))
                        {
                            try
                            {
                                var tempPerf = new gridPerformance();
                                var q = from p in grd.gridData.PerformanceList
                                        where p.Id == curStopPerf.Id
                                        select p;

                                if (!(q == null))
                                {
                                    if (q.Count() > 0)
                                    {
                                        tempPerf = q.First();
                                        grd.grdData.PerformanceList.Remove(tempPerf);
                                    }
                                }

                                grd.grdData.PerformanceList.Add(curStopPerf); //-look up ng open and completed
                            }

                            catch (Exception ex)
                            {
                            }

                            grd.MUpdatePerfInfoMain((int)curStopPerf.Id, curStopPerf.PerfConfigData);

                        }
                        else
                        {
                            new MessagesBox("Activity was Not successfully saved. Please Try again", MessageType.Warning, MessageButtons.Ok).ShowDialog();
                            grd.grdData.MainWindowAction = "";
                            return;
                        }

                        if (grd.grdData.CurrentActivity.Activity.Type.ToUpper() == "PRODUCTIVE" & grd.grdData.CurrentActivity.Activity.IsPublic)
                        {
                            PrevActId = grd.grdData.CurrentActivity.Activity.Id;
                        }
                        else
                        {
                            PrevActId = 0;
                        }
                    }
                   
                }
            }

      

            this.ClearWrapPanel();

            grd.grdData.CurrentActivity.Reset();
            this.ResetValues();

            MainScrn.ActivityElapsedTimer.Stop();

            grd.grdData.CurrentActivity.TimeStart = curStopPerf.TimeEnd; // timeEndNow
            grd.grdData.CurrentActivity.TimeEnd = curStopPerf.TimeEnd;
            grd.grdData.CurrentActivity.TimeStart2 = curStopPerf.TimeEnd2;
            grd.grdData.CurrentActivity.TimeEnd2 = curStopPerf.TimeEnd2;
            grd.grdData.CurrentActivity.TransDate2 = curStopPerf.TransDate2;

            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            grd.grdData.CurrentActivity.Id = 0;

            grd.grdData.CurrentActivity.IsPaused = false;
            grd.grdData.CurrentActivity.IdleTime = 0;
            grd.grdData.CurrentActivity.Started = false;
            MainScrn.lblTimeElapsed.Content = "00:00:00";
            MainScrn.MinuteTimer = 0;

            MainScrn.ActivityElapsedTimer.Start();

            grd.gridData.MainWindowAction = "";

            this.changelblbg();

            grd.grdData.CurrentUser.LogOut = grd.grdData.CurrentActivity.TimeEnd2;
            grd.grdData.CurrentActivity.Started = false;

            notifier.ShowSuccess("Activity successfully saved!");

            idleCtr = 0;

            if (PrevActId > 0)
            {
                bool? Result = new MessagesBox("Do you want to work on the same activity?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

                if (Result.Value)
                {
                    this.StartSameActivity();
                }
                else
                {
                    //this.GoToActivitityList();
                    return;
                }

            }

        }

        public bool PauseActivity(bool _IsAuto)
        {
            bool PauseActivityRet = default;

            PauseActivityRet = false;

            grd.grdData.MainWindowAction = "";

            bool IsCommentsChanged = false;

            grd.grdData.MainWindowAction = "";


            string PrevActType = null;

            var curPausePerf = new gridPerformance();
            curPausePerf.PerfConfigData = null;
            curPausePerf.Id = grd.grdData.CurrentActivity.Id;
            curPausePerf.UserId = grd.grdData.CurrentUser.EmpNo;
            curPausePerf.TransDate = Strings.Format(grd.grdData.CurrentUser.TransactionDate2, "MM/dd/yyyy").ToString();
            curPausePerf.ActivityId = grd.grdData.CurrentActivity.ActivityId;
            curPausePerf.TimeStart = grd.grdData.CurrentActivity.TimeStart;
            curPausePerf.TimeEnd = Strings.Format(grd.grdData.CurrentActivity.TimeEnd2, "MM/dd/yyyy h:mm:ss tt").ToString();
            curPausePerf.TransDate2 = grd.grdData.CurrentUser.TransactionDate2;
            curPausePerf.TimeStart2 = grd.grdData.CurrentActivity.TimeStart2;
            curPausePerf.TimeEnd2 = grd.grdData.CurrentActivity.TimeEnd2;

            curPausePerf.TimeElapsed = Strings.LTrim(Strings.RTrim((string)MainScrn.lblTimeElapsed.Content));
            curPausePerf.IdleTime = grd.grdData.CurrentActivity.IdleTime;
            curPausePerf.Status = "1";
            curPausePerf.IsPaused = true;
            curPausePerf.PerfStatus = 1;


            if (!(grd.grdData.CurrentActivity.Activity.ConfigInfo == null))
            {
                if (grd.grdData.CurrentActivity.Activity.ConfigInfo.Count > 0)
                {
                    curPausePerf.PerfConfigData = this.GetDynamicConfigData();
                    curPausePerf.PerfConfigData.PerfId = curPausePerf.Id;
                }
            }


            if (grd.MEditPerformanceMain(curPausePerf))
            {
                try
                {
                    var tempPerf = new gridPerformance();
                    var q = from p in grd.grdData.PerformanceList
                            where p.Id == curPausePerf.Id
                            select p;

                    if (!(q == null))
                    {
                        if (q.Count() > 0)
                        {
                            tempPerf = q.First();
                            grd.grdData.PerformanceList.Remove(tempPerf);
                        }
                    }

                    grd.grdData.PerformanceList.Add(curPausePerf);
                }

                catch (Exception ex)
                {
                }

                grd.MUpdatePerfInfoMain(Convert.ToInt32(curPausePerf.Id), curPausePerf.PerfConfigData);


                PrevActId = grd.grdData.CurrentActivity.Activity.Id;
                try
                {
                    PrevActType = grd.grdData.CurrentActivity.Activity.Type.ToUpper();
                }
                catch (Exception)
                {
                }

                PauseActivityRet = true;
            }


            else
            {

                if (_IsAuto == false)
                {
                    Interaction.MsgBox("Activity was not successfully Paused. Please try again", Constants.vbInformation, grd.AppName);
                }

                grd.grdData.MainWindowAction = "";
                return false;

            }

            this.ClearWrapPanel();


            grd.gridData.CurrentActivity.Reset();
            this.ResetValues();

            this.MainScrn.ActivityElapsedTimer.Stop();

            grd.gridData.LOBProcess = "";



            grd.grdData.CurrentActivity.TimeStart = Strings.Format(curPausePerf.TimeEnd2, "MM/dd/yyyy h:mm:ss tt").ToString();
            grd.grdData.CurrentActivity.TimeStart2 = curPausePerf.TimeEnd2;
            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            grd.grdData.CurrentActivity.Id = 0;
            grd.grdData.CurrentActivity.Started = true;
            grd.grdData.CurrentActivity.IsPaused = false;
            grd.grdData.CurrentActivity.IdleTime = 0;

            MainScrn.lblTimeElapsed.Content = "00:00:00";
            MainScrn.MinuteTimer = 0;


            // grd.gridData.CurrentActivity.Activity.ConfigInfo = Nothing

            this.MainScrn.ActivityElapsedTimer.Start();
            this.MainScrn.lblTimeElapsed.Content = "Izen";



            this.changelblbg();

            grd.grdData.CurrentUser.LogOut = grd.grdData.CurrentActivity.TimeEnd2;


            this.MainScrn.PopulateAgentMetrics();

            idleCtr = 0;

            if (_IsAuto == false)
            {
                //this.GoToActivitityList();
            }

            return PauseActivityRet;
        }

        public void ResumeActivity()
        {

        }

        public void ChangeActivity()
        {

        }

     
        #endregion

        #region "Level4 SetupDynamicConfigInfo"
        private void SetupQADynamicConfigInfo()
        {
            #region "Clean"
            this.QAWrapPanel.Children.Clear();
            try
            {
                for (int i = 1; i <= 50; i++)
                {

                    object tempObj = this.FindName("objScore" + Strings.Format(i, "00"));
                    if (!(tempObj == null))
                    {
                        this.UnregisterName("objScore" + Strings.Format(i, "00"));
                    }

                    object tempObj2 = this.FindName("objMd" + Strings.Format(i, "00"));
                    if (!(tempObj2 == null))
                    {
                        this.UnregisterName("objMd" + Strings.Format(i, "00"));
                    }

                    object tempObj3 = this.FindName("objRemarks" + Strings.Format(i, "00"));
                    if (!(tempObj3 == null))
                    {
                        this.UnregisterName("objRemarks" + Strings.Format(i, "00"));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #endregion

            dt = new DataTable();
            grd.grdData.QAQuestion.dtObjContainer = new DataTable();
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Name");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("QID");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Category");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Question");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("DDNameSel");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("DDNameMD");
            grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Remarks");

            QAQuestionForm QA = (QAQuestionForm)cmbQAForm.SelectedItem;

            grd.grdData.QAQuestion.LOBId = QA.LOBId;
            grd.grdData.QAQuestion.Formula = QA.Formula.ToString();
            grd.grdData.QAQuestion.Name = QA.Name.ToString();
            grd.grdData.QAQuestion.Target = Convert.ToInt32(QA.Target);

            if (grd.grdData.QAQuestion.LOBId.ToString() != "")
            {
                DataRow[] result = grd.grdData.QAQuestion.dtQAQuestionnaire.Select("LOBId = '" + grd.grdData.QAQuestion.LOBId + "'");
                foreach (DataRow row in result)
                {
                    if (dt.Rows.Count == 0)
                    {
                        dt.Columns.Add("Name");
                        dt.Columns.Add("QID");
                        dt.Columns.Add("Question");
                        dt.Columns.Add("ObjectType");
                        dt.Columns.Add("Category");
                        dt.Columns.Add("Description");

                        dr = dt.NewRow();
                        dr["Name"] = row["Name"];
                        dr["QID"] = row["FormId"];
                        dr["Question"] = row["Question"];
                        dr["ObjectType"] = row["ObjectType"];
                        dr["Category"] = row["Category"];
                        dr["Description"] = row["Description"];
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr["Name"] = row["Name"];
                        dr["QID"] = row["FormId"];
                        dr["Question"] = row["Question"];
                        dr["ObjectType"] = row["ObjectType"];
                        dr["Category"] = row["Category"];
                        dr["Description"] = row["Description"];
                        dt.Rows.Add(dr);
                    }

                    dt.AcceptChanges();
                }
            }

            _ConfigCtr = dt.Rows.Count;

            int loopCtr = 0;
            int loopMd = 0;
            int loopTb = 0;

            for (int k = 0; k < dt.Rows.Count; k++)
            {

                grd.grdData.QAQuestion.drObjContainer = grd.grdData.QAQuestion.dtObjContainer.NewRow();
                grd.grdData.QAQuestion.drObjContainer["Name"] = dt.Rows[k]["Name"];
                grd.grdData.QAQuestion.drObjContainer["QID"] = dt.Rows[k]["QID"];
                grd.grdData.QAQuestion.drObjContainer["Category"] = dt.Rows[k]["Category"];

                grd.grdData.QAQuestion.QID = Convert.ToInt32(dt.Rows[k]["QID"]);

                var z = new TextBlock();
                z.Text = dt.Rows[k]["Category"].ToString();
                z.TextWrapping = TextWrapping.Wrap;
                z.Width = 400;
                z.Height = 24;
                z.FontSize = 13;
                z.FontWeight = FontWeights.Bold;
                z.HorizontalAlignment = HorizontalAlignment.Left;
                z.Foreground = System.Windows.Media.Brushes.YellowGreen;
                z.Margin = new Thickness(4, 2, 0, 0);
                z.VerticalAlignment = VerticalAlignment.Top;

                if (dt.Rows[k]["Category"].ToString() != Category)
                    this.QAWrapPanel.Children.Add(z);

                Category = dt.Rows[k]["Category"].ToString();
                Question = dt.Rows[k]["Question"].ToString();
                int Q;

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    Q = r + 1;
                    if (Question.ToString() == dt.Rows[r]["Question"].ToString())
                    {
                        var x = new WrapPanel();
                        x.Background = System.Windows.Media.Brushes.Transparent;
                        x.VerticalAlignment = VerticalAlignment.Top;
                        x.HorizontalAlignment = HorizontalAlignment.Left;
                        x.Margin = new Thickness(4, -10, 1, 1);
                        x.Orientation = Orientation.Horizontal;

                        grd.grdData.QAQuestion.drObjContainer["Question"] = dt.Rows[r]["Question"];

                        var y = new TextBlock();
                        y.Text = "Q" + Q + ": " + dt.Rows[r]["Question"].ToString();
                        y.TextWrapping = TextWrapping.Wrap;
                        y.Width = 250;
                        y.FontSize = 13;
                        y.HorizontalAlignment = HorizontalAlignment.Left;
                        y.Foreground = System.Windows.Media.Brushes.White;
                        y.Margin = new Thickness(1, 6, 1, 1);
                        y.VerticalAlignment = VerticalAlignment.Top;

                        dd = new RadComboBox();
                        dd.Width = 100;
                        dd.Height = 24;
                        dd.FontSize = 11;
                        dd.HorizontalContentAlignment = HorizontalAlignment.Left;
                        dd.VerticalContentAlignment = VerticalAlignment.Center;
                        dd.VerticalAlignment = VerticalAlignment.Top;
                        dd.Foreground = System.Windows.Media.Brushes.Black;
                        dd.Margin = new Thickness(8, -1, 1, 1);
                        dd.IsEditable = false;
                        dd.SelectedValuePath = "Score";
                        dd.DisplayMemberPath = "Value";

                        dd.SelectionChanged += btnHandler_Click;

                        dd.Name = QAGetObjectNameScore(loopCtr);
                        this.RegisterName(dd.Name, dd);

                        x.Children.Add(y);
                        x.Children.Add(dd);

                        grd.grdData.QAQuestion.drObjContainer["DDNameSel"] = dd.Name;

                        mdd = new RadComboBox();
                        mdd.Width = 100;
                        mdd.Height = 24;
                        mdd.FontSize = 11;
                        mdd.HorizontalContentAlignment = HorizontalAlignment.Left;
                        mdd.VerticalContentAlignment = VerticalAlignment.Center;
                        mdd.VerticalAlignment = VerticalAlignment.Top;
                        mdd.Foreground = System.Windows.Media.Brushes.Black;
                        mdd.Margin = new Thickness(4, -1, 1, 1);
                        mdd.IsEditable = false;
                        mdd.SelectedValuePath = "Id";
                        mdd.DisplayMemberPath = "Value";

                        mdd.SelectionChanged += btnHandler_Click;

                        mdd.Name = QAGetObjectNameMd(loopMd);
                        this.RegisterName(mdd.Name, mdd);

                        x.Children.Add(mdd);

                        grd.grdData.QAQuestion.drObjContainer["DDNameMD"] = mdd.Name;

                        tb = new RadWatermarkTextBox();
                        tb.Text = "Put a remarks here..."; /*dt.Rows[r]["Description"].ToString();*/
                        tb.Width = 250;
                        tb.Height = 24;
                        tb.FontSize = 11;
                        tb.HorizontalContentAlignment = HorizontalAlignment.Left;
                        tb.VerticalContentAlignment = VerticalAlignment.Center;
                        tb.VerticalAlignment = VerticalAlignment.Top;
                        tb.Foreground = System.Windows.Media.Brushes.Black;
                        tb.Margin = new Thickness(4, -1, 1, 1);

                        tb.Name = QAGetObjectNameRem(loopTb);
                        this.RegisterName(tb.Name, tb);

                        x.Children.Add(tb);

                        grd.grdData.QAQuestion.drObjContainer["Remarks"] = tb.Name;

                        this.QAWrapPanel.Children.Add(x);

                        DataRow[] res = grd.grdData.QAQuestion.dtQASelection.Select("FormId = '" + dt.Rows[k]["QID"] + "'");
                        foreach (DataRow row in res)
                        {
                            dd.Items.Add(new { Value = row["Value"], Score = row["Score"] });
                        }

                        DataRow[] resMd = grd.grdData.QAQuestion.dtQAMarkdownSelection.Select("QID = '" + grd.grdData.QAQuestion.QID + "'");
                        foreach (DataRow row in resMd)
                        {
                            mdd.Items.Add(new { Value = row["Value"], Id = row["Id"] });
                        }

                        grd.grdData.QAQuestion.dtObjContainer.Rows.Add(grd.grdData.QAQuestion.drObjContainer);
                        grd.grdData.QAQuestion.dtObjContainer.AcceptChanges();

                        loopCtr = loopCtr + 1;
                        loopMd = loopMd + 1;
                        loopTb = loopTb + 1;
                    }
                }

                var l = new TextBlock();
                l.Text = "Description: " + dt.Rows[k]["Description"].ToString();
                l.TextWrapping = TextWrapping.Wrap;
                l.Width = 400;
                l.Height = 24;
                l.FontSize = 11;
                l.FontStyle = FontStyles.Italic;
                //l.FontWeight = FontWeights.Bold;
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.Foreground = System.Windows.Media.Brushes.White;
                l.Margin = new Thickness(6, -1, 1, 1);
                l.VerticalAlignment = VerticalAlignment.Top;

                this.QAWrapPanel.Children.Add(l);


            }

            if (_ConfigCtr > 0)
            {
                xx = new WrapPanel();
                xx.Background = System.Windows.Media.Brushes.Transparent;
                xx.VerticalAlignment = VerticalAlignment.Top;
                xx.HorizontalAlignment = HorizontalAlignment.Left;
                xx.Margin = new Thickness(4, 6, 1, 1);
                xx.Orientation = Orientation.Vertical;
            }

            //#region "Clean"
            //this.QAWrapPanel.Children.Clear();
            //try
            //{
            //    for (int i = 1; i <= 50; i++)
            //    {

            //        object tempObj = this.FindName("objScore" + Strings.Format(i, "00"));
            //        if (!(tempObj == null))
            //        {
            //            this.UnregisterName("objScore" + Strings.Format(i, "00"));
            //        }

            //        object tempObj2 = this.FindName("objMd" + Strings.Format(i, "00"));
            //        if (!(tempObj2 == null))
            //        {
            //            this.UnregisterName("objMd" + Strings.Format(i, "00"));
            //        }

            //        object tempObj3 = this.FindName("objRemarks" + Strings.Format(i, "00"));
            //        if (!(tempObj3 == null))
            //        {
            //            this.UnregisterName("objRemarks" + Strings.Format(i, "00"));
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            //#endregion

            //dt = new DataTable();
            //grd.grdData.QAQuestion.dtObjContainer = new DataTable();
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Name");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("QID");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Category");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Question");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("DDNameSel");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("DDNameMD");
            //grd.grdData.QAQuestion.dtObjContainer.Columns.Add("Remarks");

            //for (int i = lstQAForm.SelectedItems.Count - 1; i >= 0; i -= 1)
            //{
            //    var obj = lstQAForm.SelectedItems[i];
            //    var id = obj.GetType().GetProperties().First(o => o.Name == "LOBId").GetValue(obj, null);
            //    var Name = obj.GetType().GetProperties().First(o => o.Name == "Name").GetValue(obj, null);
            //    var Formula = obj.GetType().GetProperties().First(o => o.Name == "Formula").GetValue(obj, null);
            //    var Target = obj.GetType().GetProperties().First(o => o.Name == "Target").GetValue(obj, null);

            //    grd.grdData.QAQuestion.Formula = Formula.ToString();
            //    grd.grdData.QAQuestion.Name = Name.ToString();
            //    grd.grdData.QAQuestion.Target = Convert.ToInt32(Target);


            //    if (id.ToString() != "")
            //    {
            //        DataRow[] result = grd.grdData.QAQuestion.dtQAQuestionnaire.Select("LOBId = '" + id + "'");
            //        foreach (DataRow row in result)
            //        {
            //            if (dt.Rows.Count == 0)
            //            {
            //                dt.Columns.Add("Name");
            //                dt.Columns.Add("QID");
            //                dt.Columns.Add("Question");
            //                dt.Columns.Add("ObjectType");
            //                dt.Columns.Add("Category");
            //                dt.Columns.Add("Description");

            //                dr = dt.NewRow();
            //                dr["Name"] = row["Name"];
            //                dr["QID"] = row["FormId"];
            //                dr["Question"] = row["Question"];
            //                dr["ObjectType"] = row["ObjectType"];
            //                dr["Category"] = row["Category"];
            //                dr["Description"] = row["Description"];
            //                dt.Rows.Add(dr);
            //            }
            //            else
            //            {
            //                dr = dt.NewRow();
            //                dr["Name"] = row["Name"];
            //                dr["QID"] = row["FormId"];
            //                dr["Question"] = row["Question"];
            //                dr["ObjectType"] = row["ObjectType"];
            //                dr["Category"] = row["Category"];
            //                dr["Description"] = row["Description"];
            //                dt.Rows.Add(dr);
            //            }

            //            dt.AcceptChanges();
            //        }
            //    }
            //}

            //_ConfigCtr = dt.Rows.Count;

            //int loopCtr = 0;
            //int loopMd = 0;
            //int loopTb = 0;

            //for (int k = 0; k < dt.Rows.Count; k++)
            //{

            //    grd.grdData.QAQuestion.drObjContainer = grd.grdData.QAQuestion.dtObjContainer.NewRow();
            //    grd.grdData.QAQuestion.drObjContainer["Name"] = dt.Rows[k]["Name"];
            //    grd.grdData.QAQuestion.drObjContainer["QID"] = dt.Rows[k]["QID"];
            //    grd.grdData.QAQuestion.drObjContainer["Category"] = dt.Rows[k]["Category"];

            //    grd.grdData.QAQuestion.QID = Convert.ToInt32(dt.Rows[k]["QID"]);

            //    var z = new TextBlock();
            //    z.Text = dt.Rows[k]["Category"].ToString();
            //    z.TextWrapping = TextWrapping.Wrap;
            //    z.Width = 400;
            //    z.Height = 24;
            //    z.FontSize = 13;
            //    z.FontWeight = FontWeights.Bold;
            //    z.HorizontalAlignment = HorizontalAlignment.Left;
            //    z.Foreground = System.Windows.Media.Brushes.YellowGreen;
            //    z.Margin = new Thickness(4, 2, 0, 0);
            //    z.VerticalAlignment = VerticalAlignment.Top;

            //    if (dt.Rows[k]["Category"].ToString() != Category)
            //        this.QAWrapPanel.Children.Add(z);

            //    Category = dt.Rows[k]["Category"].ToString();
            //    Question = dt.Rows[k]["Question"].ToString();
            //    int Q;

            //    for (int r = 0; r < dt.Rows.Count; r++)
            //    {
            //        Q = r + 1;
            //        if (Question.ToString() == dt.Rows[r]["Question"].ToString())
            //        {
            //            var x = new WrapPanel();
            //            x.Background = System.Windows.Media.Brushes.Transparent;
            //            x.VerticalAlignment = VerticalAlignment.Top;
            //            x.HorizontalAlignment = HorizontalAlignment.Left;
            //            x.Margin = new Thickness(4, -10, 1, 1);
            //            x.Orientation = Orientation.Horizontal;

            //            grd.grdData.QAQuestion.drObjContainer["Question"] = dt.Rows[r]["Question"];

            //            var y = new TextBlock();
            //            y.Text = "Q" + Q + ": " + dt.Rows[r]["Question"].ToString();
            //            y.TextWrapping = TextWrapping.Wrap;
            //            y.Width = 250;
            //            y.FontSize = 13;
            //            y.HorizontalAlignment = HorizontalAlignment.Left;
            //            y.Foreground = System.Windows.Media.Brushes.White;
            //            y.Margin = new Thickness(1, 6, 1, 1);
            //            y.VerticalAlignment = VerticalAlignment.Top;

            //            dd = new RadComboBox();
            //            dd.Width = 100;
            //            dd.Height = 24;
            //            dd.FontSize = 11;
            //            dd.HorizontalContentAlignment = HorizontalAlignment.Left;
            //            dd.VerticalContentAlignment = VerticalAlignment.Center;
            //            dd.VerticalAlignment = VerticalAlignment.Top;
            //            dd.Foreground = System.Windows.Media.Brushes.Black;
            //            dd.Margin = new Thickness(8, -1, 1, 1);
            //            dd.IsEditable = false;
            //            dd.SelectedValuePath = "Score";
            //            dd.DisplayMemberPath = "Value";

            //            dd.SelectionChanged += btnHandler_Click;

            //            dd.Name = QAGetObjectNameScore(loopCtr);
            //            this.RegisterName(dd.Name, dd);

            //            x.Children.Add(y);
            //            x.Children.Add(dd);

            //            grd.grdData.QAQuestion.drObjContainer["DDNameSel"] = dd.Name;

            //            mdd = new RadComboBox();
            //            mdd.Width = 100;
            //            mdd.Height = 24;
            //            mdd.FontSize = 11;
            //            mdd.HorizontalContentAlignment = HorizontalAlignment.Left;
            //            mdd.VerticalContentAlignment = VerticalAlignment.Center;
            //            mdd.VerticalAlignment = VerticalAlignment.Top;
            //            mdd.Foreground = System.Windows.Media.Brushes.Black;
            //            mdd.Margin = new Thickness(4, -1, 1, 1);
            //            mdd.IsEditable = false;
            //            mdd.SelectedValuePath = "Id";
            //            mdd.DisplayMemberPath = "Value";

            //            mdd.SelectionChanged += btnHandler_Click;

            //            mdd.Name = QAGetObjectNameMd(loopMd);
            //            this.RegisterName(mdd.Name, mdd);

            //            x.Children.Add(mdd);

            //            grd.grdData.QAQuestion.drObjContainer["DDNameMD"] = mdd.Name;

            //            tb = new RadWatermarkTextBox();
            //            tb.Text = "Put a remarks here..."; /*dt.Rows[r]["Description"].ToString();*/
            //            tb.Width = 250;
            //            tb.Height = 24;
            //            tb.FontSize = 11;
            //            tb.HorizontalContentAlignment = HorizontalAlignment.Left;
            //            tb.VerticalContentAlignment = VerticalAlignment.Center;
            //            tb.VerticalAlignment = VerticalAlignment.Top;
            //            tb.Foreground = System.Windows.Media.Brushes.Black;
            //            tb.Margin = new Thickness(4, -1, 1, 1);

            //            tb.Name = QAGetObjectNameRem(loopTb);
            //            this.RegisterName(tb.Name, tb);

            //            x.Children.Add(tb);

            //            grd.grdData.QAQuestion.drObjContainer["Remarks"] = tb.Name;

            //            this.QAWrapPanel.Children.Add(x);

            //            DataRow[] res = grd.grdData.QAQuestion.dtQASelection.Select("FormId = '" + dt.Rows[k]["QID"] + "'");
            //            foreach (DataRow row in res)
            //            {
            //                dd.Items.Add(new { Value = row["Value"], Score = row["Score"] });
            //            }

            //            DataRow[] resMd = grd.grdData.QAQuestion.dtQAMarkdownSelection.Select("QID = '" + grd.grdData.QAQuestion.QID + "'");
            //            foreach (DataRow row in resMd)
            //            {
            //                mdd.Items.Add(new { Value = row["Value"], Id = row["Id"] });
            //            }

            //            grd.grdData.QAQuestion.dtObjContainer.Rows.Add(grd.grdData.QAQuestion.drObjContainer);
            //            grd.grdData.QAQuestion.dtObjContainer.AcceptChanges();

            //            loopCtr = loopCtr + 1;
            //            loopMd = loopMd + 1;
            //            loopTb = loopTb + 1;
            //        }
            //    }

            //    var l = new TextBlock();
            //    l.Text = "Description: " + dt.Rows[k]["Description"].ToString();
            //    l.TextWrapping = TextWrapping.Wrap;
            //    l.Width = 400;
            //    l.Height = 24;
            //    l.FontSize = 11;
            //    l.FontStyle = FontStyles.Italic;
            //    //l.FontWeight = FontWeights.Bold;
            //    l.HorizontalAlignment = HorizontalAlignment.Left;
            //    l.Foreground = System.Windows.Media.Brushes.White;
            //    l.Margin = new Thickness(6, -1, 1, 1);
            //    l.VerticalAlignment = VerticalAlignment.Top;

            //    this.QAWrapPanel.Children.Add(l);


            //}

            //if (_ConfigCtr > 0)
            //{
            //    xx = new WrapPanel();
            //    xx.Background = System.Windows.Media.Brushes.Transparent;
            //    xx.VerticalAlignment = VerticalAlignment.Top;
            //    xx.HorizontalAlignment = HorizontalAlignment.Left;
            //    xx.Margin = new Thickness(4, 6, 1, 1);
            //    xx.Orientation = Orientation.Vertical;
            //}


        }

        private bool QADynamicObjectsHandler()
        {
            bool temp = false;

            if (_ConfigCtr > 0)
            {
                temp = true;

                dt = new DataTable();

                dt.Columns.Add("QID");
                dt.Columns.Add("Question");
                dt.Columns.Add("Value");
                dt.Columns.Add("Score");
                dt.Columns.Add("Markdown");
                dt.Columns.Add("Remarks");

                int _Score = 0;

                foreach (DataRow row in grd.grdData.QAQuestion.dtObjContainer.Rows)
                {
                    DataRow[] fQ = dt.Select("Question = '" + row["Question"].ToString() + "'");
                    RadComboBox cbValue = (RadComboBox)this.FindName(row["DDNameSel"].ToString());
                    RadComboBox cbMd = (RadComboBox)this.FindName(row["DDNameMD"].ToString());
                    RadWatermarkTextBox tbRemarks = (RadWatermarkTextBox)this.FindName(row["Remarks"].ToString());

                    if (fQ.Length == 0)
                    {
                        dr = dt.NewRow();
                        dr["QID"] = Convert.ToInt32(row["QID"]);
                        dr["Question"] = row["Question"].ToString();
                        dr["Value"] = cbValue.Text;
                        dr["Score"] = Convert.ToInt32(cbValue.SelectedValue);
                        dr["Markdown"] = cbMd.Text;
                        dr["Remarks"] = tbRemarks.Text;
                        dt.Rows.Add(dr);
                        dt.AcceptChanges();
                    }

                    _Score = _Score + Convert.ToInt32(cbValue.SelectedValue);
                }

                switch (grd.grdData.QAQuestion.Formula.ToUpper())
                {
                    case "SUM":
                        {
                            if (_Score <= 99)
                                grd.grdData.QAQuestion.ScoreMarkings = "FAILED!";
                            else
                                grd.grdData.QAQuestion.ScoreMarkings = "PASSED!";

                            _ScoreAverage = _Score;
                            break;
                        }
                    case "AVERAGE":
                        {
                            _ScoreAverage = _Score / _ConfigCtr;

                            if (_ScoreAverage <= 99)
                                grd.grdData.QAQuestion.ScoreMarkings = "FAILED!";
                            else
                                grd.grdData.QAQuestion.ScoreMarkings = "PASSED!";

                            break;
                        }
                }

                this.xx.Children.Clear();
                this.QAWrapPanel.Children.Remove(xx);

                grd.grdData.QAQuestion.ScoreAverage = _ScoreAverage;

                var s = new TextBlock();
                if (_ScoreAverage == 0)
                    s.Text = "SCORE : ";
                else
                    s.Text = "SCORE : " + grd.grdData.QAQuestion.ScoreAverage;
                //s.Text = "SCORE: " + _ScoreAverage + " out of (" + grd.grdData.QAQuestion.Target + " Target Score)";
                s.TextWrapping = TextWrapping.Wrap;
                s.Width = 400;
                s.Height = 24;
                s.FontSize = 13;
                s.FontWeight = FontWeights.Bold;
                s.HorizontalAlignment = HorizontalAlignment.Left;
                s.Foreground = System.Windows.Media.Brushes.White;
                s.Margin = new Thickness(4, 2, 0, 0);
                s.VerticalAlignment = VerticalAlignment.Top;

                var s1 = new TextBlock();
                if (_ScoreAverage == 0)
                    s1.Text = "MARKING : ";
                else
                    s1.Text = "MARKING : " + grd.grdData.QAQuestion.ScoreMarkings;
                //s1.Text = "Remarks: " + grd.grdData.QAQuestion.ScoreRemarks + " (Formula: " + grd.grdData.QAQuestion.Formula + ")";

                s1.TextWrapping = TextWrapping.Wrap;
                s1.Width = 400;
                s1.Height = 24;
                s1.FontSize = 13;
                s1.FontWeight = FontWeights.Bold;
                s1.HorizontalAlignment = HorizontalAlignment.Left;
                s1.Foreground = System.Windows.Media.Brushes.White;
                s1.Margin = new Thickness(4, -6, 0, 0);
                s1.VerticalAlignment = VerticalAlignment.Top;


                xx.Children.Add(s);
                xx.Children.Add(s1);

                this.QAWrapPanel.Children.Add(xx);

            }
            else
            {
                temp = false;
            }

            return temp;

        }

        private void SetupDynamicConfigInfo()
        {

            #region "Clean"
            this.WrapPanelMain.Children.Clear();
            this.WrapPanelMain2.Children.Clear();
            try
            {
                for (int i = 1; i <= 50; i++)
                {

                    object tempObj = this.FindName("objRF" + Strings.Format(i, "00"));
                    if (!(tempObj == null))
                    {
                        this.UnregisterName("objRF" + Strings.Format(i, "00"));
                    }

                    object tempObj2 = this.FindName("objRF" + Strings.Format(i, "00") + "2");
                    if (!(tempObj2 == null))
                    {
                        this.UnregisterName("objRF" + Strings.Format(i, "00") + "2");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #endregion

            int loopCtr = 0;


            for (int i = 0, loopTo = grd.grdData.CurrentActivity.Activity.ConfigInfo.Count - 1; i <= loopTo; i++)
            {
                loopCtr = loopCtr + 1;

                var x = new WrapPanel();
                x.Background = System.Windows.Media.Brushes.Transparent;
                x.Height = 26;
                x.Width = 380;
                x.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                x.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                x.Margin = new Thickness(0, 1, 0, 0);
                x.Orientation = System.Windows.Controls.Orientation.Horizontal;

                var y = new System.Windows.Controls.Label();
                y.Content = grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName; //grd.grdData.QALists[i].Question;

                try
                {
                    if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].Desc != "")
                    {
                        if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].Desc.Length > 3)
                        {
                            y.ToolTip = grd.grdData.CurrentActivity.Activity.ConfigInfo[i].Desc;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                y.Width = 110;
                y.Height = 25;
                y.FontSize = 11;
                y.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                y.Foreground = System.Windows.Media.Brushes.White;
                y.Margin = new Thickness(0, 0, 0, 0);
                y.VerticalAlignment = System.Windows.VerticalAlignment.Center;



                switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].ObjectType.ToUpper())
                {
                    #region "Textbox"
                    case "TEXTBOX":

                        var z = new Telerik.Windows.Controls.RadWatermarkTextBox(); // System.Windows.Controls.TextBox

                        z.Width = 200;
                        z.Height = 25;
                        z.FontSize = 11;
                        z.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        z.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        z.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        z.Foreground = System.Windows.Media.Brushes.Black;
                        z.Background = System.Windows.Media.Brushes.White;



                        StyleManager.SetTheme(z, new Expression_DarkTheme());

                        z.Margin = new Thickness(0, 1, 1, 1);

                        z.Name = GetObjectName(i);
                        this.RegisterName(z.Name, z);


                        if (grd.grdData.CurrentActivity.Activity.DailyScorecard == true)
                        {
                            y.Width = 225;
                            y.BorderThickness = new Thickness(0.5d, 0.5d, 0.5d, 0.5d);
                            y.BorderBrush = System.Windows.Media.Brushes.White;
                            y.Height = 22;
                            y.Margin = new Thickness(0, 1, 3, 1);
                            // y.Padding = New Thickness(1)
                            y.FontSize = 10;
                            y.Content = "  " + y.Content;

                            z.Width = 70;
                            z.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

                        }




                        x.Children.Add(y);
                        x.Children.Add(z);


                        break;
                    #endregion

                    #region "DropDown"
                    case "DROPDOWN":
                        var dd = new Telerik.Windows.Controls.RadComboBox();

                        dd.Width = 200;
                        dd.Height = 25;
                        dd.FontSize = 11;
                        dd.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        dd.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        dd.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        dd.Foreground = System.Windows.Media.Brushes.Black;

                        dd.Margin = new Thickness(0, 1, 1, 1);
                        dd.IsEditable = true;

                        dd.Name = GetObjectName(i);
                        this.RegisterName(dd.Name, dd);


                        x.Children.Add(y);
                        x.Children.Add(dd);

                        break;
                    #endregion

                    #region "Selectionlist"
                    case "SELECTIONLIST":
                        var sl = new System.Windows.Controls.ComboBox();

                        sl.Width = 200;
                        sl.Height = 25;
                        sl.FontSize = 11;
                        sl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        sl.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        sl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        sl.Foreground = System.Windows.Media.Brushes.Black;
                        sl.Background = System.Windows.Media.Brushes.White;
                        sl.Margin = new Thickness(0, 1, 1, 1);
                        sl.IsEditable = false;
                        sl.Name = GetObjectName(i);

                        this.RegisterName(sl.Name, sl);


                        x.Children.Add(y);
                        x.Children.Add(sl);
                        break;
                    #endregion

                    #region "DatePicker"
                    case "DATEPICKER":
                        var dp = new RadDatePicker();

                        // Dim z As New RadDateTimePicker
                        // z.InputMode = InputMode.TimePicker

                        dp.Width = 100;
                        dp.Height = 25;
                        dp.FontSize = 11;
                        dp.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        dp.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        dp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        dp.Foreground = System.Windows.Media.Brushes.Black;
                        dp.Margin = new Thickness(0, 1, 1, 1);
                        dp.Name = GetObjectName(i);

                        try
                        {
                            if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].UseCurrentDate == true)
                            {
                                dp.SelectedDate = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
                            }
                        }
                        catch (Exception ex)
                        {

                        }



                        this.RegisterName(dp.Name, dp);

                        x.Children.Add(y);
                        x.Children.Add(dp);
                        break;
                    #endregion

                    #region "TimePicker"
                    case "TIMEPICKER":
                        var rdtp = new RadDateTimePicker();
                        rdtp.InputMode = Telerik.Windows.Controls.InputMode.TimePicker;

                        rdtp.Width = 100;
                        rdtp.Height = 25;
                        rdtp.FontSize = 11;
                        rdtp.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        rdtp.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        rdtp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        rdtp.Foreground = System.Windows.Media.Brushes.Black;
                        rdtp.Margin = new Thickness(0, 1, 1, 1);
                        rdtp.Name = GetObjectName(i);

                        try
                        {
                            if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].UseCurrentDate == true)
                            {
                                rdtp.SelectedDate = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        this.RegisterName(rdtp.Name, rdtp);

                        x.Children.Add(y);
                        x.Children.Add(rdtp);
                        break;
                    #endregion

                    #region "DateTimePicker"
                    case "DATETIMEPICKER":
                        var dtpkr = new RadDateTimePicker();
                        dtpkr.InputMode = Telerik.Windows.Controls.InputMode.DateTimePicker;

                        dtpkr.Width = 100;
                        dtpkr.Height = 25;
                        dtpkr.FontSize = 11;
                        dtpkr.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        dtpkr.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        dtpkr.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        dtpkr.Foreground = System.Windows.Media.Brushes.Black;
                        dtpkr.Margin = new Thickness(0, 1, 1, 1);
                        dtpkr.Name = GetObjectName(i);

                        try
                        {
                            if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].UseCurrentDate == true)
                            {
                                dtpkr.SelectedDate = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        this.RegisterName(dtpkr.Name, dtpkr);

                        x.Children.Add(y);
                        x.Children.Add(dtpkr);
                        break;
                    #endregion

                    #region "Yes/No"
                    case "YES/NO":
                        var cb = new System.Windows.Controls.ComboBox();

                        cb.Width = 200;
                        cb.Height = 25;
                        cb.FontSize = 11;
                        cb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        cb.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        cb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        cb.Foreground = System.Windows.Media.Brushes.Black;
                        cb.Margin = new Thickness(0, 1, 1, 1);
                        cb.IsEditable = false;
                        cb.Name = GetObjectName(i);
                        this.RegisterName(cb.Name, cb);

                        cb.Items.Add("Yes");
                        cb.Items.Add("No");


                        if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                        {
                            if (cb.Items.Count > 0)
                            {
                                cb.SelectedIndex = 0;
                            }
                        }

                        x.Children.Add(y);
                        x.Children.Add(cb);
                        break;
                    #endregion

                    #region "True/False"
                    case "TRUE/FALSE":
                        var cbtf = new System.Windows.Controls.ComboBox();

                        cbtf.Width = 200;
                        cbtf.Height = 25;
                        cbtf.FontSize = 11;
                        cbtf.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        cbtf.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                        cbtf.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        cbtf.Foreground = System.Windows.Media.Brushes.Black;
                        cbtf.Margin = new Thickness(0, 1, 1, 1);
                        cbtf.IsEditable = false;
                        cbtf.Name = GetObjectName(i);
                        this.RegisterName(cbtf.Name, cbtf);

                        cbtf.Items.Add("True");
                        cbtf.Items.Add("False");

                        if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                        {
                            if (cbtf.Items.Count > 0)
                            {
                                cbtf.SelectedIndex = 0;
                            }
                        }

                        x.Children.Add(y);
                        x.Children.Add(cbtf);
                        break;
                    #endregion

                    #region "Multiline"
                    case "MULTILINE":
                        //var rt = new Telerik.Windows.Controls.RadRichTextBox(); RichTextBox
                        var rt = new System.Windows.Controls.RichTextBox();
                        rt.Width = 200;
                        rt.Height = 60;
                        rt.FontSize = 10;

                        rt.Document.LineHeight = 1;
                        rt.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                        rt.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        rt.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                        rt.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        rt.Foreground = System.Windows.Media.Brushes.Black;
                        rt.Background = System.Windows.Media.Brushes.White;
                        rt.Margin = new Thickness(0, 1, 1, 1);

                        rt.Name = GetObjectName(i);
                        this.RegisterName(rt.Name, rt);

                        //rt.TextChanged += RichTexbox_LostFocus;

                        x.Children.Add(y);
                        x.Children.Add(rt);
                        x.Height = 60;
                        break;
                        #endregion
                }




                if (loopCtr < 12)
                    this.WrapPanelMain.Children.Add(x);
                else
                    this.WrapPanelMain2.Children.Add(x);

            }
        }

        private gridPerfInfo GetDynamicConfigData()
        {

            grd.grdData.CurrentPerfInfo = new gridPerfInfo();
            grd.grdData.CurrentPerfInfo.Count = 0;

            for (int i = 0, loopTo = grd.grdData.CurrentActivity.Activity.ConfigInfo.Count - 1; i <= loopTo; i++)
            {
                grd.grdData.CurrentPerfInfo.Count = grd.grdData.CurrentActivity.Activity.ConfigInfo.Count;
                switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].ObjectType)
                {

                    case "Textbox":
                        {

                            System.Windows.Controls.TextBox tb = (TextBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, tb.Text);
                                }
                            }

                            break;
                        }


                    case "Dropdown":
                        {

                            RadComboBox tb = (RadComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                }

                                else
                                {
                                    PassValueToRF(i, tb.Text);
                                }
                            }

                            break;
                        }

                    case "Selectionlist":
                        {

                            ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, tb.Text);
                                }
                            }

                            break;
                        }

                    case "DatePicker":
                        {


                            RadDatePicker tb = (RadDatePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (!Information.IsDate(tb.SelectedDate))
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, Strings.Format(tb.SelectedDate, "MM/dd/yyyy"));
                                }
                            }

                            break;
                        }


                    case "TimePicker":
                        {

                            var tb = new RadDateTimePicker();

                            try
                            {
                                tb = (RadDateTimePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!(tb == null))
                            {
                                if (!Information.IsDate(tb.SelectedDate))
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, Strings.Format(tb.SelectedDate, "hh:mm:ss AM/PM"));
                                }
                            }

                            break;
                        }

                    case "DateTimePicker":
                        {

                            var tb = new RadDateTimePicker();

                            try
                            {
                                tb = (RadDateTimePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!(tb == null))
                            {
                                if (!Information.IsDate(tb.SelectedDate))
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, Strings.Format(tb.SelectedDate, "MM/dd/yyyy hh:mm:ss AM/PM"));
                                }
                            }

                            break;
                        }

                    case "Yes/No":
                        {

                            System.Windows.Controls.ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));

                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, tb.Text);
                                }
                            }

                            break;
                        }

                    case "True/False":
                        {

                            System.Windows.Controls.ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                }
                                else
                                {
                                    PassValueToRF(i, tb.Text);
                                }
                            }

                            break;
                        }

                    case "Multiline":
                        {

                            RichTextBox tb = (RichTextBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));

                            if (!(tb == null))
                            {


                                string richText = null;

                                try
                                {
                                    richText = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd).Text;


                                    if (richText.EndsWith(Constants.vbCrLf))
                                    {
                                        var oTrim = new char[] { Conversions.ToChar(Constants.vbCr), Conversions.ToChar(Constants.vbLf) };
                                        richText = richText.TrimEnd(oTrim);
                                    }


                                    if (richText.StartsWith(Constants.vbCrLf))
                                    {
                                        var oTrim = new char[] { Conversions.ToChar(Constants.vbCr), Conversions.ToChar(Constants.vbLf) };
                                        richText = richText.TrimStart(oTrim);
                                    }
                                }


                                catch (Exception ex)
                                {

                                }

                                if (string.IsNullOrEmpty(richText) | richText == null)
                                {
                                }

                                else
                                {
                                    PassValueToRF(i, richText);
                                }


                            }

                            break;
                        }

                }

            }

            return grd.grdData.CurrentPerfInfo;

        }

        private bool CheckDynamicConfigData()
        {




            for (int i = 0, loopTo1 = grd.grdData.CurrentActivity.Activity.ConfigInfo.Count - 1; i <= loopTo1; i++)
            {


                switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].ObjectType.ToUpper())
                {

                    case var @case when @case == "Textbox".ToUpper():
                        {

                            System.Windows.Controls.TextBox tb = (TextBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {



                                if (tb.Text == "" | tb.Text == null)
                                {

                                    if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }


                                else
                                {

                                    switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].DataType.ToUpper())
                                    {


                                        case "NUMBER":
                                            {

                                                if (!Information.IsNumeric(tb.Text))
                                                {
                                                    new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                    //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                    return false;
                                                }

                                                break;
                                            }

                                        case "DATE":
                                            {

                                                if (!Information.IsDate(tb.Text))
                                                {
                                                    new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                    //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                    return false;
                                                }

                                                break;
                                            }


                                    }


                                }


                            }

                            break;
                        }

                    case var case1 when case1 == "DROPDOWN".ToUpper():
                        {
                            ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {

                                    if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }


                                else
                                {

                                    switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].DataType.ToUpper())
                                    {


                                        case "NUMBER":
                                            {

                                                if (!Information.IsNumeric(tb.Text))
                                                {
                                                    new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                    //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                    return false;
                                                }

                                                break;
                                            }

                                        case "DATE":
                                            {

                                                if (!Information.IsDate(tb.Text))
                                                {
                                                    new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                    //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                    return false;
                                                }

                                                break;
                                            }


                                    }


                                }
                            }

                            break;
                        }

                    case var case2 when case2 == "Selectionlist".ToUpper():
                        {

                            System.Windows.Controls.ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (tb.Text == "" | tb.Text == null)
                                {
                                    if (tb.Text == "" | tb.Text == null)
                                    {

                                        if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                        {
                                            new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                            //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                            return false;
                                        }
                                    }


                                    else
                                    {

                                        switch (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].DataType.ToUpper())
                                        {


                                            case "NUMBER":
                                                {

                                                    if (!Information.IsNumeric(tb.Text))
                                                    {
                                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                        return false;
                                                    }

                                                    break;
                                                }

                                            case "DATE":
                                                {

                                                    if (!Information.IsDate(tb.Text))
                                                    {
                                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + ": Invalid data type.", Constants.vbExclamation, grd.AppName);
                                                        return false;
                                                    }

                                                    break;
                                                }


                                        }


                                    }
                                }
                            }

                            break;
                        }

                    case var case3 when case3 == "DatePicker".ToUpper():
                        {

                            DatePicker tb = (DatePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {

                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {
                                    if (!Information.IsDate(tb.SelectedDate))
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                    case var case4 when case4 == "TimePicker".ToUpper():
                        {

                            var tb = new RadDateTimePicker();

                            try
                            {
                                tb = (RadDateTimePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!(tb == null))
                            {

                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {
                                    if (!Information.IsDate(tb.SelectedDate))
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                    case var case5 when case5 == "DateTimePicker".ToUpper():
                        {

                            var tb = new RadDateTimePicker();

                            try
                            {
                                tb = (RadDateTimePicker)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!(tb == null))
                            {

                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {
                                    if (!Information.IsDate(tb.SelectedDate))
                                    {

                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        // Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                    case var case6 when case6 == "YES/NO".ToUpper():
                        {

                            System.Windows.Controls.ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {
                                    if (tb.Text == "" | tb.Text == null)
                                    {

                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                    case var case7 when case7 == "True/False".ToUpper():
                        {

                            System.Windows.Controls.ComboBox tb = (ComboBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {
                                    if (tb.Text == "" | tb.Text == null)
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                    case var case8 when case8 == "Multiline".ToUpper():
                        {

                            System.Windows.Controls.RichTextBox tb = (RichTextBox)this.FindName("objRF" + Strings.Format(Operators.AddObject(i, 1), "00"));
                            if (!(tb == null))
                            {
                                if (grd.grdData.CurrentActivity.Activity.ConfigInfo[i].IsRequired == true)
                                {

                                    string richText = null;

                                    try
                                    {
                                        richText = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd).Text;
                                    }
                                    catch (Exception ex)
                                    {

                                    }


                                    if (string.IsNullOrEmpty(richText) | richText == null)
                                    {
                                        new MessagesBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                                        //Interaction.MsgBox(grd.grdData.CurrentActivity.Activity.ConfigInfo[i].FieldName + " is required.", Constants.vbExclamation, grd.AppName);
                                        return false;
                                    }
                                }

                            }

                            break;
                        }

                }


            }



            return true;

        }

        private void PassValueToRF(int ctr, string _Value)
        {

            switch (ctr + 1)
            {

                case 1:
                    {
                        grd.grdData.CurrentPerfInfo.RF01 = _Value;
                        break;
                    }
                case 2:
                    {
                        grd.grdData.CurrentPerfInfo.RF02 = _Value;
                        break;
                    }
                case 3:
                    {
                        grd.grdData.CurrentPerfInfo.RF03 = _Value;
                        break;
                    }
                case 4:
                    {
                        grd.grdData.CurrentPerfInfo.RF04 = _Value;
                        break;
                    }
                case 5:
                    {
                        grd.grdData.CurrentPerfInfo.RF05 = _Value;
                        break;
                    }
                case 6:
                    {
                        grd.grdData.CurrentPerfInfo.RF06 = _Value;
                        break;
                    }
                case 7:
                    {
                        grd.grdData.CurrentPerfInfo.RF07 = _Value;
                        break;
                    }
                case 8:
                    {
                        grd.grdData.CurrentPerfInfo.RF08 = _Value;
                        break;
                    }
                case 9:
                    {
                        grd.grdData.CurrentPerfInfo.RF09 = _Value;
                        break;
                    }
                case 10:
                    {
                        grd.grdData.CurrentPerfInfo.RF10 = _Value;
                        break;
                    }

                case 11:
                    {
                        grd.grdData.CurrentPerfInfo.RF11 = _Value;
                        break;
                    }
                case 12:
                    {
                        grd.grdData.CurrentPerfInfo.RF12 = _Value;
                        break;
                    }
                case 13:
                    {
                        grd.grdData.CurrentPerfInfo.RF13 = _Value;
                        break;
                    }
                case 14:
                    {
                        grd.grdData.CurrentPerfInfo.RF14 = _Value;
                        break;
                    }
                case 15:
                    {
                        grd.grdData.CurrentPerfInfo.RF15 = _Value;
                        break;
                    }
                case 16:
                    {
                        grd.grdData.CurrentPerfInfo.RF16 = _Value;
                        break;
                    }
                case 17:
                    {
                        grd.grdData.CurrentPerfInfo.RF17 = _Value;
                        break;
                    }
                case 18:
                    {
                        grd.grdData.CurrentPerfInfo.RF18 = _Value;
                        break;
                    }
                case 19:
                    {
                        grd.grdData.CurrentPerfInfo.RF19 = _Value;
                        break;
                    }
                case 20:
                    {
                        grd.grdData.CurrentPerfInfo.RF20 = _Value;
                        break;
                    }

                case 21:
                    {
                        grd.grdData.CurrentPerfInfo.RF21 = _Value;
                        break;
                    }
                case 22:
                    {
                        grd.grdData.CurrentPerfInfo.RF22 = _Value;
                        break;
                    }
                case 23:
                    {
                        grd.grdData.CurrentPerfInfo.RF23 = _Value;
                        break;
                    }
                case 24:
                    {
                        grd.grdData.CurrentPerfInfo.RF24 = _Value;
                        break;
                    }
                case 25:
                    {
                        grd.grdData.CurrentPerfInfo.RF25 = _Value;
                        break;
                    }
                case 26:
                    {
                        grd.grdData.CurrentPerfInfo.RF26 = _Value;
                        break;
                    }
                case 27:
                    {
                        grd.grdData.CurrentPerfInfo.RF27 = _Value;
                        break;
                    }
                case 28:
                    {
                        grd.grdData.CurrentPerfInfo.RF28 = _Value;
                        break;
                    }
                case 29:
                    {
                        grd.grdData.CurrentPerfInfo.RF29 = _Value;
                        break;
                    }
                case 30:
                    {
                        grd.grdData.CurrentPerfInfo.RF30 = _Value;
                        break;
                    }

                case 31:
                    {
                        grd.grdData.CurrentPerfInfo.RF31 = _Value;
                        break;
                    }
                case 32:
                    {
                        grd.grdData.CurrentPerfInfo.RF32 = _Value;
                        break;
                    }
                case 33:
                    {
                        grd.grdData.CurrentPerfInfo.RF33 = _Value;
                        break;
                    }
                case 34:
                    {
                        grd.grdData.CurrentPerfInfo.RF34 = _Value;
                        break;
                    }
                case 35:
                    {
                        grd.grdData.CurrentPerfInfo.RF35 = _Value;
                        break;
                    }
                case 36:
                    {
                        grd.grdData.CurrentPerfInfo.RF36 = _Value;
                        break;
                    }
                case 37:
                    {
                        grd.grdData.CurrentPerfInfo.RF37 = _Value;
                        break;
                    }
                case 38:
                    {
                        grd.grdData.CurrentPerfInfo.RF38 = _Value;
                        break;
                    }
                case 39:
                    {
                        grd.grdData.CurrentPerfInfo.RF39 = _Value;
                        break;
                    }
                case 40:
                    {
                        grd.grdData.CurrentPerfInfo.RF40 = _Value;
                        break;
                    }

                case 41:
                    {
                        grd.grdData.CurrentPerfInfo.RF41 = _Value;
                        break;
                    }
                case 42:
                    {
                        grd.grdData.CurrentPerfInfo.RF42 = _Value;
                        break;
                    }
                case 43:
                    {
                        grd.grdData.CurrentPerfInfo.RF43 = _Value;
                        break;
                    }
                case 44:
                    {
                        grd.grdData.CurrentPerfInfo.RF44 = _Value;
                        break;
                    }
                case 45:
                    {
                        grd.grdData.CurrentPerfInfo.RF45 = _Value;
                        break;
                    }
                case 46:
                    {
                        grd.grdData.CurrentPerfInfo.RF46 = _Value;
                        break;
                    }
                case 47:
                    {
                        grd.grdData.CurrentPerfInfo.RF47 = _Value;
                        break;
                    }
                case 48:
                    {
                        grd.grdData.CurrentPerfInfo.RF48 = _Value;
                        break;
                    }
                case 49:
                    {
                        grd.grdData.CurrentPerfInfo.RF49 = _Value;
                        break;
                    }
                case 50:
                    {
                        grd.grdData.CurrentPerfInfo.RF50 = _Value;
                        break;
                    }

            }

        }

        private void RichTexbox_LostFocus(object sender, RoutedEventArgs e)
        {
            //string str = new TextRange(sender.Document.ContentStart, sender.Document.ContentEnd).Text;

            //if (Strings.Len(str) > 1000)
            //{

            //    Interaction.MsgBox("You have exceeded the maximum limit of 1000 char.", Constants.vbCritical, grd.AppName);

            //    sender.Document.Blocks.Clear();
            //    str = Strings.Trim(str);
            //    str = Strings.Mid(str, 1, 950);

            //    sender.Document.Blocks.Add(new Paragraph(new Run(str)));

            //}
        }

        private string GetObjectName(int ctr)
        {
            return "objRF" + Strings.Format(ctr + 1, "00");
        }

        private string QAGetObjectNameScore(int ctr)
        {
            return "objScore" + Strings.Format(ctr + 1, "00");
        }

        private string QAGetObjectNameMd(int ctr)
        {
            return "objMd" + Strings.Format(ctr + 1, "00");
        }

        private string QAGetObjectNameRem(int ctr)
        {
            return "objRemarks" + Strings.Format(ctr + 1, "00");
        }

        #endregion

        #region "QA QUESTION TABS"
        [Obsolete]
        private void lstQAForm_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.StartQAQuestion();

            if (_ConfigCtr == 0)
            {
                new MessagesBox("You've Selected a Form" + Constants.vbNewLine + "With No Configuration.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }

            btnStop.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Visible;
            btnChange.Visibility = Visibility.Visible;

            this.MatDesActivityList.Visibility = Visibility.Collapsed;
        }

        private void btnHandler_Click(object sender, RoutedEventArgs e)
        {
            //notifier.ShowError(objRF01.SelectedValue.ToString());


            //for (int i = grd.grdData.QAQuestion.dtObjContainer.Rows.Count - 1; i >= 0; i--)
            //{
            //    DataRow dr = grd.grdData.QAQuestion.dtObjContainer.Rows[i];
            //    if (dr["Name"] == row1["Name"].ToString())
            //        dr.Delete();
            //}
            //grd.grdData.QAQuestion.dtObjContainer.AcceptChanges();

            QADynamicObjectsHandler();
        }

        private void btnEmpNo_Click(object sender, RoutedEventArgs e)
        {
            FrmChipUsers aa = new FrmChipUsers();
            aa.ShowDialog();

            txtAgentName.Text = grd.grdData.QAQuestion.EmpName;
            txtWorkgroup.Focus();

        }

        private void btnUploadSS_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog() { Multiselect = true };
            OpenFileDialog.Filter = "JPG Files (*.jpg)|*.jpg| PNG Files (*.png)| All Files (*.*)";
            OpenFileDialog.Title = "Browse Screenshot";

            bool? response = OpenFileDialog.ShowDialog();

            if (response == true)
            {
                string[] files = OpenFileDialog.FileNames;


                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);
                    FileInfo fileInfo = new FileInfo(files[i]);

                    UploadFileList.Items.Add(new Upload()
                    {
                        //FileName = filename,
                        FileName = fileInfo.ToString(),
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });
                }

                this.AttContorls.Visibility = Visibility.Visible;
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = System.IO.Path.GetFileName(files[0]);



                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = System.IO.Path.GetFileName(files[i]);
                    FileInfo fileInfo = new FileInfo(files[i]);

                    UploadFileList.Items.Add(new Upload()
                    {
                        //FileName = filename,
                        FileName = fileInfo.ToString(),
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });
                }

                this.AttContorls.Visibility = Visibility.Visible;
            }
        }

        private void btnCancelAtt_Click(object sender, RoutedEventArgs e)
        {
            if (UploadFileList.Items.Count > 0)
            {
                bool? Result = new MessagesBox("Remove Attachments?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (Result.Value)
                {
                    grd.grdData._lstQAAttachments.Clear();
                    UploadFileList.Items.Clear();
                    this.AttContorls.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                new MessagesBox("No Attachment(s) to remove.", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }
        }

        [Obsolete]
        private void btnSaveAtt_Click(object sender, RoutedEventArgs e)
        {
            if (UploadFileList.Items.Count > 0)
            {
                for (int i = 0, loopTo = UploadFileList.Items.Count - 1; i <= loopTo; i++)
                {
                    var obj = UploadFileList.Items[i];
                    var FileName = Convert.ToString(obj.GetType().GetProperties().First(o => o.Name == "FileName").GetValue(obj, null));



                    grd.grdData._lstQAAttachments.Add(new QAQuestionForm { SSId = i, SSFileName = FileName.ToString() });
                }

                bool? Result = new MessagesBox("This will save the uploaded Screenshots(" + UploadFileList.Items.Count + ")." + Constants.vbNewLine + "Are all Attachments correct?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (Result.Value)
                {
                    for (int i = 0, loopTo = grd.grdData._lstQAAttachments.Count - 1; i <= loopTo; i++)
                    {
                        FileInfo fileInfo = new FileInfo(grd.grdData._lstQAAttachments[i].SSFileName);
                        byte[] ss = null;
                        FileStream fs = new FileStream(fileInfo.ToString(), FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);

                        ss = br.ReadBytes((int)fs.Length);
                        br.Close();
                        fs.Close();

                        this.grd.SaveQAAttachments(grd.grdData.QAQuestion.ItemId, ss);
                    }

                    this.AttContorls.Visibility = Visibility.Collapsed;
                    grd.grdData._lstQAAttachments.Clear();
                    UploadFileList.Items.Clear();

                    notifier.ShowSuccess("Attactments Saved.");
                }
            }
            else
            {
                new MessagesBox("No Screenshot(s) to attach.", MessageType.Error, MessageButtons.Ok).ShowDialog();
            }


        }

        private void cmbQAForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetupQADynamicConfigInfo();
            if (QADynamicObjectsHandler())
            {
                this.QAWrapPanel.Visibility = Visibility.Visible;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (cmbQADate == null || txtAgentName.Text == "" || txtWorkgroup.Text == "" || txtCustomer.Text == "" || txtRecordId.Text == "" || cmbChannel.Text == "" || cmbEvaluationType.Text == "")
                new MessagesBox("One or more fields were empty." + Constants.vbNewLine + "Please Check.", MessageType.Error, MessageButtons.Ok).ShowDialog();
            else
            {
                this.grd.SaveQAItemsSCOPE_IDENTITY(grd.grdData.QAQuestion.QADate, grd.grdData.QAQuestion.UserId, txtWorkgroup.Text, txtCustomer.Text,txtRecordId.Text,cmbChannel.Text,cmbEvaluationType.Text); //-- Getting ItemId

                this.TabQuestion.IsSelected = true;
                this.TabQuestion.IsEnabled = true;
                this.TabSS.IsEnabled = true;
                this.GroupQAItem.IsEnabled = false;
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ClearItemFields();
        }

        public void ClearItemFields()
        {
            cmbQAForm.SelectedItem = null;
            
            cmbQADate.SelectedDate = null;
            txtAgentName.Text = "";
            txtWorkgroup.Text = "";
            txtCustomer.Text = "";
            txtRecordId.Text = "";
            cmbChannel.Text = "";
            cmbEvaluationType.Text = "";
        }

        [Obsolete]
        public void StartQAQuestion()
        {
            grd.grdData.MainWindowAction = "";

            this.ClearWrapPanel();

            idleCtr = 0;

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
                catch (Exception ex)
                {
                }

                btnStop.Visibility = Visibility.Visible;
                btnPause.Visibility = Visibility.Visible;
                btnChange.Visibility = Visibility.Visible;

            }

            grd.grdData.CurrentActivity.Id = newPerf.Id;

            MainScrn.lblActivityName.Content = grd.grdData.CurrentActivity.Activity.ActName;
            MainScrn.lblAHT.Content = grd.grdData.CurrentActivity.Activity.AHT;
            MainScrn.lblProcessName.Content = grd.grdData.CurrentActivity.Activity.Process;
            MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            MainScrn.MinuteTimer = 0;
            idleCtr = 0;

            notifier.ShowInformation("  Your Activity Started @ " + this.MainScrn.lblStartTime.Content);

        }

        private void StopQAActivity()
        {
            if (grd.grdData.QAQuestion.dtObjContainer.Rows.Count == _ConfigCtr)
            {
                dt = new DataTable();

                dt.Columns.Add("QID");
                dt.Columns.Add("Category");
                dt.Columns.Add("Question");
                dt.Columns.Add("Value");
                dt.Columns.Add("Score");
                dt.Columns.Add("Markdown");
                dt.Columns.Add("Remarks");

                int _Score = 0;

                foreach (DataRow row in grd.grdData.QAQuestion.dtObjContainer.Rows)
                {
                    DataRow[] fQ = dt.Select("Question = '" + row["Question"].ToString() + "'");
                    RadComboBox cbValue = (RadComboBox)this.FindName(row["DDNameSel"].ToString());
                    RadComboBox cbMd = (RadComboBox)this.FindName(row["DDNameMD"].ToString());
                    RadWatermarkTextBox tbRemarks = (RadWatermarkTextBox)this.FindName(row["Remarks"].ToString());

                    if (fQ.Length == 0)
                    {
                        dr = dt.NewRow();
                        dr["QID"] = Convert.ToInt32(row["QID"]);
                        dr["Category"] = row["Category"].ToString();
                        dr["Question"] = row["Question"].ToString();
                        dr["Value"] = cbValue.Text;
                        dr["Score"] = Convert.ToInt32(cbValue.SelectedValue);
                        dr["Markdown"] = cbMd.Text;
                        dr["Remarks"] = tbRemarks.Text;
                        dt.Rows.Add(dr);
                        dt.AcceptChanges();
                    }

                    _Score = _Score + Convert.ToInt32(cbValue.SelectedValue);
                }


                if (dt.Rows.Count > 0)
                {
                    string Markdown;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int QID = Convert.ToInt32(dt.Rows[i]["QID"]);
                        string Category = (string)dt.Rows[i]["Category"];
                        string Question = (string)dt.Rows[i]["Question"];
                        int Score = Convert.ToInt32(dt.Rows[i]["Score"]);

                        if (dt.Rows[i]["Markdown"].ToString() == "")
                            Markdown = "NA";
                        else
                        {
                            Markdown = (string)dt.Rows[i]["Markdown"];
                        }

                        string Remarks = (string)dt.Rows[i]["Remarks"];

                        this.grd.SaveQAScores(grd.grdData.QAQuestion.ItemId, (int)cmbQAForm.SelectedValue, cmbQAForm.Text, Category, QID, Question, Score, Markdown, Remarks);
                    }


                    this.grd.grdData.QAQuestion.dtObjContainer.Rows.Clear();
                    this.dt.Clear();
                    this.ClearItemFields();
                    this.ClearWrapPanel();
                    this.ResetValues();

                    this.btnStart.Visibility = Visibility.Collapsed;
                    this.btnStop.Visibility = Visibility.Collapsed;
                    this.btnPause.Visibility = Visibility.Collapsed;
                    this.btnChange.Visibility = Visibility.Collapsed;

                    this.GroupQAItem.IsEnabled = true;
                    this.MatDesGridQA.Visibility = Visibility.Collapsed;
                    this.MatDesActivityList.Visibility = Visibility.Visible;


                    notifier.ShowSuccess("QA Questions Successfully Saved.");

                }


            }
        }

        #endregion

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {
            DateTime timeEndNow = Conversions.ToDate(Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MM/dd/yyyy h:mm:ss tt"));
            //lblCurrentDateTime.Content = Strings.Format(timeEndNow, "MMM. dd - h:mm tt");

            if (grd.grdData.CurrentActivity.Id <= 0)
            {

                this.MainScrn.lblTimeElapsed.Content = "00:00:00";


                grd.grdData.CurrentActivity.TimeStart = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet).ToString();
                grd.grdData.CurrentActivity.TimeEnd = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet).ToString();
                grd.grdData.CurrentActivity.TimeStart2 = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
                grd.grdData.CurrentActivity.TimeEnd2 = Convert.ToDateTime(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet));

                this.MainScrn.BorderTotalTime.Background = System.Windows.Media.Brushes.Transparent;

                this.MainScrn.lblStartTime.Content = "00:00:00";


                return;
            }

            else
            {
                this.MainScrn.lblStartTime.Content = grd.grdData.CurrentActivity.TimeStart2.ToLongTimeString();
            }

            //}




            if (grd.grdData.CurrentActivity.Started == true)
            {

                var curElapsedTime = Conversions.ToDate(this.MainScrn.lblTimeElapsed.Content).AddSeconds(1d);

                this.MainScrn.lblTimeElapsed.Content = curElapsedTime.ToString("HH:mm:ss");

                if (grd.grdData.CurrentActivity.IsPaused == false)
                {
                    this.MainScrn.lblTimeElapsed.Content = grd.MGetTimeElapsed(grd.grdData.CurrentActivity.TimeStart2, timeEndNow, this.MainScrn.lblTimeElapsed.Content.ToString());
                }
                else
                {
                    this.MainScrn.lblTimeElapsed.Content = grd.MGetTimeElapsedPaused(grd.grdData.CurrentActivity.TimeEnd2, timeEndNow, this.MainScrn.lblTimeElapsed.Content.ToString());
                }

                grd.grdData.CurrentActivity.TimeEnd = Strings.Format(timeEndNow, "MM/dd/yyyy h:mm:ss tt");
                grd.grdData.CurrentActivity.TimeEnd2 = timeEndNow;

                //CheckIfOverAHT();
            }

            else
            {
                this.MainScrn.lblTimeElapsed.Content = "00:00:00";
            }



        }

        [Obsolete]
        private void _eMsgBox(params string[] errorMsg)
        {
            string msg = string.Format("Unhandled error has been found. " + "Please contact your system administrator. Error:{0}{0}{1}", Constants.vbNewLine, Strings.Join(errorMsg, ""));
            Interaction.MsgBox(msg, Constants.vbCritical, grd.AppName);
        }

        public void ResetValues()
        {
            grd.grdData.CurrentActivity.Reset();
            MainScrn.lblAHT.Content = "00:00:00";
            MainScrn.lblStartTime.Content = "00:00:00";
            MainScrn.lblActivityName.Content = "";
            MainScrn.lblProcessName.Content = "";
        }

        public void changelblbg()
        {
            MainScrn.BorderTotalTime.Background = System.Windows.Media.Brushes.Transparent;
        }

        public void ClearWrapPanel()
        {
            this.GridDynamicObjects.Visibility = Visibility.Collapsed;

            this.WrapPanelMain.Visibility = Visibility.Collapsed;
            this.WrapPanelMain2.Visibility = Visibility.Collapsed;
            this.WrapPanelMain.Children.Clear();
            this.WrapPanelMain2.Children.Clear();

            this.QAWrapPanel.Visibility = Visibility.Collapsed;
            this.QAWrapPanel.Children.Clear();
        }

        private void StartActivity(int _ctr)
        {
            bool IsActivityStarted = false;
            int LOBItemId = 0;


            if (_ctr == 1)
            {


                if (lvMyActivities.SelectedItem is not null)
                {
                    curAct = (gridActivity)lvMyActivities.SelectedItem;


                    var q = from p in grd.grdData.ActivityList
                            where p.Id == curAct.Id
                            select p.ConfigInfo;

                    if (!(q == null))
                    {
                        if (q.Count() > 0)
                        {
                            curAct.ConfigInfo = q.FirstOrDefault();
                        }
                    }

                    if (curAct.ConfigInfo == null)
                    {
                        //curAct.ConfigInfo = grd.GetPerfConfigListLocal(curAct.LOBId == 79 ? 1594 : curAct.Id);
                    }
                }
            }
            else if (_ctr == 2)
            {

                if (lvProductivity.SelectedItem is not null)
                {
                    curAct = (gridActivity)lvProductivity.SelectedItem;


                    var q = from p in grd.grdData.ActivityList
                            where p.Id == 17
                            select p.ConfigInfo;
                    if (!(q == null))
                    {
                        if (q.Count() > 0)
                        {
                            curAct.ConfigInfo = q.FirstOrDefault();
                        }
                    }


                }
            }


            if (!(curAct == null))
            {

                if (curAct.Id == 17)
                {
                    this.GridDynamicObjects.Visibility = Visibility.Collapsed;
                    this.WrapPanelMain.Visibility = Visibility.Collapsed;
                    this.WrapPanelMain2.Visibility = Visibility.Collapsed;
                    this.QAWrapPanel.Visibility = Visibility.Collapsed;

                    this.MatDesGridQA.Visibility = Visibility.Visible;
                    this.MatDesActivityList.Visibility = Visibility.Collapsed;

                    grd.grdData.CurrentActivity.Started = true;
                    if (grd.grdData.CurrentActivity.Id > 0)
                    {
                        grd.grdData.MainWindowAction = "CHANGE";
                    }
                    else
                    {
                        grd.grdData.MainWindowAction = "START";
                        IsActivityStarted = true;
                    }

                    var withBlock = grd.grdData.CurrentActivity;
                    withBlock.Activity = curAct;
                    withBlock.ActivityId = curAct.Id;
                    withBlock.LOBId = curAct.LOBId;
                    withBlock.LOBItemId = 0;

                    if (IsActivityStarted)
                        this.MainWindowActivated();

                }
                else
                {
                    grd.grdData.CurrentActivity.Started = true;
                    if (grd.grdData.CurrentActivity.Id > 0)
                    {
                        grd.grdData.MainWindowAction = "CHANGE";
                    }
                    else
                    {
                        grd.grdData.MainWindowAction = "START";
                        IsActivityStarted = true;
                    }

                    var withBlock = grd.grdData.CurrentActivity;
                    withBlock.Activity = curAct;
                    withBlock.ActivityId = curAct.Id;
                    withBlock.LOBId = curAct.LOBId;
                    withBlock.LOBItemId = 0;

                    if (IsActivityStarted)
                        this.MainWindowActivated();

                }


            }

            this.TabAcc.IsSelected = true;
            this.TabQuestion.IsEnabled = false;
            this.TabSS.IsEnabled = false;

            btnStop.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Visible;
            btnChange.Visibility = Visibility.Visible;
            grd.grdData.ScrContent.IsActivityRunning = true;
            //grd.grdData.ScrContent.IsMyDataChanged = true;

        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            {
                if (!(grd.grdData.CurrentActivity.Id > 0))
                {
                    new MessagesBox("There is no current activity to Pause.", MessageType.Warning, MessageButtons.Ok).ShowDialog();
                    return;
                }

                try
                {
                    this.PauseActivity(false);

                    MainScrn.cmbCompletedActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 2);
                    MainScrn.cmbCompletedActName.SelectedIndex = 0;
                    MainScrn.cmbOpenActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 1);
                    MainScrn.cmbOpenActName.SelectedIndex = 0;

                    //this.LoadOpenActivity();
                    //this.LoadCompletedActivity();

                    grd.grdData.CurrentActivity.Started = false;

                    this.GridDynamicObjects.Visibility = Visibility.Collapsed;
                    this.WrapPanelMain.Visibility = Visibility.Collapsed;
                    this.WrapPanelMain2.Visibility = Visibility.Collapsed;

                    btnStop.Visibility = Visibility.Collapsed;
                    btnPause.Visibility = Visibility.Collapsed;
                    btnChange.Visibility = Visibility.Collapsed;

                    grd.grdData.ScrContent.IsActivityRunning = false;

                    if (grd.grdData.ScrContent.IsBreakStarted)
                        grd.grdData.ScrContent.IsBreakStarted = false;



                    notifier.ShowWarning("Current activity paused.");

                    new MessagesBox("Current Activity Paused.", MessageType.Info, MessageButtons.Ok).ShowDialog();
                }

                catch (Exception ex)
                {
                    _eMsgBox("clickPauseActivity", ex.Message);
                }
            }
        }

        private void clickStopActivity(object sender, RoutedEventArgs e)
        {
            try
            {
                StopActivity();

                MainScrn.cmbCompletedActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 2);
                MainScrn.cmbCompletedActName.SelectedIndex = 0;
                MainScrn.cmbOpenActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 1);
                MainScrn.cmbOpenActName.SelectedIndex = 0;

                if (grd.grdData.ScrContent.IsBreakStarted)
                    grd.grdData.ScrContent.IsBreakStarted = false;
            }

            catch (Exception ex)
            {
                _eMsgBox("clickStopActivity", ex.Message);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Collapsed;

            this.MatDesActivityList.Visibility = Visibility.Visible;
            this.MatDesGridQA.Visibility = Visibility.Collapsed;
        }

        private void GvProductivity_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.StartActivity(2);
        }

        private void lvMyActivities_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.StartActivity(1);
        }

        private void cmdMyFilter_Click(object sender, RoutedEventArgs e)
        {
            lvMyActivities.ItemsSource = grd.grdData._lstMyActivitiesOrig;
            cmbProcess.SelectedItem = null;
        }

        private void btnDeleteFromFav_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;

            foreach (gridActivity hAct in lvMyActivities.SelectedItems)
            {

                if (grd.DeleteActivityFromFavorites(hAct, Convert.ToInt32(grd.grdData.CurrentUser.EmpNo)))
                {
                    x += 1;
                }

            }
            new MessagesBox(string.Format("{0} Activities Deleted Successfully!", (object)x), MessageType.Info, MessageButtons.Ok).ShowDialog();

            this.PopulateFavoriteList();
        }

        private void PopulateFavoriteList() // 11-13
        {
            lvMyActivities.ItemsSource = null;
            grd.grdData._lstMyActivitiesOrig = grd.GetFavoriteActivities();
            lvMyActivities.ItemsSource = grd.grdData._lstMyActivitiesOrig;

            this.PopulateProcessAndSubProcessMyActivities();
        }

        private void cmbProcessProd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProcessProd.SelectedValue is not null)
            {
                var q = from p in grd.grdData._lstProductivityOrig
                        where p.Process.Equals(cmbProcessProd.SelectedValue.ToString())
                        select p;

                lvProductivity.ItemsSource = q;
            }
        }

        private void btnClearFilterProd_Click(object sender, RoutedEventArgs e)
        {
            lvProductivity.ItemsSource = grd.grdData._lstProductivityOrig;
            cmbProcessProd.SelectedItem = null;
        }

        private void btnProductivityAddtoFav_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            foreach (gridActivity hAct in lvProductivity.SelectedItems)
            {

                hAct.UserId = grd.grdData.CurrentUser.EmpNo;

                if (grd.AddActivityToFavorites(hAct))
                {
                    x += 1;
                }
            }

            if (x == 0)
            {
                new MessagesBox("No activity added. Please check if it's already in the My Favorites.", MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
            else
            {
                new MessagesBox(string.Format("{0} Activities Added Successfully!", (object)x), MessageType.Info, MessageButtons.Ok).ShowDialog();

            }

            this.PopulateFavoriteList();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (grd.grdData.QAQuestion.dtObjContainer.Rows.Count == _ConfigCtr)
            {
                dt = new DataTable();

                dt.Columns.Add("QID");
                dt.Columns.Add("Category");
                dt.Columns.Add("Question");
                dt.Columns.Add("Value");
                dt.Columns.Add("Score");
                dt.Columns.Add("Markdown");
                dt.Columns.Add("Remarks");

                int _Score = 0;

                foreach (DataRow row in grd.grdData.QAQuestion.dtObjContainer.Rows)
                {
                    DataRow[] fQ = dt.Select("Question = '" + row["Question"].ToString() + "'");
                    RadComboBox cbValue = (RadComboBox)this.FindName(row["DDNameSel"].ToString());
                    RadComboBox cbMd = (RadComboBox)this.FindName(row["DDNameMD"].ToString());
                    RadWatermarkTextBox tbRemarks = (RadWatermarkTextBox)this.FindName(row["Remarks"].ToString());

                    if (fQ.Length == 0)
                    {
                        dr = dt.NewRow();
                        dr["QID"] = Convert.ToInt32(row["QID"]);
                        dr["Category"] = row["Category"].ToString();
                        dr["Question"] = row["Question"].ToString();
                        dr["Value"] = cbValue.Text;
                        dr["Score"] = Convert.ToInt32(cbValue.SelectedValue);
                        dr["Markdown"] = cbMd.Text;
                        dr["Remarks"] = tbRemarks.Text;
                        dt.Rows.Add(dr);
                        dt.AcceptChanges();
                    }

                    _Score = _Score + Convert.ToInt32(cbValue.SelectedValue);
                }
             

                if (dt.Rows.Count > 0)
                {
                    string Markdown;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {                       
                        int QID = Convert.ToInt32(dt.Rows[i]["QID"]);
                        string Category = (string)dt.Rows[i]["Category"];
                        string Question = (string)dt.Rows[i]["Question"];
                        int Score = Convert.ToInt32(dt.Rows[i]["Score"]);

                        if (dt.Rows[i]["Markdown"].ToString() == "")
                            Markdown = "NA";
                        else
                        {
                            Markdown = (string)dt.Rows[i]["Markdown"];
                        }
                    
                        string Remarks = (string)dt.Rows[i]["Remarks"];

                        this.grd.SaveQAScores(grd.grdData.QAQuestion.ItemId, (int)cmbQAForm.SelectedValue, cmbQAForm.Text, Category, QID, Question, Score, Markdown, Remarks);
                    }          

                    this.grd.grdData.QAQuestion.dtObjContainer.Rows.Clear();
                    this.dt.Clear();
                    this.ClearItemFields();
                    this.ClearWrapPanel();
                    this.ResetValues();

                    this.btnStart.Visibility = Visibility.Collapsed;
                    this.btnStop.Visibility = Visibility.Collapsed;
                    this.btnPause.Visibility = Visibility.Collapsed;
                    this.btnChange.Visibility = Visibility.Collapsed;

                    this.GroupQAItem.IsEnabled = true;
                    this.MatDesGridQA.Visibility = Visibility.Collapsed;
                    this.MatDesActivityList.Visibility = Visibility.Visible;


                    notifier.ShowSuccess("QA Questions Successfully Saved.");

                }

            
            }
        }

        private void cmbProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProcess.SelectedValue is not null)
            {
                var q = from p in grd.grdData._lstMyActivitiesOrig
                        where p.Process.Equals(cmbProcess.SelectedValue.ToString())
                        select p;

                lvMyActivities.ItemsSource = q;
            }
        }

      
   
    }
}
