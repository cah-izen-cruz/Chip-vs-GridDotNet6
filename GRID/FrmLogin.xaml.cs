using GRID.Themes;
using GRIDLibraries.Libraries;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static GRID.MessagesBox;
using Constants = Microsoft.VisualBasic.Constants;

namespace GRID
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FrmLogin : Window
    {
        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private readonly BackgroundWorker bw_single = new BackgroundWorker();

        //string UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        string HostName = "";
        string UserName = "";
        string Ex = "";
        bool UserInitialized = false;
        private string _installPath = "";
        bool VersionUpdated = true;

        GridLib grd = new GridLib();
        Mutex grdMutexx = new Mutex();


        DateTime LoginTime;
        DateTime ActualLoginTime;
        FileVersionInfo GridMainVersion;

        DateTime ScheduleLogIn = default;
        DateTime ScheduleLogOut = default;

        DateTime cmbTransDate1 = default;
        DateTime cmbTransDate2 = default;

        DispatcherTimer ElapsedTimer;
        int LoadCtr = 0;

        public FrmLogin()
        {
            grd = new GridLib();
            UserName = Environment.UserName; //DESKTOP-A0R75AD //WPEC5009GRDRP01 //WPPHL039SQL01 //WPEC5009GRDRP01 //"izen.cruz"; 

            //ImgUser.ImageSource = new BitmapImage(new Uri(SysInfo.GetUserPicturePath(UserName)));
            //grd.grdData.ScrContent.IsDark = true;

            InitializeComponent();

            Log.Text = "Login to your existing account12";
            this.Height = 360;
            this.Title = grd.grdData.GridVersion;

            if (UserName == "izen.cruz1")
            {
                txtUserName.IsEnabled = true;
            }
            else
            {
                txtUserName.IsEnabled = false;
            }

            txtPassword.IsEnabled = false;

            txtUserName.Text = UserName;
            txtPassword.Password = "***********";

            objBusyIndicator.BusyContent = "Initializing...";
            objBusyIndicator.IsBusy = true;

            //int len = UserName.Substring(0, UserName.IndexOf('\\')).Length;
            //txtUserName.Text = UserName[len..][1..];

            Themes.IsChecked = true;

            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(Theme.Dark);
            ImageBrush b = new ImageBrush();
            //b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
            ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            //grd.grdData.ScrContent.MainBG = b;
            //grd.grdData.ScrContent.TimerStart = true;
            //grd.grdData.ScrContent.IsDark = true;

            paletteHelper.SetTheme(theme);

            bw_single.DoWork += bw_single_DoWork;
            bw_single.RunWorkerCompleted += bw_single_Complete;

            ElapsedTimer = new DispatcherTimer();
            ElapsedTimer.Tick += ElapsedTimer_Tick;
            ElapsedTimer.Interval = new TimeSpan(0, 0, 1);
            ElapsedTimer.Start();

        }

        [Obsolete]
        private void _eMsgBox(params string[] errorMsg)
        {
            string msg = string.Format("Unhandled error has been found. " + "Please contact your system administrator. Error:{0}{0}{1}", Constants.vbNewLine, Strings.Join(errorMsg, ""));
            Interaction.MsgBox(msg, Constants.vbCritical, grd.AppName);
        }

        private void ElapsedTimer_Tick(object sender, EventArgs e) //
        {
            LoadCtr += 1;
            if (LoadCtr == 2)
            {
                ElapsedTimer.Stop();
                bw_single.RunWorkerAsync();
            }
        }

        private void bw_single_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Ex = "";

            this.grdMutexx.WaitOne();
            try
            {
                Ex = Conversions.ToString(grd.CheckMainDbConnection());
            }
            catch (Exception ex)
            {
            }

            if (Ex == "")
            {
                try
                {
                    Ex = grd.InitializeException(UserName);
                }
                catch (Exception ex)
                {
                }
            }

            try
            {

                if (grd.Initialize(UserName))
                    UserInitialized = true;
                else
                    UserInitialized = false;
            }
            catch (Exception ex)
            {
            }

            this.grdMutexx.ReleaseMutex();

        }

        private void bw_single_Complete(object sender, RunWorkerCompletedEventArgs e)
        {

            this.grdMutexx.WaitOne();
            //update ui once worker complete his work
            if (Ex != "True")
            {
                this.Height = 400;
                txtShift.Document.Blocks.Add(new Paragraph(new Run("Failed to initialize user profile.")));
                txtShift.Document.Blocks.Add(new Paragraph(new Run(Ex.ToString())));
                txtShift.Visibility = Visibility.Visible;
                btnLogin.IsEnabled = false;

                objBusyIndicator.IsBusy = false;

                return;
            }

            if (UserInitialized)
            {

                if (grd.grdData.CurrentUser.Status == false)
                {
                    txtShift.Document.Blocks.Add(new Paragraph(new Run("Account is no longer active.")));
                    btnLogin.IsEnabled = false;
                    objBusyIndicator.IsBusy = false;

                    Interaction.MsgBox("Your account is no longer active!", Constants.vbInformation, grd.AppName);

                    return;
                }
            }

            else
            {
                txtShift.Document.Blocks.Add(new Paragraph(new Run("Account not found.")));
                btnLogin.IsEnabled = false;
                objBusyIndicator.IsBusy = false;

                Interaction.MsgBox("Login Failed!" + Constants.vbCrLf + "User not found: " + txtUserName.Text, Constants.vbExclamation, grd.AppName);
                return;
            }

            try
            {

                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation");
                }

                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation\GRID"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation\GRID");
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation\GRID\CSSProofing"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation\GRID\CSSProofing");
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                _installPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Automation\GRID";
            }
            catch (Exception ex)
            {

            }

            if (_installPath != "")
            {
                if (Directory.Exists(_installPath))
                {

                    if (!File.Exists(_installPath + @"\local.mdb"))
                    {

                        byte[] tmpdb;
                        tmpdb = null;
                        try
                        {
                            tmpdb = grd.GetLocalDb();
                        }
                        catch (Exception ex)
                        {

                            txtShift.Document.Blocks.Clear();
                            txtShift.Document.Blocks.Add(new Paragraph(new Run(ex.Message)));
                            btnLogin.IsEnabled = false;

                            objBusyIndicator.IsBusy = false;
                            return;

                        }


                        if (!(tmpdb == null))
                        {
                            try
                            {
                                File.WriteAllBytes(_installPath + @"\local.mdb", tmpdb);
                            }
                            catch (Exception ex)
                            {

                                txtShift.Document.Blocks.Clear();
                                txtShift.Document.Blocks.Add(new Paragraph(new Run(ex.Message)));
                                btnLogin.IsEnabled = false;

                                objBusyIndicator.IsBusy = false;
                                return;

                            }
                        }
                    }
                }

                else
                {
                    txtShift.Document.Blocks.Clear();
                    txtShift.Document.Blocks.Add(new Paragraph(new Run("Cannot locate path/folder: for local db")));
                    btnLogin.IsEnabled = false;
                    objBusyIndicator.IsBusy = false;
                    return;
                }
            }

            else
            {
                txtShift.Document.Blocks.Clear();
                txtShift.Document.Blocks.Add(new Paragraph(new Run("Cannot locate path/folder: for local db")));

                btnLogin.IsEnabled = false;
                objBusyIndicator.IsBusy = false;

                return;
            }

            grd.grdData.ApplicationDirectory = _installPath;

            HostName = Environment.MachineName.Length > 20 ? Strings.Mid(Environment.MachineName, 1, 20) : Environment.MachineName;
            HostName = Strings.Trim(HostName);

            //lblStatus.Visibility = System.Windows.Visibility.Hidden;

            txtShift.Document.Blocks.Clear();

            txtShift.Document.LineHeight = 3;

            int temp = grd.GetUserStatus(Environment.UserName);

            if (temp <= 0)
            {
                txtShift.Document.Blocks.Clear();
                txtShift.Document.Blocks.Add(new Paragraph(new Run(temp == 0 ? "Failed to open main database." : "Your account is no longer active!")));
                btnLogin.IsEnabled = false;

                objBusyIndicator.IsBusy = false;

                return;

            }

            if (UserInitialized)
            {

                try
                {
                    txtShift.Document.Blocks.Clear();
                    txtShift.Document.Blocks.Add(new Paragraph(new Run("Your Shift Schedule " + Strings.Format(Conversions.ToDate(DateTime.Now.Date + grd.grdData.CurrentUser.SchedTimeIn), "h:mmtt").ToString() + "-" + Strings.Format(Conversions.ToDate(DateTime.Now.Date + grd.grdData.CurrentUser.SchedTimeOut), "h:mmtt").ToString() + " " + grd.grdData.TeamInfo.TimeZone)));
                    Log.Text = "Validated. Please click Login";
                }
                catch (Exception ex)
                {

                }

                //grd.grdData.CurrentUser.VDI = grd.IsCitrixHost(Strings.Mid(HostName, 1, 1).ToString().ToUpper());

                if (grd.grdData.CurrentUser.VDI == false)
                {

                    if (grd.grdData.CurrentUser.GridUpdated == false)
                    {
                        VersionUpdated = false;
                    }
                    else
                    {
                        try
                        {
                            if (GridMainVersion.FileVersion.ToString().Length > 6)
                            {
                                //VersionUpdated = grd.IsInstalledVersionLatest(grd.gridData.CurrentUser.EID, GridMainVersion.FileVersion.ToString);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

            }

            if (VersionUpdated == false)
            {
                //Interaction.MsgBox("Grid is not updated.", Constants.vbCritical, grd.AppName);

                if (!((Environment.UserName.ToUpper() ?? "") == ("armand.zablan".ToUpper() ?? "") | (Environment.UserName.ToUpper() ?? "") == ("marklorenzo.caldo".ToUpper() ?? "") | (Environment.UserName.ToUpper() ?? "") == ("christian.acedillo".ToUpper() ?? "") | (Environment.UserName.ToUpper() ?? "") == ("izen.cruz".ToUpper() ?? "")))
                {

                    try
                    {

                        var info = new ProcessStartInfo();
                        info.FileName = Environment.CurrentDirectory + @"\GRID Update.exe";
                        info.WorkingDirectory = Environment.CurrentDirectory;
                        int x = Process.Start(info).Id;
                        Interaction.AppActivate(x);

                        Interaction.AppActivate(info.FileName);
                    }

                    catch (Exception ex)
                    {

                    }
                    //this.Close(); page kasi
                }

            }
            objBusyIndicator.IsBusy = false;
            this.grdMutexx.ReleaseMutex();
        }

        [Obsolete]
        private void PerformLogin()
        {
            bool flg = false;

            DateTime date1;
            DateTime date2;
            DateTime date3;

            DateTime tempTransDate;

            grd.grdData.CurrentUser.TransactionDate = DateTime.Now.ToString("MM/dd/yyyy");
            grd.grdData.CurrentUser.TransactionDate2 = Conversions.ToDate(DateTime.Now.ToString("MM/dd/yyyy"));

            date2 = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);

            date1 = Convert.ToDateTime(date2.Date.ToShortDateString() + " " + grd.grdData.CurrentUser.SchedTimeIn);
            date3 = Convert.ToDateTime(date2.Date.ToShortDateString() + " " + grd.grdData.CurrentUser.SchedTimeOut);

            ScheduleLogIn = date1;
            ScheduleLogOut = date3.AddDays(1);

            //if (date1 > date3)
            //{
            //    date3 = Convert.ToDateTime(date2.AddDays(1).Date.ToShortDateString() + " " + grd.grdData.CurrentUser.SchedTimeOut);
            //}

            //tempTransDate = date1.Date;

            //date1 = date1.AddHours(-3);
            //date3 = date3.AddHours(3);

            //if (!(date2 >= date1 & date2 <= date3))
            //{
            //    date1 = date1.AddDays(-1);
            //    date3 = date3.AddDays(-1);

            //    tempTransDate = date1.AddHours(-3).Date;
            //}

            //if (date2 >= date1 & date2 <= date3)
            //{
            //    grd.grdData.CurrentUser.TransactionDate = Strings.Format(tempTransDate.Date, "MM/dd/yyyy").ToString();
            //    grd.grdData.CurrentUser.TransactionDate2 = tempTransDate.Date;

            //    ScheduleLogIn = date1.AddHours(3);
            //    ScheduleLogOut = date3.AddHours(-3);

            //    grd.grdData.CurrentUser.SchedLogIn = ScheduleLogIn;
            //    grd.grdData.CurrentUser.SchedLogOut = ScheduleLogOut;

            //    flg = true;
            //}

            //if (grd.grdData.CurrentUser.EmpNo == "852761x")
            //{

            //    txtUserName.IsEnabled = false;
            //    txtPassword.IsEnabled = false;
            //    btnLogin.IsEnabled = false;

            //    this.cmbTransdate.Items.Clear();
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(-1).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(1).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(2).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(3).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(4).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(5).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add("08/16/2020");
            //    this.cmbTransdate.SelectedIndex = 0;

            //    this.Height = 430;

            //    cmdSave.IsEnabled = true;

            //    return;
            //}

            //if (grd.gridData.CurrentUser.WithShifdateOption == true)
            //{
            //    txtUserName.IsEnabled = false;
            //    txtPassword.IsEnabled = false;
            //    btnLogin.IsEnabled = false;

            //    this.cmbTransdate.Items.Clear();
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(-1).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.Items.Add(Strings.Format(date2.AddDays(1).Date, "MMM dd, yyyy"));
            //    this.cmbTransdate.SelectedIndex = 0;

            //    this.Height = 430;

            //    cmdSave.IsEnabled = true;

            //    return;

            //}

            //if (flg == false)
            //{
            //    txtUserName.IsEnabled = false;
            //    txtPassword.IsEnabled = false;
            //    btnLogin.IsEnabled = false;

            //    this.cmbTransdate.Items.Clear();

            //    if (date2.Hour == 0 | date2.Hour < 23)
            //    {
            //        cmbTransDate1 = date2.AddDays(-1);
            //        cmbTransDate2 = date2;

            //        this.cmbTransdate.Items.Add(Strings.Format(cmbTransDate1.Date, "MMM dd, yyyy"));
            //        this.cmbTransdate.Items.Add(Strings.Format(cmbTransDate2.Date, "MMM dd, yyyy"));
            //    }
            //    else
            //    {
            //        cmbTransDate1 = date2;
            //        cmbTransDate2 = date2.AddDays(1);

            //        this.cmbTransdate.Items.Add(Strings.Format(cmbTransDate1.Date, "MMM dd, yyyy"));
            //        this.cmbTransdate.Items.Add(Strings.Format(cmbTransDate2.Date, "MMM dd, yyyy"));
            //    }

            //    this.cmbTransdate.SelectedIndex = 0;

            //    this.Height = 440;

            //    cmdSave.IsEnabled = true;

            //    return;
            //}

            this.LoginToTimeCard();

            if (HostName.Length > 0)
            {
                HostName = HostName + "-" + Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MMddyyyy-HHmmss").ToString();
            }
            else
            {
                HostName = "HostName-" + Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MMddyyyy-HHmmss").ToString();
            }

            grd.grdData.CurrentUser.HostName = HostName;


            if (!((Environment.UserName.ToUpper() ?? "") == ("izen.cruz".ToUpper() ?? "") & txtUserName.Text.ToUpper() != "izen.cruz".ToUpper()))
            {
                grd.SetUserLoggedInFlag(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.HostName, grd.grdData.CurrentUser.TransactionDate2);
            }



            if (!((Environment.UserName.ToUpper() ?? "") == ("izen.cruz".ToUpper() ?? "") & txtUserName.Text.ToUpper() != "izen.cruz".ToUpper()))
            {
                this.SaveLogTrail();
            }

            this.Hide();
            System.Threading.Thread.Sleep(1000);
            var n = new MainWindow();
            n.Show();
            this.Close();


            //if (grd.IsMoraleExist(grd.grdData.CurrentUser.TeamId, grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2))
            //{
            //    if (grd.grdData.CurrentUser.OnShore == false)
            //    {
            //        if (Environment.UserName == "izen.cruz")
            //        {
            //            if (txtUserName.Text == "izen.cruz")
            //            {
            //                if (grd.grdData.CurrentUser.TeamId == 213)
            //                {
            //                    try
            //                    {
            //                        if (grd.grdData.WFHInfo.TransDate != grd.grdData.CurrentUser.TransactionDate2)
            //                        {
            //                            //    var frm = new frmSurvey(grd);
            //                            //    frm.ShowDialog();
            //                            //}
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        //var frm = new frmSurvey(grd);
            //                        //frm.ShowDialog();
            //                    }

            //                }
            //            }
            //        }

            //        else
            //        {
            //            //var frm = new frmSurvey(grd);
            //            //frm.ShowDialog();
            //        }

            //    }

            //    this.Hide();
            //    System.Threading.Thread.Sleep(1000);
            //    var n = new MainWindow();
            //    n.Show();
            //    this.Close();
            //}
            //else
            //{


            //    if (grd.grdData.CurrentUser.OnShore == false)
            //    {

            //        if (Environment.UserName == "izen.cruz")
            //        {

            //            if (txtUserName.Text == "izen.cruz")
            //            {
            //                if (grd.grdData.CurrentUser.TeamId == 213)
            //                {
            //                    try
            //                    {
            //                        if (grd.grdData.WFHInfo.TransDate != grd.grdData.CurrentUser.TransactionDate2)
            //                        {
            //                            //var frm = new frmSurvey(grd);
            //                            //frm.ShowDialog();
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        //var frm = new frmSurvey(grd);
            //                        //frm.ShowDialog();
            //                    }

            //                }
            //            }
            //        }


            //        else
            //        {

            //            //var frm = new frmSurvey(grd);
            //            //frm.ShowDialog();
            //        }

            //    }

            //    if ((Environment.UserName.ToUpper() ?? "") == ("izen.cruz".ToUpper() ?? "") & txtUserName.Text.ToUpper() != "izen.cruz".ToUpper())
            //    {
            //        this.Hide();
            //        System.Threading.Thread.Sleep(1000);
            //        var n = new MainWindow();
            //        n.Show();
            //        this.Close();
            //    }
            //    else
            //    {
            //        this.Hide();
            //        System.Threading.Thread.Sleep(1000);
            //        var n = new MainWindow();
            //        n.Show();
            //        this.Close();

            //        //this.Hide();
            //        //System.Threading.Thread.Sleep(1000);
            //        //var m = new frmMorale(grd);
            //        //m.Show();
            //        //this.Close();
            //    }


            //}

        }

        [Obsolete]
        private void SaveLogTrail()
        {

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var localZone = TimeZone.CurrentTimeZone;

            try
            {
                grd.AddLogTrail(grd.grdData.CurrentUser.EmpNo.ToString(), Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MM/dd/yyyy h:mm:ss tt").ToString(), Strings.Format(grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet), "MM/dd/yyyy h:mm:ss tt").ToString(), "Login", Environment.MachineName, grd.grdData.GridVersion, localZone.ToUniversalTime(DateTime.Now).ToString());
            }

            catch (Exception)
            {

            }

        }

        [Obsolete]
        private void LoginToTimeCard()
        {
            string _Remarks;

            if (ScheduleLogIn < grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet))
            {
                grd.grdData.CurrentUser.IsLate = true;
                _Remarks = "Late";
            }
            else
            {
                grd.grdData.CurrentUser.IsLate = false;
                _Remarks = "Present";
            }

            var obj = new gridTimeCard();

            obj.Id = 0;
            obj.UserId = grd.grdData.CurrentUser.EmpNo;
            obj.TeamId = grd.grdData.CurrentUser.TeamId;
            obj.TransDate = Strings.Format(grd.grdData.CurrentUser.TransactionDate2.Date, "MM/dd/yyyy").ToString();
            obj.LogIn = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
            obj.LogOut = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
            obj.Status = true;
            obj.Role = grd.grdData.CurrentUser.Role;
            obj.SupEmpNo = grd.grdData.CurrentUser.SupEmpNo;
            obj.SchedTimeIn = ScheduleLogIn;
            obj.SchedTimeOut = ScheduleLogOut;
            obj.Remarks = _Remarks;
            obj.ModifiedBy = grd.grdData.CurrentUser.EID;
            obj.DateModified = grd.ConvertTimeZone(grd.grdData.TeamInfo.OffSet);
            obj.OffSet = grd.grdData.TeamInfo.OffSet;

            grd.UserLogIn(obj);
        }

        private void Themes_Click(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();
            ImageBrush b = new ImageBrush();

            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));
                grd.grdData.ScrContent.MainBG = b;
                grd.grdData.ScrContent.TimerStart = false;
                grd.grdData.ScrContent.IsDark = false;
                ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
                grd.grdData.ScrContent.MainBG = b;
                grd.grdData.ScrContent.TimerStart = true;
                grd.grdData.ScrContent.IsDark = true;
            }

            paletteHelper.SetTheme(theme);


            //grd.grdData.ScrContent.IsStarted = false;

            //if (Themes.IsChecked == true)
            //{
            //    IsDarkTheme = true;
            //    b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
            //    ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            //    Hey.Foreground = System.Windows.Media.Brushes.Black;
            //    Log.Foreground = System.Windows.Media.Brushes.Black;
            //    txtUserName.BorderBrush = System.Windows.Media.Brushes.Black;
            //    txtUserName.Foreground = System.Windows.Media.Brushes.Black;
            //    txtPassword.BorderBrush = System.Windows.Media.Brushes.Black;
            //    txtPassword.Foreground = System.Windows.Media.Brushes.Black;
            //}
            //else
            //{
            //    IsDarkTheme = false;
            //    b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));
            //    ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            //    txtUserName.BorderBrush = System.Windows.Media.Brushes.LightGray;
            //    //txtUserName.Foreground = System.Windows.Media.Brushes.LightGray;
            //    txtPassword.BorderBrush = System.Windows.Media.Brushes.LightGray;
            //    //txtPassword.Foreground = System.Windows.Media.Brushes.LightGray;
            //}
            //b.Stretch = Stretch.UniformToFill;
            //((MainWindow)Application.Current.MainWindow).Main.Background = b;
        }

        [Obsolete]
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            

            if (grd.grdData.ScrContent.IsInitialized)
            {
                if (grd.grdData.CurrentUser.Status == false)
                {
                    return;
                }

                LoginTime = grd.GetServerDate();
                ActualLoginTime = LoginTime;

                try
                {
                    this.PerformLogin();
                }
                catch (Exception ex)
                {
                    _eMsgBox(ex.Message);
                }


            }
            else
            {
                Interaction.MsgBox("Login Failed!" + Constants.vbCrLf + "User not found: " + txtUserName.Text, Constants.vbExclamation, grd.AppName);
                return;
            }
        }

        private void FrmLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
