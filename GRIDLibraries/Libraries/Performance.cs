using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public List<gridActivity> GetDistinctPerfActivity(string UserId, DateTime TransDate, int Stat)
        {

            var retList = new List<gridActivity>();
            retList.Add(new gridActivity() { Id = 0, ActName = "-All-" });

            this.grdMutexx.WaitOne();

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@UserId", UserId);
                this.gridDbCommand.Parameters.AddWithValue("@TransDate", Strings.Format(TransDate, "MM/dd/yyyy").ToString());
                this.gridDbCommand.Parameters.AddWithValue("@Status", Stat.ToString());
                //this.gridDbCommand.CommandText = "SELECT DISTINCT [ActivityId],[Name] FROM vPerformanceActivity WHERE [UserId]=@UserId AND [TransDate]=@TransDate AND [Status]=@Status;";
                //this.gridDbCommand.CommandText = "SELECT DISTINCT [ActivityId],[ActName] FROM vPerformanceActivity WHERE [UserId]=@UserId AND [TransDate]=@TransDate AND [Status]=@Status;";

                this.gridDbCommand.CommandText = "SELECT DISTINCT tblActivity.Id AS Id, tblActivity.ActName AS ActName from tblActivity INNER JOIN tblPerformance ON tblActivity.Id = tblPerformance.ActivityId WHERE tblPerformance.[UserId]=@UserId AND tblPerformance.[TransDate]=@TransDate AND tblPerformance.[Status]=@Status;";
                var dr = this.gridDbCommand.ExecuteReader();

                while (dr.Read())
                    //retList.Add(new gridActivity() { Id = (int)dr["ActivityId"], ActName = (string)dr["Name"] });
                    retList.Add(new gridActivity() { Id = (int)dr["Id"], ActName = (string)dr["ActName"] });
                dr.Close();

                this.CloseDbConnection();

            }

            this.grdMutexx.ReleaseMutex();

            return retList;

        }

        public List<string> GetConfigFieldName(int ActId)
        {

            var temp = new List<string>();


            this.grdMutexx.WaitOne();

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();


                this.gridDbCommand.Parameters.AddWithValue("@Id", ActId);

                if (ActId > 0)
                {
                    this.gridDbCommand.CommandText = @"SELECT FieldName FROM tblPerfConfig WHERE ActivityId =@Id ORDER BY [Sequence];";
                    //this.gridDbCommand.CommandText = @"SELECT FieldName FROM tblActivityConfig WHERE ActivityId =@Id ORDER BY [Sequence];";

                    try
                    {

                        var dr = this.gridDbCommand.ExecuteReader();
                        while (dr.Read())
                            temp.Add((string)dr["FieldName"]);
                        dr.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                }


                else
                {
                    this.gridDbCommand.CommandText = @"SELECT distinct TOP 1  Count(tblPerfConfig.Id) AS CountOfConfigId
                                                From tblActivity INNER Join tblPerfConfig On tblActivity.Id = tblPerfConfig.ActivityId
                                                Where tblActivity.Id = 0
                                                Group By tblActivity.Id
                                                Order By Count(tblPerfConfig.Id) DESC;";

                    //this.gridDbCommand.CommandText = @"SELECT distinct TOP 1  Count(tblActivityConfig.Id) AS CountOfConfigId
                    //                            From tblActivity INNER Join tblActivityConfig On tblActivity.Id = tblActivityConfig.ActivityId
                    //                            Where tblActivity.Id = 0
                    //                            Group By tblActivity.Id
                    //                            Order By Count(tblActivityConfig.Id) DESC;";

                    int RF = 0;
                    try
                    {

                        var dr = this.gridDbCommand.ExecuteReader();
                        if (dr.Read())
                        {
                            RF = (int)dr["CountOfConfigId"];
                        }
                        dr.Close();
                    }
                    catch (Exception)
                    {

                    }


                    if (RF > 0)
                    {
                        for (int i = 1, loopTo = RF; i <= loopTo; i++)
                            temp.Add("RF" + Strings.Format(i, "00"));
                    }

                }




                this.CloseDbConnection();

            }


            this.grdMutexx.ReleaseMutex();

            return temp;


        }

        public List<gridPerformance> GetPerformances(int ActId, int Stat)
        {

            var retList = new List<gridPerformance>();

            this.grdMutexx.WaitOne();

            string query = "SELECT tblPerformance.Id, tblPerformance.UserId, tblPerformance.ActivityId, tblPerformance.TransDate, tblPerformance.Status, "
                       + "tblPerformance.TimeStart, tblPerformance.TimeEnd, tblPerformance.ReferenceId, tblPerformance.Remarks, tblPerformance.TimeElapsed, "
                       + "tblPerformance.IdleTime, tblPerformance.IsPaused, tblPerformance.PerfStatus, tblPerformance.LOBId, tblPerformance.LOBItemId, tblActivity.ActName, tblActivity.AHT,tblActivity.Type, "
                       + "tblActivity.Process, tblPerfInfo.RF01, tblPerfInfo.RF02, tblPerfInfo.RF03, tblPerfInfo.RF04, tblPerfInfo.RF05, "
                       + "tblPerfInfo.RF06, tblPerfInfo.RF07, tblPerfInfo.RF08, tblPerfInfo.RF09, tblPerfInfo.RF10, tblPerfInfo.RF11, tblPerfInfo.RF12, tblPerfInfo.RF13, tblPerfInfo.RF14, tblPerfInfo.RF15, "
                       + "tblPerfInfo.RF16, tblPerfInfo.RF17, tblPerfInfo.RF18, tblPerfInfo.RF19, tblPerfInfo.RF20, tblPerfInfo.RF21, tblPerfInfo.RF22, tblPerfInfo.RF23, tblPerfInfo.RF24, tblPerfInfo.RF25, tblPerfInfo.RF26, "
                       + "tblPerfInfo.RF27, tblPerfInfo.RF28, tblPerfInfo.RF29, tblPerfInfo.RF30, tblPerfInfo.RF31, tblPerfInfo.RF32, tblPerfInfo.RF33, tblPerfInfo.RF34, tblPerfInfo.RF35, tblPerfInfo.RF36, tblPerfInfo.RF37, "
                       + "tblPerfInfo.RF38, tblPerfInfo.RF39, tblPerfInfo.RF40, tblPerfInfo.RF41, tblPerfInfo.RF42, tblPerfInfo.RF43, tblPerfInfo.RF44, tblPerfInfo.RF45, tblPerfInfo.RF46, tblPerfInfo.RF47, tblPerfInfo.RF48, tblPerfInfo.RF49, tblPerfInfo.RF50 "
                       + "FROM(tblActivity INNER JOIN tblPerformance ON tblActivity.Id = tblPerformance.ActivityId) LEFT JOIN tblPerfInfo ON tblPerformance.Id = tblPerfInfo.PerfId";

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                this.gridDbCommand.Parameters.AddWithValue("@TransDate", this.grdData.CurrentUser.TransactionDate);
                this.gridDbCommand.Parameters.AddWithValue("@Status", Stat.ToString());



                if (ActId > 0)
                {
                    this.gridDbCommand.Parameters.AddWithValue("@ActivityId", ActId);
                    //this.gridDbCommand.CommandText = "SELECT * FROM vPERFORMANCEACTIVITY WHERE [UserId]=@UserId AND [TransDate]=@TransDate AND [Status]=@Status AND [ActivityId]=@ActivityId;";

                    this.gridDbCommand.CommandText = query + " WHERE tblPerformance.[UserId]=@UserId AND tblPerformance.[TransDate]=@TransDate AND tblPerformance.[Status]=@Status AND tblPerformance.[ActivityId]=@ActivityId;";
                }
                else
                {
                    //this.gridDbCommand.CommandText = "SELECT * FROM vPERFORMANCEACTIVITY WHERE [UserId]=@UserId AND [TransDate]=@TransDate AND [Status]=@Status;";
                    this.gridDbCommand.CommandText = query + " WHERE tblPerformance.[UserId]=@UserId AND tblPerformance.[TransDate]=@TransDate AND tblPerformance.[Status]=@Status;";
                }

                var dr = this.gridDbCommand.ExecuteReader();

                while (dr.Read())
                {

                    var newPerf = new gridPerformance();
                    newPerf.Activity = new gridActivity();

                    newPerf.Id = dr.GetInt64("Id");
                    newPerf.TimeElapsed = dr.GetValue("TimeElapsed").ToString();
                    newPerf.TimeStart = dr.GetValue("TimeStart").ToString();
                    newPerf.TimeEnd = dr.GetValue("TimeEnd").ToString();
                    newPerf.ReferenceId = dr.GetString("ReferenceId");
                    newPerf.LOBItemId = Convert.ToInt32(dr["LOBItemId"]);
                    newPerf.Remarks = "";
                    newPerf.Status = "";

                    //newPerf.Activity.ActName = dr.GetString("Name");
                    //newPerf.Activity.ActName = dr.GetString("ActName");
                    newPerf.ActivityName = dr.GetString("ActName");
                    newPerf.Activity.AHT = dr.GetString("AHT");
                    //newPerf.Activity.Type = dr.GetString("Type");
                    newPerf.Type = dr.GetString("Type");
                    newPerf.Activity.Id = Convert.ToInt32(dr["ActivityId"]);
                    //newPerf.Activity.Process = dr.GetString("Process");
                    newPerf.Process = dr.GetString("Process");




                    newPerf.PerfConfigData = new gridPerfInfo()
                    {
                        RF01 = (string)Interaction.IIf(dr["RF01"].ToString() is DBNull, Constants.vbNullString, dr["RF01"].ToString()),
                        RF02 = (string)Interaction.IIf(dr["RF02"].ToString() is DBNull, Constants.vbNullString, dr["RF02"].ToString()),
                        RF03 = (string)Interaction.IIf(dr["RF03"].ToString() is DBNull, Constants.vbNullString, dr["RF03"].ToString()),
                        RF04 = (string)Interaction.IIf(dr["RF04"].ToString() is DBNull, Constants.vbNullString, dr["RF04"].ToString()),
                        RF05 = (string)Interaction.IIf(dr["RF05"].ToString() is DBNull, Constants.vbNullString, dr["RF05"].ToString()),
                        RF06 = (string)Interaction.IIf(dr["RF06"].ToString() is DBNull, Constants.vbNullString, dr["RF06"].ToString()),
                        RF07 = (string)Interaction.IIf(dr["RF07"].ToString() is DBNull, Constants.vbNullString, dr["RF07"].ToString()),
                        RF08 = (string)Interaction.IIf(dr["RF08"].ToString() is DBNull, Constants.vbNullString, dr["RF08"].ToString()),
                        RF09 = (string)Interaction.IIf(dr["RF09"].ToString() is DBNull, Constants.vbNullString, dr["RF09"].ToString()),
                        RF10 = (string)Interaction.IIf(dr["RF10"].ToString() is DBNull, Constants.vbNullString, dr["RF10"].ToString()),
                        RF11 = (string)Interaction.IIf(dr["RF11"].ToString() is DBNull, Constants.vbNullString, dr["RF11"].ToString()),
                        RF12 = (string)Interaction.IIf(dr["RF12"].ToString() is DBNull, Constants.vbNullString, dr["RF12"].ToString()),

                        RF13 = (string)Interaction.IIf(dr["RF13"] is DBNull, Constants.vbNullString, dr["RF13"]),
                        RF14 = (string)Interaction.IIf(dr["RF14"] is DBNull, Constants.vbNullString, dr["RF14"]),
                        RF15 = (string)Interaction.IIf(dr["RF15"] is DBNull, Constants.vbNullString, dr["RF15"]),
                        RF16 = (string)Interaction.IIf(dr["RF16"] is DBNull, Constants.vbNullString, dr["RF16"]),
                        RF17 = (string)Interaction.IIf(dr["RF17"] is DBNull, Constants.vbNullString, dr["RF17"]),
                        RF18 = (string)Interaction.IIf(dr["RF18"] is DBNull, Constants.vbNullString, dr["RF18"]),
                        RF19 = (string)Interaction.IIf(dr["RF19"] is DBNull, Constants.vbNullString, dr["RF19"]),
                        RF20 = (string)Interaction.IIf(dr["RF20"] is DBNull, Constants.vbNullString, dr["RF20"]),
                        RF21 = (string)Interaction.IIf(dr["RF21"] is DBNull, Constants.vbNullString, dr["RF21"]),
                        RF22 = (string)Interaction.IIf(dr["RF22"] is DBNull, Constants.vbNullString, dr["RF22"]),
                        RF23 = (string)Interaction.IIf(dr["RF23"] is DBNull, Constants.vbNullString, dr["RF23"]),
                        RF24 = (string)Interaction.IIf(dr["RF24"] is DBNull, Constants.vbNullString, dr["RF24"]),
                        RF25 = (string)Interaction.IIf(dr["RF25"] is DBNull, Constants.vbNullString, dr["RF25"]),
                        RF26 = (string)Interaction.IIf(dr["RF26"] is DBNull, Constants.vbNullString, dr["RF26"]),
                        RF27 = (string)Interaction.IIf(dr["RF27"] is DBNull, Constants.vbNullString, dr["RF27"]),
                        RF28 = (string)Interaction.IIf(dr["RF28"] is DBNull, Constants.vbNullString, dr["RF28"]),
                        RF29 = (string)Interaction.IIf(dr["RF29"] is DBNull, Constants.vbNullString, dr["RF29"]),
                        RF30 = (string)Interaction.IIf(dr["RF30"] is DBNull, Constants.vbNullString, dr["RF30"]),
                        RF31 = (string)Interaction.IIf(dr["RF31"] is DBNull, Constants.vbNullString, dr["RF31"]),
                        RF32 = (string)Interaction.IIf(dr["RF32"] is DBNull, Constants.vbNullString, dr["RF32"]),
                        RF33 = (string)Interaction.IIf(dr["RF33"] is DBNull, Constants.vbNullString, dr["RF33"]),
                        RF34 = (string)Interaction.IIf(dr["RF34"] is DBNull, Constants.vbNullString, dr["RF34"]),
                        RF35 = (string)Interaction.IIf(dr["RF35"] is DBNull, Constants.vbNullString, dr["RF35"]),
                        RF36 = (string)Interaction.IIf(dr["RF36"] is DBNull, Constants.vbNullString, dr["RF36"]),
                        RF37 = (string)Interaction.IIf(dr["RF37"] is DBNull, Constants.vbNullString, dr["RF37"]),
                        RF38 = (string)Interaction.IIf(dr["RF38"] is DBNull, Constants.vbNullString, dr["RF38"]),
                        RF39 = (string)Interaction.IIf(dr["RF39"] is DBNull, Constants.vbNullString, dr["RF39"]),
                        RF40 = (string)Interaction.IIf(dr["RF40"] is DBNull, Constants.vbNullString, dr["RF40"]),
                        RF41 = (string)Interaction.IIf(dr["RF41"] is DBNull, Constants.vbNullString, dr["RF41"]),
                        RF42 = (string)Interaction.IIf(dr["RF42"] is DBNull, Constants.vbNullString, dr["RF42"]),
                        RF43 = (string)Interaction.IIf(dr["RF43"] is DBNull, Constants.vbNullString, dr["RF43"]),
                        RF44 = (string)Interaction.IIf(dr["RF44"] is DBNull, Constants.vbNullString, dr["RF44"]),
                        RF45 = (string)Interaction.IIf(dr["RF45"] is DBNull, Constants.vbNullString, dr["RF45"]),
                        RF46 = (string)Interaction.IIf(dr["RF46"] is DBNull, Constants.vbNullString, dr["RF46"]),
                        RF47 = (string)Interaction.IIf(dr["RF47"] is DBNull, Constants.vbNullString, dr["RF47"]),
                        RF48 = (string)Interaction.IIf(dr["RF48"] is DBNull, Constants.vbNullString, dr["RF48"]),
                        RF49 = (string)Interaction.IIf(dr["RF49"] is DBNull, Constants.vbNullString, dr["RF49"]),
                        RF50 = (string)Interaction.IIf(dr["RF50"] is DBNull, Constants.vbNullString, dr["RF50"])
                    };

                    retList.Add(newPerf);
                }

                //retList.Add(newPerf);
                dr.Close();

                this.CloseDbConnection();

            }

            this.grdMutexx.ReleaseMutex();









            return retList;

        }
    }
}
