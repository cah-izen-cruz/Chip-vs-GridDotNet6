using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using GRIDLibraries.Libraries;
using System.Runtime.ConstrainedExecution;

namespace GRIDLibraries.Libraries
{

    partial class GridLib
    {

        public GridData grdData { get; set; } = GridData.gridDataStore;

        bool Initialized;
        public bool Initialize(string _EID)
        {
            Initialized = false;
            if (CheckMainDbConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@EID", _EID);
                this.gridMainDbCommand.CommandText = "SELECT * FROM tblUserInfo WITH (NOLOCK) WHERE EID=@EID;";
                this.gridMainDbCommand.CommandTimeout = 0;

                SqlDataReader dr;
                dr = this.gridMainDbCommand.ExecuteReader();

                if (dr.Read())
                {
                    grdData.CurrentUser.EmpNo = dr.GetString("EmpNo");
                    grdData.CurrentUser.TeamId = dr.GetInt32("TeamId");
                    grdData.CurrentUser.EID = dr.GetString("EID");
                    grdData.CurrentUser.LastName = dr.GetString("LastName");
                    grdData.CurrentUser.FirstName = dr.GetString("FirstName");
                    grdData.CurrentUser.MiddleName = dr.GetString("MiddleName");
                    grdData.CurrentUser.Role = dr.GetOrdinal("RoleID");
                    grdData.CurrentUser.SupEmpNo = dr.GetString("SupEmpNo");
                    grdData.CurrentUser.SchedTimeIn = dr.GetTimeSpan(dr.GetOrdinal("SchedTimeIn"));
                    grdData.CurrentUser.SchedTimeOut = dr.GetTimeSpan(dr.GetOrdinal("SchedTimeOut"));
                    grdData.CurrentUser.LunchBreak = dr.GetTimeSpan(dr.GetOrdinal("LunchBreak"));
                    grdData.CurrentUser.LogInID = 0;

                    TimeSpan FirstBreak = TimeSpan.FromHours(2);
                    TimeSpan SecondBreak = TimeSpan.FromHours(7);

                    try
                    {
                      
                        TimeSpan ts = grdData.CurrentUser.SchedTimeIn;
                        var FB = ts.Add(FirstBreak);
           
                        //grdData.CurrentUser.FirstBreak = dr.GetTimeSpan(dr.GetOrdinal("FirstBreak")); //11:00:00.0000000
                        grdData.CurrentUser.FirstBreak = FB;

                    }
                    catch (System.IO.IOException)
                    {
                    }

                    try
                    {
                        TimeSpan ts = grdData.CurrentUser.SchedTimeIn;
                        var SB = ts.Add(SecondBreak);
                        grdData.CurrentUser.SecondBreak = SB;
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    grdData.CurrentUser.ActualTagging = false;
                    try
                    {
                        grdData.CurrentUser.ActualTagging = !dr.IsDBNull(dr.GetOrdinal("ActualTagging"));
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    grdData.CurrentUser.Status = dr.GetBoolean("Status");

                    try
                    {
                        grdData.CurrentUser.OnShore = System.Convert.ToBoolean(dr.GetBoolean("OnShore"));
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    this.grdData.CurrentUser = grdData.CurrentUser;

                    Initialized = true;
                    grdData.ScrContent.IsInitialized = true;
                }
                dr.Close();

            }

            if (Initialized)
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@TeamId", this.grdData.CurrentUser.TeamId);
                this.gridMainDbCommand.CommandText = "SELECT * FROM [dbo].[tblTeam] WITH (NOLOCK) WHERE Id=@TeamId;";

                SqlDataReader dr2;
                dr2 = this.gridMainDbCommand.ExecuteReader();

                if (dr2.Read())
                {
                    grdData.TeamInfo.Id = dr2.GetInt32("Id");
                    grdData.TeamInfo.Tower = dr2.GetString("Tower");
                    grdData.TeamInfo.Cluster = dr2.GetString("Department");
                    grdData.TeamInfo.Segment = dr2.GetString("Segment");
                    grdData.TeamInfo.TeamName = dr2.GetString("Function");
                    grdData.TeamInfo.DBName = dr2.GetString("DB");
                    grdData.TeamInfo.OffSet = (int)dr2["OffSet"];
                    grdData.TeamInfo.Server = dr2.GetString("Server");
                    grdData.TeamInfo.GlobalTeamId = 0;
                    grdData.TeamInfo.ProjectId = 0;
                    grdData.TeamInfo.TimeZone = dr2.GetString("TimeZone");
                    grdData.TeamInfo.TowerId = dr2.GetInt32("TowerId");
                    grdData.TeamInfo.DeptId = dr2.GetInt32("DepartmentId");

                    try
                    {
                        grdData.TeamInfo.ProjectId = dr2.GetInt32("TowerId");
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    try
                    {
                        grdData.TeamInfo.GlobalTeamId = dr2.GetInt32("GlobalTeamId");
                    }
                    catch (System.IO.IOException)
                    {
                    }

                    this.grdData.TeamInfo = grdData.TeamInfo;

                    //this.conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=" + grdData.TeamInfo.DBName + "; " + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";
                    this.conString = "Data Source=DESKTOP-A0R75AD;" + "Initial Catalog=" + grdData.TeamInfo.DBName + "; " + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";

                }

                else
                {
                    Initialized = false;
                    grdData.ScrContent.IsInitialized = false;
                    return false;
                }

                dr2.Close();


                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                this.gridMainDbCommand.CommandText = "SELECT [Status] FROM [dbo].[tblWithShifdateOption] WITH (NOLOCK) WHERE UserId=@UserId;";

                SqlDataReader dr3;
                try
                {
                    dr3 = this.gridMainDbCommand.ExecuteReader();
                    if (dr3.Read())
                    {
                        this.grdData.CurrentUser.WithShifdateOption = (bool)dr3["Status"];
                    }
                    dr3.Close();
                }
                catch (Exception ex)
                {
                }


                this.grdData.WFHInfo = new GridWFHinfo();
                this.grdData.WFHInfo.Id = 0;
               
            }

            this.CloseMainDbConnection();

            if (Initialized)
            {
                this.UpdateUserSched(this.grdData.CurrentUser.EmpNo, this.ConvertTimeZone(this.grdData.TeamInfo.OffSet).Date);
            }

  

            if (Initialized)
            {
                this.grdData.CurrentPerfInfo = new gridPerfInfo();

                if (this.grdData.CurrentUser.OnShore == false)
                {
                    if (this.OpenDbConnection())
                    {

                        this.gridDbCommand.Parameters.Clear();
                        this.gridDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                        this.gridDbCommand.CommandText = "SELECT TOP (1) * FROM [dbo].[tblWFHInfo] WITH (NOLOCK) WHERE UserId=@UserId ORDER BY [TransDate] DESC, [Id] DESC;";

                        SqlDataReader dr4;
                        try
                        {
                            dr4 = this.gridDbCommand.ExecuteReader();
                            if (dr4.Read())
                            {
                                this.grdData.WFHInfo.Id = (Int64)dr4["Id"];
                                this.grdData.WFHInfo.UserId =(string)dr4["UserId"];
                                this.grdData.WFHInfo.TransDate = (DateTime)dr4["TransDate"];
                                this.grdData.WFHInfo.IsWFH = (bool)dr4["IsWFH"];
                                this.grdData.WFHInfo.Opt = (int)dr4["Opt"];
                                this.grdData.WFHInfo.Other = (string)dr4["Other"];
                            }
                            dr4.Close();
                        }
                        catch (Exception ex)
                        {
                        }

                        this.CloseDbConnection();
                    }

                }

            }

            grdData.ScrContent.IsInitialized = true;
            return true;
        }

    

    }
        

   
    
}
