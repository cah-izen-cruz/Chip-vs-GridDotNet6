using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static GRIDLibraries.Libraries.GridLib;

namespace GRIDLibraries.Libraries
{   


    public class gridMainScrContent
    {
        public string LiveTabName { get; set; }
        public bool IsStarted { get; set; } = false;
        public ImageBrush MainBG { get; set; }
        public bool IsDark { get; set; } = false;
        public bool TimerStart { get; set; } = false;
        public bool IsBreaktime { get; set; } = false;
        public string BreaktimeName { get; set; } = "";

        public bool IsMyDataChanged { get; set; } = false;

        public bool IsInitialized { get; set; } =false;

        public bool IsStartClicked { get; set; } = false;

        public bool IsBreakClicked { get; set; } = false;

        public bool IsActivityRunning { get; set; } = false;

        public bool IsBreakStarted { get; set; } = false;

    }

    public class gridUserInfo
    {
        public string EmpNo { get; set; }
        public int TeamId { get; set; }
        public string EID { get; set; }

        //public string EmployeeName { get; set; }
        public string EmpName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        public string Email { get; set; }
        public string SupName { get; set; }
        public string SupEmpNo { get; set; }

        public int Role { get; set; }

        public string RoleStr { get; set; }
        public string AccessCode { get; set; }
        public TimeSpan SchedTimeIn { get; set; }
        public TimeSpan SchedTimeOut { get; set; }
        public string PrivelegeCode { get; set; }
        public TimeSpan LunchBreak { get; set; }
        public TimeSpan FirstBreak { get; set; }
        public TimeSpan SecondBreak { get; set; }

        public double ProdTime { get; set; }
        public string TransactionDate { get; set; }
        public DateTime TransactionDate2 { get; set; }

        public DateTime SchedLogIn { get; set; }
        public DateTime SchedLogOut { get; set; }

        public int LogInID { get; set; }
        public DateTime LogIn { get; set; }
        public DateTime LogOut { get; set; }
        public DateTime ActualLogin { get; set; }

        public bool IsLate { get; set; }

        public string AuditQueue { get; set; }
        public string GroupTab { get; set; }
        public string WindId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }

        public bool Status { get; set; }

        public string Skills { get; set; }
        public string RGHId { get; set; }
        public string HostName { get; set; }

        public bool VDI { get; set; }
        public bool OnShore { get; set; }
        public bool GridUpdated { get; set; }
        public bool WithShifdateOption { get; set; }

        public bool ShowCompletedQueue { get; set; }

        public bool ActualTagging { get; set; }

        public bool IsCurrentlyLogged { get; set; }
        public bool AutoSynch { get; set; }

        public string Site { get; set; }
        public string CRMID { get; set; }

        public bool IsSPLogged { get; set; }
    }

    public class gridPerformance
    {
        public void Reset()
        {
            this.Id = 0;
            this.UserId = "";
            this.ActivityId = 0;
            this.TransDate = "";
            this.Status = "";
            // Me.TimeStart = ""
            // Me.TimeEnd = ""
            this.ReferenceId = "";
            this.Volume = 0;
            this.Remarks = "";
            this.ActivityTime = "";
            this.IsClosed = false;
            this.TimeElapsed = "";
            this.IsPaused = false;

            this.LOBId = 0;
            this.LOBItemId = 0;           
        }

       
        public gridPerfInfo PerfConfigData { get; set; }
        public gridActivity Activity { get; set; }

    
        public DateTime idleTimeDate { get; set; }
        public long Id { get; set; }
        public string UserId { get; set; }
        public int ActivityId { get; set; }
        public string TransDate { get; set; }
        public string Status { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }

        public DateTime TransDate2 { get; set; }
        public DateTime TimeStart2 { get; set; }
        public DateTime TimeEnd2 { get; set; }

        public string ReferenceId { get; set; }
        public int Volume { get; set; }
        public string Remarks { get; set; }
        public string ActivityTime { get; set; }
        public bool IsClosed { get; set; }
        public string TimeElapsed { get; set; }
        public int IdleTime { get; set; }
        public int PerfStatus { get; set; }
        public string PerfStatusDesc { get; set; }

        public bool Started { get; set; } = false;
        public bool IsPaused { get; set; } = false;

        public int LOBId { get; set; } = 0;
        public int LOBItemId { get; set; } = 0;
        public int TicketNo { get; set; } = 0;
    }

    public class gridTeam
    {
        public int ProjectId { get; set; }
        public int TowerId { get; set; }
        public int Id { get; set; }
        public int GlobalTeamId { get; set; }
        public string TeamName { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Tower { get; set; }
        public string Cluster { get; set; }
        public int DeptId { get; set; }
        public string Department { get; set; }
        public string Segment { get; set; }
        public string WorkType { get; set; }
        public int OffSet { get; set; }
        public string TimeZone { get; set; }
        public string DBName { get; set; }
        public string Server { get; set; }
        public double Status { get; set; }


        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }
    }

    public class gridActivity 
    {
        public List<PerfConfig>? ConfigInfo { get; set; } 
        
        public gridActivity()
        {
            ConfigInfo = new List<PerfConfig>();
        }
       
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string ActName { get; set; }
        public string AHT { get; set; }
        public string Type { get; set; }
        public string Process { get; set; }
        public bool Status { get; set; }
        public int LOBId { get; set; }
        public int LOBItemId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }

        public int ConfigId { get; set; }
        public bool StandardAct { get; set; }
        public bool DailyDashboard { get; set; }
        public bool DailyScorecard { get; set; }
        public bool RiskIssue { get; set; }
        public bool IsPublic { get; set; }

        public string UserId { get; set; }


    }

    public class gridPerfConfigItem
    {
        public int Id { get; set; }
        public int PerfConfigId { get; set; }
        public string Item { get; set; }
    }

    public class PerfConfig
    {
        public List<gridPerfConfigItem> ConfigItem { get; set; }

        public PerfConfig()
        {
            ConfigItem = new List<gridPerfConfigItem>();
        }

        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string FieldName { get; set; }
        public string DataType { get; set; }
        public string ObjectType { get; set; }
        public bool IsRequired { get; set; }
        public string Desc { get; set; }
        public bool WithItem { get; set; }
        public bool Status { get; set; }

        public bool UseCurrentDate { get; set; }
        public int Sequence { get; set; }
    }

    public partial class gridPerfInfo
    {

        public long PerfId { get; set; }
        public string UserId { get; set; }
        public string TimeStart { get; set; }

        public string RF01 { get; set; }
        public string RF02 { get; set; }
        public string RF03 { get; set; }
        public string RF04 { get; set; }
        public string RF05 { get; set; }
        public string RF06 { get; set; }
        public string RF07 { get; set; }
        public string RF08 { get; set; }
        public string RF09 { get; set; }
        public string RF10 { get; set; }
        public string RF11 { get; set; }
        public string RF12 { get; set; }
        public string RF13 { get; set; }
        public string RF14 { get; set; }
        public string RF15 { get; set; }
        public string RF16 { get; set; }
        public string RF17 { get; set; }
        public string RF18 { get; set; }
        public string RF19 { get; set; }
        public string RF20 { get; set; }
        public string RF21 { get; set; }
        public string RF22 { get; set; }
        public string RF23 { get; set; }
        public string RF24 { get; set; }
        public string RF25 { get; set; }
        public string RF26 { get; set; }
        public string RF27 { get; set; }
        public string RF28 { get; set; }
        public string RF29 { get; set; }
        public string RF30 { get; set; }
        public string RF31 { get; set; }
        public string RF32 { get; set; }
        public string RF33 { get; set; }
        public string RF34 { get; set; }
        public string RF35 { get; set; }
        public string RF36 { get; set; }
        public string RF37 { get; set; }
        public string RF38 { get; set; }
        public string RF39 { get; set; }
        public string RF40 { get; set; }
        public string RF41 { get; set; }
        public string RF42 { get; set; }
        public string RF43 { get; set; }
        public string RF44 { get; set; }
        public string RF45 { get; set; }
        public string RF46 { get; set; }
        public string RF47 { get; set; }
        public string RF48 { get; set; }
        public string RF49 { get; set; }
        public string RF50 { get; set; }

        public int Count { get; set; }



        public string Description { get; set; }



    }

    public partial class gridTimeCard
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string SupEmpNo { get; set; }
        public string SupName { get; set; }
        public string EID { get; set; }
        public string EmpName { get; set; }
        public int TeamId { get; set; }
        public string TransDate { get; set; }
        public DateTime TransDate2 { get; set; }
        public DateTime LogIn { get; set; }
        public DateTime LogOut { get; set; }
        public string TotalTime { get; set; }
        public int Role { get; set; }
        public DateTime SchedTimeIn { get; set; }
        public DateTime SchedTimeOut { get; set; }
        public string Remarks { get; set; }
        public bool Status { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }

        public long ProdOT { get; set; }
        public int OffSet { get; set; }
        public int HeadCount { get; set; }
        public string IsManualEntry { get; set; }
        public string Reason { get; set; }

        public string Comments { get; set; }

    }

    public partial class GridWFHinfo : IDisposable
    {

        public long Id { get; set; } // 
        public string UserId { get; set; }
        public DateTime TransDate { get; set; }
        public bool IsWFH { get; set; }
        public int Opt { get; set; }
        public string Other { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        // Protected Overrides Sub Finalize()
        // ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    public partial class gridMainAgentMetrics
    {

        public double Efficiency { get; set; }
        public double Utilization { get; set; }
        public string TransactionDate { get; set; }
        public string ProductionTime { get; set; }
        public string ShrinkageTime { get; set; }
        public string BreakTime { get; set; }
        public string AdminTime { get; set; }
        public int OpenActivities { get; set; }
        public int ClosedActivities { get; set; }


        public long ProdTimeSec { get; set; }
        public long ShrinkTimeSec { get; set; }
        public long AdminTimeSec { get; set; }
        public long BreakTimeSec { get; set; }

    }

    public class QAQuestionForm
    {
        public List<QAPerfConfig>? ListOfQA { get; set; }

        public DataTable dtLOB = new DataTable();
        public DataTable dtQAQuestionnaire = new DataTable();
        public DataTable dtQASelection = new DataTable();
        public DataTable dtQAMarkdownSelection = new DataTable();

        public DataTable dtObjContainer = new DataTable();
        public DataRow drObjContainer;


        public QAQuestionForm()
        {
            ListOfQA = new List<QAPerfConfig>();

            //DataTable dtLOB;
            //DataTable dtQAQuestionnaire;
            //DataTable dtQASelection;
            //DataTable dtQAMarkdownSelection;
        }


        public int UserId { get; set; }
        public string EmpName { get; set; }
        public int LOBId { get; set; }
        public string Name { get; set; }
        public int QID { get; set; }
        public string Question { get; set; }
        public string ObjectType { get; set; }
        public string Remarks { get; set; }          
        public int MaxLOBId { get; set; }
        public int MaxQID { get; set; }       
        public int SelId { get; set; }
        public int MarkId { get; set; }
        public string SelectionValue { get; set; }
        public double Score { get; set; }
        public string MarkdownType { get; set; }
        public string Formula { get; set; }
        public int Target { get; set; }
        public string ScoreRemarks { get; set; }

    }


    public class QAPerfConfig
    {
        public List<gridQAPerfConfigItem> QAConfigItem { get; set; }

        public QAPerfConfig()
        {
            QAConfigItem = new List<gridQAPerfConfigItem>();
        }

        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string FieldName { get; set; }
        public string DataType { get; set; }
        public string ObjectType { get; set; }
        public bool IsRequired { get; set; }
        public string Desc { get; set; }
        public bool WithItem { get; set; }
        public bool Status { get; set; }

        public bool UseCurrentDate { get; set; }
        public int Sequence { get; set; }

        
    }


    public class gridQAPerfConfigItem
    {
        public int Id { get; set; }
        public int PerfConfigId { get; set; }
        public string Item { get; set; }
    }



}
