using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public int CreateCopyOfPendingActivity(string UserId, DateTime TransDate)
        {

            int ctr = 0;

            var dt = new DataTable();

            if (this.OpenDbConnection())
            {


                var da = new SqlDataAdapter();
                da = new SqlDataAdapter(@"SELECT dbo.tblPerformance.Id,[TimeElapsed]
                            FROM dbo.tblActivity WITH (NOLOCK) INNER JOIN dbo.tblPerformance WITH (NOLOCK) ON dbo.tblActivity.Id = dbo.tblPerformance.ActivityId
                            WHERE (dbo.tblActivity.RetainPending=1) AND (dbo.tblPerformance.PerfStatus<2) AND (dbo.tblPerformance.UserId=@UserId) AND (dbo.tblPerformance.TransDate<@TransDate);", this.gridDbConnection);

                da.SelectCommand.Parameters.AddWithValue("@UserId", UserId);
                da.SelectCommand.Parameters.AddWithValue("@TransDate", TransDate.ToShortDateString());

                da.SelectCommand.CommandTimeout = 1000;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception)
                {
                }

                this.CloseDbConnection();

            }

            if (!(dt == null))
            {
                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("NewId");
                    if (this.OpenDbConnection())
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            //rw.item("NewId") = 0;

                            row["NewId"] =0;

                            this.gridDbCommand.Parameters.Clear();
                            this.gridDbCommand.Parameters.AddWithValue("@Id", Conversions.ToInteger(row["Id"]));
                            this.gridDbCommand.Parameters.AddWithValue("@TransDate", TransDate.ToShortDateString());
                            this.gridDbCommand.Parameters.AddWithValue("@Remarks", "Recreated-" + ConvertTimeZone(grdData.TeamInfo.OffSet).ToString());


                            if ((row["TimeElapsed"].ToString() ?? "") == ("00:00:01".ToString() ?? ""))
                            {

                                this.gridDbCommand.CommandText = "UPDATE [dbo].[tblPerformance] SET [TransDate]=@TransDate,[Remarks]=@Remarks WHERE [Id]=@Id;";

                                try
                                {
                                    this.gridDbCommand.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                            else
                            {

                                this.gridDbCommand.CommandText = "INSERT INTO [dbo].[tblPerformance] " + "SELECT [UserId],@TransDate As [TransDate],[ActivityId],[TimeStart],[TimeEnd],[ReferenceId],[Volume],@Remarks As [Remarks],'00:00:01' AS [TimeElapsed],'00:00:00' AS [IdleTime],1 AS [Status], 1 AS [IsPaused],[PerfStatus],[LOBId],[LOBItemId] FROM [dbo].[tblPerformance] WITH (NOLOCK) WHERE [Id]=@Id; " + "SELECT SCOPE_IDENTITY()";

                                try
                                {
                                    row["NewId"] = this.gridDbCommand.ExecuteScalar();
                                }
                                catch (Exception ex)
                                {
                                }

                                if (Conversion.Val(row["NewId"]) > 0d)
                                {


                                    this.gridDbCommand.CommandText = "UPDATE [dbo].[tblPerformance] SET [PerfStatus]=2,[Remarks]=@Remarks WHERE [Id]=@Id;";

                                    try
                                    {
                                        this.gridDbCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                }
                            }

                        }

                        this.CloseDbConnection();

                    }
                }
            }


            if (!(dt == null))
            {
                if (dt.Rows.Count > 0)
                {
                    if (this.OpenDbConnection())
                    {
                        foreach (DataRow rw in dt.DefaultView) // foreach (DataRow row in dt.Rows)
                        {
                           
                            this.gridDbCommand.Parameters.Clear();
                            this.gridDbCommand.Parameters.AddWithValue("@PerfId", Conversions.ToInteger(rw["Id"]));
                            this.gridDbCommand.Parameters.AddWithValue("@NewPerfId", Conversions.ToInteger(rw["NewId"]));

                            if (Conversions.ToInteger(rw["NewId"]) > 0)
                            {

                                this.gridDbCommand.CommandText = "INSERT INTO [dbo].[tblPerfInfo] " + "SELECT @NewPerfId AS [PerfId],[QueueId],[RF01],[RF02],[RF03],[RF04],[RF05],[RF06],[RF07],[RF08],[RF09],[RF10],[RF11],[RF12],[RF13],[RF14],[RF15],[RF16],[RF17],[RF18],[RF19],[RF20],[RF21],[RF22],[RF23],[RF24],[RF25],[RF26],[RF27],[RF28],[RF29],[RF30],[RF31],[RF32],[RF33],[RF34],[RF35],[RF36],[RF37],[RF38],[RF39],[RF40],[RF41],[RF42],[RF43],[RF44],[RF45],[RF46],[RF47],[RF48],[RF49],[RF50] FROM [dbo].[tblPerfInfo] WITH (NOLOCK) WHERE [PerfId]=@PerfId;";

                                try
                                {
                                    if (this.gridDbCommand.ExecuteNonQuery() > 0)
                                        ctr += 1;
                                }
                                catch (Exception ex)
                                {
                                }


                            }



                        }

                        this.CloseDbConnection();

                    }
                }
            }

            return ctr;

        }

        [Obsolete]
        public long AddPerformanceInfo(gridPerformance ObjPerfData)
        {

            bool IsSaved = false;
            long temp = 0L;
            string Exp = "";

            if (this.OpenDbConnection())
            {
                for (int i = 0; i <= 4; i++)
                {


                    this.gridDbCommand.Parameters.Clear();

                    this.gridDbCommand.Parameters.AddWithValue("@UserId", ObjPerfData.UserId);
                    this.gridDbCommand.Parameters.AddWithValue("@TransDate", ObjPerfData.TransDate2.ToShortDateString());
                    this.gridDbCommand.Parameters.AddWithValue("@ActivityId", ObjPerfData.ActivityId);
                    this.gridDbCommand.Parameters.AddWithValue("@TimeStart", ObjPerfData.TimeStart2.ToString());
                    this.gridDbCommand.Parameters.AddWithValue("@TimeEnd", ObjPerfData.TimeEnd2.ToString());
                    this.gridDbCommand.Parameters.AddWithValue("@ReferenceId", ObjPerfData.ReferenceId);
                    this.gridDbCommand.Parameters.AddWithValue("@TimeElapsed", ObjPerfData.TimeElapsed);
                    this.gridDbCommand.Parameters.AddWithValue("@IdleTime", ConvertSecondsToHHMMSS((int)ObjPerfData.IdleTime < 0 ? 0 : (int)ObjPerfData.IdleTime));
                    this.gridDbCommand.Parameters.AddWithValue("@Status", ObjPerfData.Status);
                    this.gridDbCommand.Parameters.AddWithValue("@IsPaused", ObjPerfData.IsPaused);
                    this.gridDbCommand.Parameters.AddWithValue("@PerfStatus", ObjPerfData.PerfStatus);
                    this.gridDbCommand.Parameters.AddWithValue("@LOBId", ObjPerfData.LOBId);
                    this.gridDbCommand.Parameters.AddWithValue("@LOBItemId", ObjPerfData.LOBItemId);
                    this.gridDbCommand.Parameters.AddWithValue("@Remarks", this.ConvertTimeZone(this.grdData.TeamInfo.OffSet).ToString());
                    this.gridDbCommand.Parameters.AddWithValue("@Volume", "1");

                    this.gridDbCommand.CommandText = "INSERT tblPerformance ([UserId],[TransDate],[ActivityId],[TimeStart],[TimeEnd],[ReferenceId],[TimeElapsed],[IdleTime],[Status],[IsPaused],[PerfStatus],[LOBId],[LOBItemId],[Remarks],[Volume]) " + "VALUES (@UserId,@TransDate,@ActivityId,@TimeStart,@TimeEnd,@ReferenceId,@TimeElapsed,@IdleTime,@Status,@IsPaused,@PerfStatus,@LOBId,@LOBItemId,@Remarks,@Volume); " + "SELECT SCOPE_IDENTITY()";
                    try
                    {//(long) 
                        temp = Convert.ToInt64(this.gridDbCommand.ExecuteScalar());
                        if (temp > 0L)
                        {
                            i = 5;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Exp = ex.Message.ToString();
                    }

                }


                this.CloseDbConnection();


            }


            if (temp == 0L)
            {

                string ss;

                ss = Constants.vbCrLf + "Id:" + ObjPerfData.Id.ToString();
                ss = ss + Constants.vbCrLf + "UserId:" + ObjPerfData.UserId;
                ss = ss + Constants.vbCrLf + "TransDate:" + ObjPerfData.TransDate.ToString();
                ss = ss + Constants.vbCrLf + "ActivityId:" + ObjPerfData.ActivityId.ToString();
                ss = ss + Constants.vbCrLf + "TimeStart:" + ObjPerfData.TimeStart.ToString();
                ss = ss + Constants.vbCrLf + "TimeEnd:" + ObjPerfData.TimeEnd.ToString();
                ss = ss + Constants.vbCrLf + "TransDate2:" + ObjPerfData.TransDate2.ToString();
                ss = ss + Constants.vbCrLf + "TimeStart2:" + ObjPerfData.TimeStart2.ToString();
                ss = ss + Constants.vbCrLf + "TimeEnd2:" + ObjPerfData.TimeEnd2.ToString();
                ss = ss + Constants.vbCrLf + "TimeElapsed:" + ObjPerfData.TimeElapsed.ToString();
                ss = ss + Constants.vbCrLf + "IdleTime:" + ObjPerfData.IdleTime.ToString();
                ss = ss + Constants.vbCrLf + "Status:" + ObjPerfData.Status.ToString();
                ss = ss + Constants.vbCrLf + "IsPaused:" + ObjPerfData.IsPaused.ToString();
                ss = ss + Constants.vbCrLf + "ReferenceId:" + ObjPerfData.ReferenceId.ToString();
                ss = ss + Constants.vbCrLf + "LOBId:" + ObjPerfData.LOBId.ToString();
                ss = ss + Constants.vbCrLf + "LOBItemId:" + ObjPerfData.LOBItemId.ToString();

                ss = ss + Constants.vbCrLf + "PerfStatus:" + ObjPerfData.PerfStatus.ToString();

                this.LogError("AddPerformanceInfo", Exp, ss);

                // temp = Me.AddPerformanceInfoOpt2(ObjPerfData)

            }


            return temp;





        }

        public bool LogError(string _Activity, string _Exception, string _Details)
        {
            bool LogErrorRet = default;

            LogErrorRet = false;

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();

                this.gridDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                this.gridDbCommand.Parameters.AddWithValue("@Date", DateTime.Now.ToString());
                this.gridDbCommand.Parameters.AddWithValue("@Activity", _Activity);
                this.gridDbCommand.Parameters.AddWithValue("@Exception", Strings.Len(_Exception) > 500 ? Strings.Mid(_Exception, 1, 500) : _Exception);
                this.gridDbCommand.Parameters.AddWithValue("@Details", Strings.Len(_Details) > 1000 ? Strings.Mid(_Details, 1, 1000) : _Details);
                this.gridDbCommand.Parameters.AddWithValue("@Version", this.grdData.GridVersion);
                this.gridDbCommand.CommandText = "INSERT INTO tblException ([UserId],[Date],[Activity],[Exception],[Details],[Version]) VALUES (@UserId,@Date,@Activity,@Exception,@Details,@Version);";

                try
                {
                    if (this.gridDbCommand.ExecuteNonQuery() > 0)
                        LogErrorRet = true;
                }
                catch (Exception ex)
                {
                }

                this.CloseDbConnection();

            }

            return LogErrorRet;


        }



    }
}
