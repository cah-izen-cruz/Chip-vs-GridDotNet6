using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public gridPerfInfo PerfConfigData { get; private set; }
        public string TimeStart { get; private set; }

        public List<gridPerformance> MICopyPerfFromMain(string UserId, string TransDate)
        {
            List<gridPerformance> MICopyPerfFromMainRet = null;
            var Perflst = new List<gridPerformance>();

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@UserId", UserId);
                this.gridDbCommand.Parameters.AddWithValue("@TransDate", TransDate);

                this.gridDbCommand.CommandText = "SELECT * FROM tblPerformance WITH (NOLOCK) WHERE UserId=@UserId AND TransDate=@TransDate;";

                try
                {
                    SqlDataReader dr = this.gridDbCommand.ExecuteReader();

                    while (dr.Read())
                    {
                        //dr.GetTimeSpan(dr.GetOrdinal("LunchBreak"))
                        //Conversions.ToDate(dr.GetDateTime("IdleTime"));
                        //this.grdData.CurrentActivity.idleTimeDate = (DateTime)dr["IdleTime"];
                        //int idleTime = this.grdData.CurrentActivity.idleTimeDate.Hour * 60 * 60 + this.grdData.CurrentActivity.idleTimeDate.Minute * 60 + this.grdData.CurrentActivity.idleTimeDate.Second;

                        gridPerformance itm = new gridPerformance();
                        {
                            itm.Id = dr.GetInt64("Id");
                            itm.UserId = dr.GetString("UserId").ToString();
                            itm.TransDate = Strings.Format(Conversions.ToDate(dr.GetDateTime("TransDate")), "MM/dd/yyyy");
                            itm.ActivityId = dr.GetInt32("ActivityId");
                            itm.TimeStart = Strings.Format(Conversions.ToDate(dr.GetDateTime("TimeStart")), "MM/dd/yyyy h:mm:ss tt").ToString();
                            itm.ReferenceId = dr.GetString("ReferenceId").ToString();
                            itm.Remarks = dr.GetString("Remarks").ToString();
                            itm.TimeEnd = Strings.Format(Conversions.ToDate(dr.GetDateTime("TimeEnd")), "MM/dd/yyyy h:mm:ss tt").ToString();
                            itm.TimeElapsed = dr["TimeElapsed"].ToString();
                            //itm.IdleTime = idleTime,
                            itm.Status = (string)dr["Status"].ToString();
                            itm.IsPaused = dr.GetBoolean("IsPaused");
                            itm.PerfStatus = Convert.ToInt32(dr["PerfStatus"]);
                            itm.LOBId = dr.GetInt32("LOBId");
                            itm.LOBItemId = Convert.ToInt32(dr["LOBItemId"]);
                        }

                        itm.PerfConfigData = null;

                        Perflst.Add(itm);

                    }

                    dr.Close();
                }

                catch (Exception ex)
                {
                }

                this.CloseDbConnection();
            }

            if (Perflst is not null)
            {
                if (Perflst.Count > 0)
                {
                    if (this.OpenDbConnection())
                    {
                        for (int i = 0, loopTo = Perflst.Count - 1; i <= loopTo; i++)
                        {
                            this.gridDbCommand.Parameters.Clear();
                            this.gridDbCommand.Parameters.AddWithValue("@PerfId", Perflst[i].Id);
                            this.gridDbCommand.CommandText = "SELECT * FROM [dbo].[tblPerfInfo] WITH (NOLOCK) WHERE PerfId=@PerfId;";

                            try
                            {
                                SqlDataReader dr = this.gridDbCommand.ExecuteReader();

                                if (dr.Read())
                                {
                                    Perflst[i].PerfConfigData = new gridPerfInfo();
                                    {
                                        Perflst[i].PerfConfigData.PerfId = (long)Interaction.IIf(dr.GetInt64("PerfId") is DBNull, Constants.vbNullString, dr.GetInt64("PerfId"));
                                        Perflst[i].PerfConfigData.RF01 = (string)Interaction.IIf(dr.GetString("RF01") is DBNull, Constants.vbNullString, dr.GetString("RF01"));
                                        Perflst[i].PerfConfigData.RF02 = (string)Interaction.IIf(dr.GetString("RF02") is DBNull, Constants.vbNullString, dr.GetString("RF02"));
                                        Perflst[i].PerfConfigData.RF03 = (string)Interaction.IIf(dr.GetString("RF03") is DBNull, Constants.vbNullString, dr.GetString("RF03"));

                                        Perflst[i].PerfConfigData.RF04 = (string)Interaction.IIf(dr.GetString("RF04") is DBNull, Constants.vbNullString, dr.GetString("RF04"));
                                        Perflst[i].PerfConfigData.RF05 = (string)Interaction.IIf(dr.GetString("RF05") is DBNull, Constants.vbNullString, dr.GetString("RF05"));
                                        Perflst[i].PerfConfigData.RF06 = (string)Interaction.IIf(dr.GetString("RF06") is DBNull, Constants.vbNullString, dr.GetString("RF06"));
                                        Perflst[i].PerfConfigData.RF07 = (string)Interaction.IIf(dr.GetString("RF07") is DBNull, Constants.vbNullString, dr.GetString("RF07"));
                                        Perflst[i].PerfConfigData.RF08 = (string)Interaction.IIf(dr.GetString("RF08") is DBNull, Constants.vbNullString, dr.GetString("RF08"));
                                        Perflst[i].PerfConfigData.RF09 = (string)Interaction.IIf(dr.GetString("RF09") is DBNull, Constants.vbNullString, dr.GetString("RF09"));
                                        Perflst[i].PerfConfigData.RF10 = (string)Interaction.IIf(dr.GetString("RF10") is DBNull, Constants.vbNullString, dr.GetString("RF10"));
                                        Perflst[i].PerfConfigData.RF11 = (string)Interaction.IIf(dr.GetString("RF11") is DBNull, Constants.vbNullString, dr.GetString("RF11"));
                                        Perflst[i].PerfConfigData.RF12 = (string)Interaction.IIf(dr.GetString("RF12") is DBNull, Constants.vbNullString, dr.GetString("RF12"));
                                        Perflst[i].PerfConfigData.RF13 = (string)Interaction.IIf(dr.GetString("RF13") is DBNull, Constants.vbNullString, dr.GetString("RF13"));
                                        Perflst[i].PerfConfigData.RF14 = (string)Interaction.IIf(dr.GetString("RF14") is DBNull, Constants.vbNullString, dr.GetString("RF14"));
                                        Perflst[i].PerfConfigData.RF15 = (string)Interaction.IIf(dr.GetString("RF15") is DBNull, Constants.vbNullString, dr.GetString("RF15"));
                                        Perflst[i].PerfConfigData.RF16 = (string)Interaction.IIf(dr.GetString("RF16") is DBNull, Constants.vbNullString, dr.GetString("RF16"));
                                        Perflst[i].PerfConfigData.RF17 = (string)Interaction.IIf(dr.GetString("RF17") is DBNull, Constants.vbNullString, dr.GetString("RF17"));
                                        Perflst[i].PerfConfigData.RF18 = (string)Interaction.IIf(dr.GetString("RF18") is DBNull, Constants.vbNullString, dr.GetString("RF18"));
                                        Perflst[i].PerfConfigData.RF19 = (string)Interaction.IIf(dr.GetString("RF19") is DBNull, Constants.vbNullString, dr.GetString("RF19"));
                                        Perflst[i].PerfConfigData.RF20 = (string)Interaction.IIf(dr.GetString("RF20") is DBNull, Constants.vbNullString, dr.GetString("RF20"));
                                        Perflst[i].PerfConfigData.RF21 = (string)Interaction.IIf(dr.GetString("RF21") is DBNull, Constants.vbNullString, dr.GetString("RF21"));
                                        Perflst[i].PerfConfigData.RF22 = (string)Interaction.IIf(dr.GetString("RF22") is DBNull, Constants.vbNullString, dr.GetString("RF22"));
                                        Perflst[i].PerfConfigData.RF23 = (string)Interaction.IIf(dr.GetString("RF23") is DBNull, Constants.vbNullString, dr.GetString("RF23"));
                                        Perflst[i].PerfConfigData.RF24 = (string)Interaction.IIf(dr.GetString("RF24") is DBNull, Constants.vbNullString, dr.GetString("RF24"));
                                        Perflst[i].PerfConfigData.RF25 = (string)Interaction.IIf(dr.GetString("RF25") is DBNull, Constants.vbNullString, dr.GetString("RF25"));
                                        Perflst[i].PerfConfigData.RF26 = (string)Interaction.IIf(dr.GetString("RF26") is DBNull, Constants.vbNullString, dr.GetString("RF26"));
                                        Perflst[i].PerfConfigData.RF27 = (string)Interaction.IIf(dr.GetString("RF27") is DBNull, Constants.vbNullString, dr.GetString("RF27"));
                                        Perflst[i].PerfConfigData.RF28 = (string)Interaction.IIf(dr.GetString("RF28") is DBNull, Constants.vbNullString, dr.GetString("RF28"));
                                        Perflst[i].PerfConfigData.RF29 = (string)Interaction.IIf(dr.GetString("RF29") is DBNull, Constants.vbNullString, dr.GetString("RF29"));
                                        Perflst[i].PerfConfigData.RF30 = (string)Interaction.IIf(dr.GetString("RF30") is DBNull, Constants.vbNullString, dr.GetString("RF30"));
                                        Perflst[i].PerfConfigData.RF31 = (string)Interaction.IIf(dr.GetString("RF31") is DBNull, Constants.vbNullString, dr.GetString("RF31"));
                                        Perflst[i].PerfConfigData.RF32 = (string)Interaction.IIf(dr.GetString("RF32") is DBNull, Constants.vbNullString, dr.GetString("RF32"));
                                        Perflst[i].PerfConfigData.RF33 = (string)Interaction.IIf(dr.GetString("RF33") is DBNull, Constants.vbNullString, dr.GetString("RF33"));
                                        Perflst[i].PerfConfigData.RF34 = (string)Interaction.IIf(dr.GetString("RF34") is DBNull, Constants.vbNullString, dr.GetString("RF34"));
                                        Perflst[i].PerfConfigData.RF35 = (string)Interaction.IIf(dr.GetString("RF35") is DBNull, Constants.vbNullString, dr.GetString("RF35"));
                                        Perflst[i].PerfConfigData.RF36 = (string)Interaction.IIf(dr.GetString("RF36") is DBNull, Constants.vbNullString, dr.GetString("RF36"));
                                        Perflst[i].PerfConfigData.RF37 = (string)Interaction.IIf(dr.GetString("RF37") is DBNull, Constants.vbNullString, dr.GetString("RF37"));
                                        Perflst[i].PerfConfigData.RF38 = (string)Interaction.IIf(dr.GetString("RF38") is DBNull, Constants.vbNullString, dr.GetString("RF38"));
                                        Perflst[i].PerfConfigData.RF39 = (string)Interaction.IIf(dr.GetString("RF39") is DBNull, Constants.vbNullString, dr.GetString("RF39"));
                                        Perflst[i].PerfConfigData.RF40 = (string)Interaction.IIf(dr.GetString("RF40") is DBNull, Constants.vbNullString, dr.GetString("RF40"));
                                        Perflst[i].PerfConfigData.RF41 = (string)Interaction.IIf(dr.GetString("RF41") is DBNull, Constants.vbNullString, dr.GetString("RF41"));
                                        Perflst[i].PerfConfigData.RF42 = (string)Interaction.IIf(dr.GetString("RF42") is DBNull, Constants.vbNullString, dr.GetString("RF42"));
                                        Perflst[i].PerfConfigData.RF43 = (string)Interaction.IIf(dr.GetString("RF43") is DBNull, Constants.vbNullString, dr.GetString("RF43"));
                                        Perflst[i].PerfConfigData.RF44 = (string)Interaction.IIf(dr.GetString("RF44") is DBNull, Constants.vbNullString, dr.GetString("RF44"));
                                        Perflst[i].PerfConfigData.RF45 = (string)Interaction.IIf(dr.GetString("RF45") is DBNull, Constants.vbNullString, dr.GetString("RF45"));
                                        Perflst[i].PerfConfigData.RF46 = (string)Interaction.IIf(dr.GetString("RF46") is DBNull, Constants.vbNullString, dr.GetString("RF46"));
                                        Perflst[i].PerfConfigData.RF47 = (string)Interaction.IIf(dr.GetString("RF47") is DBNull, Constants.vbNullString, dr.GetString("RF47"));
                                        Perflst[i].PerfConfigData.RF48 = (string)Interaction.IIf(dr.GetString("RF48") is DBNull, Constants.vbNullString, dr.GetString("RF48"));
                                        Perflst[i].PerfConfigData.RF49 = (string)Interaction.IIf(dr.GetString("RF49") is DBNull, Constants.vbNullString, dr.GetString("RF49"));
                                        Perflst[i].PerfConfigData.RF50 = (string)Interaction.IIf(dr.GetString("RF50") is DBNull, Constants.vbNullString, dr.GetString("RF50"));

                                        UserId = Perflst[i].UserId;
                                        TimeStart = Perflst[i].TimeStart;
                                    };
                                }
                                dr.Close();
                            }
                            catch (Exception ex)
                            {

                            }


                        }
                    }
                }
            }


            return Perflst;




        }

        public bool MUpdatePerfInfoMain(int PerfId, gridPerfInfo oPInfo)
        {

            bool temp = false;

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@PerfId", PerfId);

                this.gridDbCommand.CommandText = "DELETE FROM [tblPerfInfo] WHERE PerfId=@PerfId;";

                try
                {
                    this.gridDbCommand.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                }

                try
                {
                    if (!(oPInfo == null))
                    {

                        this.gridDbCommand.Parameters.Clear();
                        this.gridDbCommand.Parameters.AddWithValue("@PerfId", Interaction.IIf(oPInfo.PerfId.ToString() == Constants.vbNullString, DBNull.Value, oPInfo.PerfId));

                        this.gridDbCommand.Parameters.AddWithValue("@RF01", Interaction.IIf(oPInfo.RF01 == Constants.vbNullString | oPInfo.RF01 == null, DBNull.Value, oPInfo.RF01));
                        this.gridDbCommand.Parameters.AddWithValue("@RF02", Interaction.IIf(oPInfo.RF02 == Constants.vbNullString | oPInfo.RF02 == null, DBNull.Value, oPInfo.RF02));
                        this.gridDbCommand.Parameters.AddWithValue("@RF03", Interaction.IIf(oPInfo.RF03 == Constants.vbNullString | oPInfo.RF03 == null, DBNull.Value, oPInfo.RF03));
                        this.gridDbCommand.Parameters.AddWithValue("@RF04", Interaction.IIf(oPInfo.RF04 == Constants.vbNullString | oPInfo.RF04 == null, DBNull.Value, oPInfo.RF04));
                        this.gridDbCommand.Parameters.AddWithValue("@RF05", Interaction.IIf(oPInfo.RF05 == Constants.vbNullString | oPInfo.RF05 == null, DBNull.Value, oPInfo.RF05));

                        if (oPInfo.Count > 5)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF06", Interaction.IIf(oPInfo.RF06 == Constants.vbNullString | oPInfo.RF06 == null, DBNull.Value, oPInfo.RF06));
                            this.gridDbCommand.Parameters.AddWithValue("@RF07", Interaction.IIf(oPInfo.RF07 == Constants.vbNullString | oPInfo.RF07 == null, DBNull.Value, oPInfo.RF07));
                            this.gridDbCommand.Parameters.AddWithValue("@RF08", Interaction.IIf(oPInfo.RF08 == Constants.vbNullString | oPInfo.RF08 == null, DBNull.Value, oPInfo.RF08));
                            this.gridDbCommand.Parameters.AddWithValue("@RF09", Interaction.IIf(oPInfo.RF09 == Constants.vbNullString | oPInfo.RF09 == null, DBNull.Value, oPInfo.RF09));
                            this.gridDbCommand.Parameters.AddWithValue("@RF10", Interaction.IIf(oPInfo.RF10 == Constants.vbNullString | oPInfo.RF10 == null, DBNull.Value, oPInfo.RF10));
                        }

                        if (oPInfo.Count > 10)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF11", Interaction.IIf(oPInfo.RF11 == Constants.vbNullString | oPInfo.RF11 == null, DBNull.Value, oPInfo.RF11));
                            this.gridDbCommand.Parameters.AddWithValue("@RF12", Interaction.IIf(oPInfo.RF12 == Constants.vbNullString | oPInfo.RF12 == null, DBNull.Value, oPInfo.RF12));
                            this.gridDbCommand.Parameters.AddWithValue("@RF13", Interaction.IIf(oPInfo.RF13 == Constants.vbNullString | oPInfo.RF13 == null, DBNull.Value, oPInfo.RF13));
                            this.gridDbCommand.Parameters.AddWithValue("@RF14", Interaction.IIf(oPInfo.RF14 == Constants.vbNullString | oPInfo.RF14 == null, DBNull.Value, oPInfo.RF14));
                            this.gridDbCommand.Parameters.AddWithValue("@RF15", Interaction.IIf(oPInfo.RF15 == Constants.vbNullString | oPInfo.RF15 == null, DBNull.Value, oPInfo.RF15));
                        }


                        if (oPInfo.Count > 15)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF16", Interaction.IIf(oPInfo.RF16 == Constants.vbNullString | oPInfo.RF16 == null, DBNull.Value, oPInfo.RF16));
                            this.gridDbCommand.Parameters.AddWithValue("@RF17", Interaction.IIf(oPInfo.RF17 == Constants.vbNullString | oPInfo.RF17 == null, DBNull.Value, oPInfo.RF17));
                            this.gridDbCommand.Parameters.AddWithValue("@RF18", Interaction.IIf(oPInfo.RF18 == Constants.vbNullString | oPInfo.RF18 == null, DBNull.Value, oPInfo.RF18));
                            this.gridDbCommand.Parameters.AddWithValue("@RF19", Interaction.IIf(oPInfo.RF19 == Constants.vbNullString | oPInfo.RF19 == null, DBNull.Value, oPInfo.RF19));
                            this.gridDbCommand.Parameters.AddWithValue("@RF20", Interaction.IIf(oPInfo.RF20 == Constants.vbNullString | oPInfo.RF20 == null, DBNull.Value, oPInfo.RF20));
                        }

                        if (oPInfo.Count > 20)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF21", Interaction.IIf(oPInfo.RF21 == Constants.vbNullString | oPInfo.RF21 == null, DBNull.Value, oPInfo.RF21));
                            this.gridDbCommand.Parameters.AddWithValue("@RF22", Interaction.IIf(oPInfo.RF22 == Constants.vbNullString | oPInfo.RF22 == null, DBNull.Value, oPInfo.RF22));
                            this.gridDbCommand.Parameters.AddWithValue("@RF23", Interaction.IIf(oPInfo.RF23 == Constants.vbNullString | oPInfo.RF23 == null, DBNull.Value, oPInfo.RF23));
                            this.gridDbCommand.Parameters.AddWithValue("@RF24", Interaction.IIf(oPInfo.RF24 == Constants.vbNullString | oPInfo.RF24 == null, DBNull.Value, oPInfo.RF24));
                            this.gridDbCommand.Parameters.AddWithValue("@RF25", Interaction.IIf(oPInfo.RF25 == Constants.vbNullString | oPInfo.RF25 == null, DBNull.Value, oPInfo.RF25));
                        }

                        if (oPInfo.Count > 25)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF26", Interaction.IIf(oPInfo.RF26 == Constants.vbNullString | oPInfo.RF26 == null, DBNull.Value, oPInfo.RF26));
                            this.gridDbCommand.Parameters.AddWithValue("@RF27", Interaction.IIf(oPInfo.RF27 == Constants.vbNullString | oPInfo.RF27 == null, DBNull.Value, oPInfo.RF27));
                            this.gridDbCommand.Parameters.AddWithValue("@RF28", Interaction.IIf(oPInfo.RF28 == Constants.vbNullString | oPInfo.RF28 == null, DBNull.Value, oPInfo.RF28));
                            this.gridDbCommand.Parameters.AddWithValue("@RF29", Interaction.IIf(oPInfo.RF29 == Constants.vbNullString | oPInfo.RF29 == null, DBNull.Value, oPInfo.RF29));
                            this.gridDbCommand.Parameters.AddWithValue("@RF30", Interaction.IIf(oPInfo.RF30 == Constants.vbNullString | oPInfo.RF30 == null, DBNull.Value, oPInfo.RF30));
                        }


                        if (oPInfo.Count > 30)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF31", Interaction.IIf(oPInfo.RF31 == Constants.vbNullString | oPInfo.RF31 == null, DBNull.Value, oPInfo.RF31));
                            this.gridDbCommand.Parameters.AddWithValue("@RF32", Interaction.IIf(oPInfo.RF32 == Constants.vbNullString | oPInfo.RF32 == null, DBNull.Value, oPInfo.RF32));
                            this.gridDbCommand.Parameters.AddWithValue("@RF33", Interaction.IIf(oPInfo.RF33 == Constants.vbNullString | oPInfo.RF33 == null, DBNull.Value, oPInfo.RF33));
                            this.gridDbCommand.Parameters.AddWithValue("@RF34", Interaction.IIf(oPInfo.RF34 == Constants.vbNullString | oPInfo.RF34 == null, DBNull.Value, oPInfo.RF34));
                            this.gridDbCommand.Parameters.AddWithValue("@RF35", Interaction.IIf(oPInfo.RF35 == Constants.vbNullString | oPInfo.RF35 == null, DBNull.Value, oPInfo.RF35));

                        }

                        if (oPInfo.Count > 35)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF36", Interaction.IIf(oPInfo.RF36 == Constants.vbNullString | oPInfo.RF36 == null, DBNull.Value, oPInfo.RF36));
                            this.gridDbCommand.Parameters.AddWithValue("@RF37", Interaction.IIf(oPInfo.RF37 == Constants.vbNullString | oPInfo.RF37 == null, DBNull.Value, oPInfo.RF37));
                            this.gridDbCommand.Parameters.AddWithValue("@RF38", Interaction.IIf(oPInfo.RF38 == Constants.vbNullString | oPInfo.RF38 == null, DBNull.Value, oPInfo.RF38));
                            this.gridDbCommand.Parameters.AddWithValue("@RF39", Interaction.IIf(oPInfo.RF39 == Constants.vbNullString | oPInfo.RF39 == null, DBNull.Value, oPInfo.RF39));
                            this.gridDbCommand.Parameters.AddWithValue("@RF40", Interaction.IIf(oPInfo.RF40 == Constants.vbNullString | oPInfo.RF40 == null, DBNull.Value, oPInfo.RF40));
                        }


                        if (oPInfo.Count > 40)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF41", Interaction.IIf(oPInfo.RF41 == Constants.vbNullString | oPInfo.RF41 == null, DBNull.Value, oPInfo.RF41));
                            this.gridDbCommand.Parameters.AddWithValue("@RF42", Interaction.IIf(oPInfo.RF42 == Constants.vbNullString | oPInfo.RF42 == null, DBNull.Value, oPInfo.RF42));
                            this.gridDbCommand.Parameters.AddWithValue("@RF43", Interaction.IIf(oPInfo.RF43 == Constants.vbNullString | oPInfo.RF43 == null, DBNull.Value, oPInfo.RF43));
                            this.gridDbCommand.Parameters.AddWithValue("@RF44", Interaction.IIf(oPInfo.RF44 == Constants.vbNullString | oPInfo.RF44 == null, DBNull.Value, oPInfo.RF44));
                            this.gridDbCommand.Parameters.AddWithValue("@RF45", Interaction.IIf(oPInfo.RF45 == Constants.vbNullString | oPInfo.RF45 == null, DBNull.Value, oPInfo.RF45));

                        }

                        if (oPInfo.Count > 45)
                        {
                            this.gridDbCommand.Parameters.AddWithValue("@RF46", Interaction.IIf(oPInfo.RF46 == Constants.vbNullString | oPInfo.RF46 == null, DBNull.Value, oPInfo.RF46));
                            this.gridDbCommand.Parameters.AddWithValue("@RF47", Interaction.IIf(oPInfo.RF47 == Constants.vbNullString | oPInfo.RF47 == null, DBNull.Value, oPInfo.RF47));
                            this.gridDbCommand.Parameters.AddWithValue("@RF48", Interaction.IIf(oPInfo.RF48 == Constants.vbNullString | oPInfo.RF48 == null, DBNull.Value, oPInfo.RF48));
                            this.gridDbCommand.Parameters.AddWithValue("@RF49", Interaction.IIf(oPInfo.RF49 == Constants.vbNullString | oPInfo.RF49 == null, DBNull.Value, oPInfo.RF49));
                            this.gridDbCommand.Parameters.AddWithValue("@RF50", Interaction.IIf(oPInfo.RF50 == Constants.vbNullString | oPInfo.RF50 == null, DBNull.Value, oPInfo.RF50));
                        }


                        if (oPInfo.Count < 6)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05)";
                        }

                        else if (oPInfo.Count > 5 & oPInfo.Count < 11)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10)";
                        }


                        else if (oPInfo.Count > 10 & oPInfo.Count < 16)
                        {
                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15)";
                        }

                        else if (oPInfo.Count > 15 & oPInfo.Count < 21)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20)";
                        }

                        else if (oPInfo.Count > 20 & oPInfo.Count < 26)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25)";
                        }

                        else if (oPInfo.Count > 25 & oPInfo.Count < 31)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25,RF26,RF27,RF28,RF29,RF30) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25,@RF26,@RF27,@RF28,@RF29,@RF30)";
                        }

                        else if (oPInfo.Count > 30 & oPInfo.Count < 36)
                        {
                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25,RF26,RF27,RF28,RF29,RF30,RF31,RF32,RF33,RF34,RF35) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25,@RF26,@RF27,@RF28,@RF29,@RF30,@RF31,@RF32,@RF33,@RF34,@RF35)";
                        }

                        else if (oPInfo.Count > 35 & oPInfo.Count < 41)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25,RF26,RF27,RF28,RF29,RF30,RF31,RF32,RF33,RF34,RF35,RF36,RF37,RF38,RF39,RF40) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25,@RF26,@RF27,@RF28,@RF29,@RF30,@RF31,@RF32,@RF33,@RF34,@RF35,@RF36,@RF37,@RF38,@RF39,@RF40)";
                        }

                        else if (oPInfo.Count > 40 & oPInfo.Count < 46)
                        {
                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25,RF26,RF27,RF28,RF29,RF30,RF31,RF32,RF33,RF34,RF35,RF36,RF37,RF38,RF39,RF40,RF41,RF42,RF43,RF44,RF45) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25,@RF26,@RF27,@RF28,@RF29,@RF30,@RF31,@RF32,@RF33,@RF34,@RF35,@RF36,@RF37,@RF38,@RF39,@RF40,@RF41,@RF42,@RF43,@RF44,@RF45)";
                        }

                        else if (oPInfo.Count > 45)
                        {

                            this.gridDbCommand.CommandText = "INSERT INTO tblPerfInfo " + "(PerfId,RF01,RF02,RF03,RF04,RF05,RF06,RF07,RF08,RF09,RF10,RF11,RF12,RF13,RF14,RF15,RF16,RF17,RF18,RF19,RF20,RF21,RF22,RF23,RF24,RF25,RF26,RF27,RF28,RF29,RF30,RF31,RF32,RF33,RF34,RF35,RF36,RF37,RF38,RF39,RF40,RF41,RF42,RF43,RF44,RF45,RF46,RF47,RF48,RF49,RF50) VALUES " + "(@PerfId,@RF01,@RF02,@RF03,@RF04,@RF05,@RF06,@RF07,@RF08,@RF09,@RF10,@RF11,@RF12,@RF13,@RF14,@RF15,@RF16,@RF17,@RF18,@RF19,@RF20,@RF21,@RF22,@RF23,@RF24,@RF25,@RF26,@RF27,@RF28,@RF29,@RF30,@RF31,@RF32,@RF33,@RF34,@RF35,@RF36,@RF37,@RF38,@RF39,@RF40,@RF41,@RF42,@RF43,@RF44,@RF45,@RF46,@RF47,@RF48,@RF49,@RF50)";


                        }

                        #region Parameters



















                        #endregion



                        try
                        {
                            if (this.gridDbCommand.ExecuteNonQuery() > 0)
                                temp = true;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {

                }

                this.CloseDbConnection();

            }

            return temp;

        }


        public bool MEditPerformanceMain(gridPerformance ObjPerfData)
        {

            bool temp = false;

            string exp = "";

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();

                this.gridDbCommand.Parameters.AddWithValue("@UserId", ObjPerfData.UserId);
                this.gridDbCommand.Parameters.AddWithValue("@TransDate", ObjPerfData.TransDate2.ToShortDateString());
                this.gridDbCommand.Parameters.AddWithValue("@ActivityId", ObjPerfData.ActivityId);
                this.gridDbCommand.Parameters.AddWithValue("@TimeStart", ObjPerfData.TimeStart2.ToString());
                this.gridDbCommand.Parameters.AddWithValue("@TimeEnd", ObjPerfData.TimeEnd2.ToString());
                this.gridDbCommand.Parameters.AddWithValue("@ReferenceId", ObjPerfData.ReferenceId == null ? "" : ObjPerfData.ReferenceId);

                this.gridDbCommand.Parameters.AddWithValue("@TimeElapsed", ObjPerfData.TimeElapsed);
                this.gridDbCommand.Parameters.AddWithValue("@IdleTime", ConvertSecondsToHHMMSS(ObjPerfData.IdleTime < 0 ? 0 : ObjPerfData.IdleTime));

                this.gridDbCommand.Parameters.AddWithValue("@Status", ObjPerfData.Status);
                this.gridDbCommand.Parameters.AddWithValue("@IsPaused", ObjPerfData.IsPaused);
                this.gridDbCommand.Parameters.AddWithValue("@PerfStatus", ObjPerfData.PerfStatus);
                this.gridDbCommand.Parameters.AddWithValue("@LOBId", ObjPerfData.LOBId);
                this.gridDbCommand.Parameters.AddWithValue("@LOBItemId", ObjPerfData.LOBItemId);

                this.gridDbCommand.Parameters.AddWithValue("@Id", ObjPerfData.Id);

                this.gridDbCommand.CommandText = "UPDATE tblPerformance SET " + "UserId=@UserId, TransDate=@TransDate, ActivityId=@ActivityId, TimeStart=@TimeStart, TimeEnd=@TimeEnd, [ReferenceId]=@ReferenceId, " + "TimeElapsed=@TimeElapsed, " + "IdleTime=@IdleTime, Status=@Status, IsPaused=@IsPaused, PerfStatus=@PerfStatus, LOBId=@LOBId, LOBItemId=@LOBItemId WHERE (Id=@Id);";
                try
                {
                    if (this.gridDbCommand.ExecuteNonQuery() > 0)
                        temp = true;
                }

                catch (Exception ex)
                {

                    exp = ex.Message.ToString();

                    this.gridDbCommand.CommandText = "UPDATE tblPerformance SET " + "UserId=@UserId, TransDate=@TransDate, ActivityId=@ActivityId, TimeStart=@TimeStart, TimeEnd=@TimeEnd, [ReferenceId]=@ReferenceId, " + "TimeElapsed=@TimeElapsed, Status=@Status, IsPaused=@IsPaused, PerfStatus=@PerfStatus, LOBId=@LOBId, LOBItemId=@LOBItemId WHERE (Id=@Id);";

                    try
                    {
                        if (this.gridDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }
                    catch (Exception ex2)
                    {
                        exp = exp + " err2: " + ex2.Message.ToString();
                    }

                }

                this.CloseDbConnection();

            }


            if (temp == false)
            {

                string ss;

                ss = "Id:" + ObjPerfData.Id.ToString();
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


                if (this.OpenDbConnection())
                {

                    this.gridDbCommand.Parameters.Clear();

                    this.gridDbCommand.Parameters.AddWithValue("@UserId", ObjPerfData.UserId);
                    this.gridDbCommand.Parameters.AddWithValue("@Date", DateTime.Now);
                    this.gridDbCommand.Parameters.AddWithValue("@Activity", "Update entry in tblPerformance MEditPerformanceMain".ToString());
                    this.gridDbCommand.Parameters.AddWithValue("@Exception", Strings.Len(exp) > 500 ? Strings.Mid(exp, 1, 500) : exp);
                    this.gridDbCommand.Parameters.AddWithValue("@Details", Strings.Len(ss) > 1000 ? Strings.Mid(ss, 1, 1000) : ss);
                    this.gridDbCommand.Parameters.AddWithValue("@Version", this.gridData.GridVersion);
                    this.gridDbCommand.CommandText = "INSERT INTO tblException ([UserId],[Date],[Activity],[Exception],[Details],[Version]) VALUES (@UserId,@Date,@Activity,@Exception,@Details,@Version);";

                    try
                    {
                        this.gridDbCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }

                    this.CloseDbConnection();

                }
            }


            return temp;



        }


        #region "Constructors"
        public string MGetTimeElapsed(DateTime tStart, DateTime tEnd, string tTextbox)
        {

            var timeStart = tStart;
            var timeEnd = tEnd;
            long rangeDiffSeconds = DateAndTime.DateDiff(DateInterval.Second, timeStart, timeEnd);
            if (rangeDiffSeconds >= 86400L | rangeDiffSeconds < 1L)
            {
                return tTextbox;
            }
            else
            {
                var x = new TimeSpan(0, 0, (int)rangeDiffSeconds);
                return x.ToString();

            }

        }

        public string MGetTimeElapsedPaused(DateTime tStart, DateTime tEnd, string tTextbox)
        {

            var timeStart = tStart;
            var timeEnd = tEnd;
            DateTime ElapsedTime = Conversions.ToDate(tTextbox);
            long rangeDiffSeconds = DateAndTime.DateDiff(DateInterval.Second, timeStart, timeEnd) + ElapsedTime.Hour * 3600 + ElapsedTime.Minute * 60 + ElapsedTime.Second - 1L;
            if (rangeDiffSeconds >= 86400L | rangeDiffSeconds < 1L)
            {
                return tTextbox;
            }
            else
            {
                var x = new TimeSpan(0, 0, (int)rangeDiffSeconds);
                return x.ToString();

            }

        }


        #endregion


        public bool UpdateLogOutEveryMin(gridTimeCard ObjTimeCard)
        {

            bool temp = false;

            long ProdOT = 0L;


            if (this.OpenDbTimecardConnection())
            {

                this.gridDbTimecardCommand.Parameters.Clear();
                this.gridDbTimecardCommand.Parameters.AddWithValue("@Id", ObjTimeCard.Id);
                this.gridDbTimecardCommand.Parameters.AddWithValue("@LogOut", ObjTimeCard.LogOut);

                this.gridDbTimecardCommand.CommandText = "UPDATE tblTimeCard SET [LogOut]=@LogOut WHERE [Id]=@Id;";

                try
                {
                    if (this.gridDbTimecardCommand.ExecuteNonQuery() >= 1)
                    {

                        this.grdData.CurrentUser.LogOut = ObjTimeCard.LogOut;

                        temp = true;

                    }
                }

                catch (Exception ex)
                {

                }

                this.CloseDbTimecardConnection();

            }


            return temp;


        }



    }
}
