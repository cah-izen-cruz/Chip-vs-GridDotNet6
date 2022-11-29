using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public DateTime GetServerDate()
        {
            DateTime GetServerDateRet = default;

            GetServerDateRet = default;

            if (this.OpenMainDbConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.CommandText = "Select GetDate()";

                    GetServerDateRet = (DateTime)this.gridMainDbCommand.ExecuteScalar();

                    this.CloseMainDbConnection();
                }

                catch (Exception ex)
                {
                    Interaction.MsgBox(ex.Message);
                }
            }

            else
            {
                Interaction.MsgBox("Failed to open main database. Please check you connection.");
            }

            return GetServerDateRet;

        }


        public byte[] GetLocalDb()
        {
            byte[] GetLocalDbRet = default;

            GetLocalDbRet = null;


            //string conString = "Data Source=WPPHL039SQL01;" + "Initial Catalog=RPA_GRID_MAIN;" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";
            //string conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=GRID;" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";

            string conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=GRID;" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";


            if (this.OpenDbConnection3(conString))
            {

                this.gridDbCommand3.Parameters.Clear();
                this.gridDbCommand3.Parameters.AddWithValue("@FileName", "local.mdb");
                this.gridDbCommand3.CommandText = "SELECT [FileData] FROM tblInstallation WHERE FileName=@FileName;";

                var dr = this.gridDbCommand3.ExecuteReader();
                if (dr.Read())
                {
                    //GetLocalDbRet = dr.GetInt64("FileData");
                    dr.Close();
                }
                this.CloseDbConnection3();

            }

            return GetLocalDbRet;




        }

        public int GetUserStatus(string EID)
        {

            int temp = 1;


            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@EID", EID);
                this.gridMainDbCommand.CommandText = "Select [Status] FROM [dbo].[tblUserInfo] WHERE [EID]=@EID;";

                try
                {
                    SqlDataReader dr = this.gridMainDbCommand.ExecuteReader();

                    if (dr.Read())
                    {
                        if (dr.GetBoolean("Status") == false)
                        {
                            temp = -1;

                        }
                    }
                    dr.Close();
                }



                catch (Exception ex)
                {

                    temp = 0;
                }

                this.CloseMainDbConnection();


            }

            return temp;
        }


        public bool UserLogIn(gridTimeCard ObjTimeCard)
        {

            bool temp = true;

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@TeamId", ObjTimeCard.TeamId);
                this.gridDbCommand.Parameters.AddWithValue("@UserId", ObjTimeCard.UserId);
                this.gridDbCommand.Parameters.AddWithValue("@TransDate", ObjTimeCard.TransDate);
                this.gridDbCommand.CommandText = "SELECT [Id],[Login],[Remarks] FROM [dbo].[tblTimeCard] WITH (NOLOCK) WHERE [TeamId]=@TeamId AND [UserId]=@UserId AND [TransDate]=@TransDate;";

                try
                {
                    SqlDataReader dr = this.gridDbCommand.ExecuteReader();

                    if (dr.Read())
                    {
                        //dr.GetTimeSpan(dr.GetOrdinal("LunchBreak")); 
                        this.grdData.CurrentUser.ActualLogin = Conversions.ToDate(dr.GetValue("Login"));
                        this.grdData.CurrentUser.IsLate = dr.GetOrdinal("Remarks").ToString() == "Late" ? true : false;
                        this.grdData.CurrentUser.LogIn = Conversions.ToDate(dr.GetValue("Login"));
                        this.grdData.CurrentUser.LogInID = Convert.ToInt32(dr["Id"]);
                    }

                    dr.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.CloseDbConnection();


            }


            if (this.grdData.CurrentUser.LogInID <= 0)
            {

                var ctr = this.TMGetRequiredAgent(ObjTimeCard.TeamId);

                if (this.OpenDbConnection())
                {

                    this.gridDbCommand.Parameters.Clear();

                    this.gridDbCommand.Parameters.AddWithValue("@UserId", ObjTimeCard.UserId);
                    this.gridDbCommand.Parameters.AddWithValue("@TeamId", ObjTimeCard.TeamId);
                    this.gridDbCommand.Parameters.AddWithValue("@TransDate", ObjTimeCard.TransDate);
                    this.gridDbCommand.Parameters.AddWithValue("@LogIn", ObjTimeCard.LogIn);
                    this.gridDbCommand.Parameters.AddWithValue("@LogOut", ObjTimeCard.LogOut);
                    this.gridDbCommand.Parameters.AddWithValue("@Role", ObjTimeCard.Role);
                    this.gridDbCommand.Parameters.AddWithValue("@SupEmpNo", ObjTimeCard.SupEmpNo);

                    this.gridDbCommand.Parameters.AddWithValue("@SchedTimeIn", ObjTimeCard.SchedTimeIn);
                    this.gridDbCommand.Parameters.AddWithValue("@SchedTimeOut", ObjTimeCard.SchedTimeOut);
                    this.gridDbCommand.Parameters.AddWithValue("@Remarks", ObjTimeCard.Remarks);
                    this.gridDbCommand.Parameters.AddWithValue("@HeadCount", ctr);
                    this.gridDbCommand.Parameters.AddWithValue("@OffSet", ObjTimeCard.OffSet);

                    this.gridDbCommand.Parameters.AddWithValue("@ModifiedBy", ObjTimeCard.ModifiedBy);
                    this.gridDbCommand.Parameters.AddWithValue("@DateModified", ObjTimeCard.DateModified);
                    this.gridDbCommand.Parameters.AddWithValue("@Status", true);

                    this.gridDbCommand.CommandText = "INSERT tblTimeCard ([UserId],[TeamId],[TransDate],[LogIn],[LogOut],[Role],[SupEmpNo],[SchedTimeIn],[SchedTimeOut],[Remarks],[HeadCount],[OffSet],[ModifiedBy],[DateModified],[Status]) " + "VALUES (@UserId,@TeamId,@TransDate,@LogIn,@LogOut,@Role,@SupEmpNo,@SchedTimeIn,@SchedTimeOut,@Remarks,@HeadCount,@OffSet,@ModifiedBy,@DateModified,@Status);" + "SELECT SCOPE_IDENTITY()";

                    try
                    {

                        this.grdData.CurrentUser.LogInID = this.gridDbCommand.ExecuteNonQuery();
                    }

                    catch (Exception)
                    {
                    }

                    this.CloseDbConnection();

                }

                if (this.grdData.CurrentUser.LogInID > 0)
                { //DateTime.ParseExact(dr.GetString("Login"), "MM/dd/yyyy h:mm:ss tt", null); Strings.Format(Conversions.ToDate(ObjTimeCard.LogIn), "MM/dd/yyyy  h:mm:ss tt");
                    this.grdData.CurrentUser.LogIn = DateTime.Parse(ObjTimeCard.LogIn.ToString("MM/dd/yyyy  h:mm:ss tt"));
                    this.grdData.CurrentUser.LogOut = DateTime.Parse(ObjTimeCard.LogOut.ToString("MM/dd/yyyy  h:mm:ss tt"));
                    temp = true;
                }
            }


            else if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@Id", this.grdData.CurrentUser.LogInID);
                this.gridDbCommand.Parameters.AddWithValue("@LogOut", ObjTimeCard.LogOut);
                this.gridDbCommand.Parameters.AddWithValue("@Role", ObjTimeCard.Role);

                this.gridDbCommand.CommandText = "UPDATE tblTimeCard SET [LogOut]=@LogOut,[Role]=@Role WHERE [Id]=@Id;";

                if (this.gridDbCommand.ExecuteNonQuery() >= 1)
                {

                    this.grdData.CurrentUser.LogOut = ObjTimeCard.LogOut;

                    this.gridDbCommand.CommandText = "SELECT [Login],[Remarks] FROM [dbo].[tblTimeCard] WITH (NOLOCK) WHERE [Id]=@Id";

                    SqlDataReader dr = this.gridDbCommand.ExecuteReader();

                    if (dr.Read())
                    { 
                        this.grdData.CurrentUser.ActualLogin = Conversions.ToDate(dr.GetValue("Login"));
                        this.grdData.CurrentUser.IsLate = dr.GetOrdinal("Remarks").ToString() == "Late" ? true : false;
                        this.grdData.CurrentUser.LogIn = Conversions.ToDate(dr.GetValue("Login"));

        
                        temp = true;
                    }
                    dr.Close();



                }


                this.CloseDbConnection();


            }

            return temp;

        }

        public bool SetUserLoggedInFlag(string UserId, string HostName, DateTime TransDate)
        {
            bool SetUserLoggedInFlagRet = default;

            SetUserLoggedInFlagRet = false;
            string GrdVersion = "";
            try
            {
                GrdVersion = this.grdData.GridVersion;
            }
            catch (Exception)
            {

            }
            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();

                this.gridMainDbCommand.Parameters.AddWithValue("@EmpNo", UserId);
                this.gridMainDbCommand.Parameters.AddWithValue("@IsCurrentlyLogged", true);
                this.gridMainDbCommand.Parameters.AddWithValue("@ShiftDate", TransDate);
                this.gridMainDbCommand.Parameters.AddWithValue("@HostName", HostName);
                this.gridMainDbCommand.Parameters.AddWithValue("@LogIn", this.ConvertTimeZone(this.grdData.TeamInfo.OffSet));
                this.gridMainDbCommand.Parameters.AddWithValue("@LogOut", DBNull.Value);
                this.gridMainDbCommand.Parameters.AddWithValue("@GridVersion", GrdVersion);
                this.gridMainDbCommand.CommandText = "UPDATE tblUserInfo SET [IsCurrentlyLogged]=@IsCurrentlyLogged,[ShiftDate]=@ShiftDate,[HostName]=@HostName,[GridVersion]=@GridVersion,[LogIn]=@LogIn,[LogOut]=@LogOut WHERE EmpNo=@EmpNo;";

                try
                {
                    if (this.gridMainDbCommand.ExecuteNonQuery() >= 1)
                        SetUserLoggedInFlagRet = true;
                }
                catch (Exception)
                {
                }

                this.CloseMainDbConnection();

            }

            return SetUserLoggedInFlagRet;


        }

        public bool AddLogTrail(string UserId, string _Login, string _Now, string _Action, string _HostName, string _Version, string _UTC)
        {
            bool AddLogTrailRet = default;

            AddLogTrailRet = false;

            if (this.OpenMainDbConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", UserId);
                this.gridMainDbCommand.Parameters.AddWithValue("@LogInfo", _Login);
                this.gridMainDbCommand.Parameters.AddWithValue("@LogAction", _Action);
                this.gridMainDbCommand.Parameters.AddWithValue("@ActualLogInfo", _Now);
                this.gridMainDbCommand.Parameters.AddWithValue("@HostName", _HostName);
                this.gridMainDbCommand.Parameters.AddWithValue("@Version", _Version);
                this.gridMainDbCommand.Parameters.AddWithValue("@UTC", _UTC);

                try
                {
                    this.gridMainDbCommand.CommandText = "INSERT INTO tblLogTrail (UserId, LogInfo, LogAction,ActualLogInfo,HostName,Version,[UTC]) VALUES (@UserId, @LogInfo, @LogAction,@ActualLogInfo,@HostName,@Version,@UTC);";
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                    {
                        AddLogTrailRet = true;
                    }
                }

                catch (Exception)
                {
                }

                this.CloseMainDbConnection();

            }
            return AddLogTrailRet;

        }

        public bool IsMoraleExist(int _TeamId, string _UserId, DateTime _TransDate)
        {
            bool IsMoraleExistRet = default;

            IsMoraleExistRet = false;

            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();

                this.gridMainDbCommand.Parameters.AddWithValue("@TeamId", _TeamId);
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", _UserId);
                this.gridMainDbCommand.Parameters.AddWithValue("@TransDate", _TransDate.Date);

                this.gridMainDbCommand.CommandText = "SELECT * FROM tblMorale WHERE [TeamId]=@TeamId AND [UserId]=@UserId AND [TransDate]=@TransDate;";


                try
                {
                    SqlDataReader dr = this.gridMainDbCommand.ExecuteReader();
                    if (dr.Read())
                        IsMoraleExistRet = true;
                    dr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.CloseMainDbConnection();

            }

            return IsMoraleExistRet;


        }

        public bool UpdateUserSched(string _EmpNo, DateTime _TransDate)
        {
            bool UpdateUserSchedRet = default;

            bool temp = false;

            UpdateUserSchedRet = false;

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();

                this.gridDbCommand.Parameters.AddWithValue("@TransDate", _TransDate.ToShortDateString());
                this.gridDbCommand.Parameters.AddWithValue("@UserId", _EmpNo);
                this.gridDbCommand.CommandText = "SELECT * FROM tblSchedule WITH (NOLOCK) WHERE UserId=@UserId AND " + "(@TransDate BETWEEN DateStart AND DateEnd)";

                SqlDataReader dr = this.gridDbCommand.ExecuteReader();

                if (dr.Read())
                {
                    this.grdData.CurrentUser.SchedTimeIn = (TimeSpan)dr["SchedTimeIn"];
                    this.grdData.CurrentUser.SchedTimeOut = (TimeSpan)dr["SchedTimeOut"];
                    this.grdData.CurrentUser.LunchBreak = (TimeSpan)dr["LunchBreak"];

                    try
                    {
                        this.grdData.CurrentUser.FirstBreak = (TimeSpan)dr["FirstBreak"];
                    }

                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        this.gridData.CurrentUser.SecondBreak = (TimeSpan)dr["SecondBreak"];
                    }
                    catch (Exception ex)
                    {

                    }




                    temp = true;
                }
                dr.Close();

                this.CloseDbConnection();

            }



            if (temp == true)
            {

                temp = false;

                if (this.OpenMainDbConnection())
                {

                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@SchedTimeIn", this.grdData.CurrentUser.SchedTimeIn);
                    this.gridMainDbCommand.Parameters.AddWithValue("@SchedTimeOut", this.grdData.CurrentUser.SchedTimeOut);
                    this.gridMainDbCommand.Parameters.AddWithValue("@LunchBreak", this.grdData.CurrentUser.LunchBreak);
                    this.gridMainDbCommand.Parameters.AddWithValue("@FirstBreak", this.grdData.CurrentUser.FirstBreak);
                    this.gridMainDbCommand.Parameters.AddWithValue("@SecondBreak", this.grdData.CurrentUser.SecondBreak);
                    this.gridMainDbCommand.Parameters.AddWithValue("@EmpNo", this.grdData.CurrentUser.EmpNo);


                    this.gridMainDbCommand.CommandText = "UPDATE tblUserInfo SET SchedTimeIn=@SchedTimeIn,SchedTimeOut=@SchedTimeOut,LunchBreak=@LunchBreak,[FirstBreak]=@FirstBreak,[SecondBreak]=@SecondBreak WHERE EmpNo=@EmpNo;";


                    try
                    {

                        if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }

                    catch (Exception ex)
                    {

                        this.gridMainDbCommand.CommandText = "UPDATE tblUserInfo SET SchedTimeIn=@SchedTimeIn,SchedTimeOut=@SchedTimeOut,LunchBreak=@LunchBreak WHERE EmpNo=@EmpNo;";

                        try
                        {
                            if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                                temp = true;
                        }
                        catch (Exception ex2)
                        {

                        }

                    }




                    this.CloseMainDbConnection();

                }

            }



            return temp;


        }


    }
}
