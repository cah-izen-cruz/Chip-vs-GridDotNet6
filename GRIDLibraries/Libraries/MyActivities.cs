using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public bool AddActivityToFavorites(gridActivity ObjgridActivity)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainDbConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                this.gridMainDbCommand.Parameters.AddWithValue("@ActivityId", ObjgridActivity.Id);
                this.gridMainDbCommand.CommandText = "SELECT * FROM tblMyActivity WHERE [UserId]=@UserId AND [ActivityId]=@ActivityId;";

                try
                {
                    var dr = this.gridMainDbCommand.ExecuteReader();
                    if (dr.Read())
                        temp = true;
                    dr.Close();
                }
                catch (Exception)
                {
                }

                if (temp == false)
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@ActivityId", ObjgridActivity.Id);
                    this.gridMainDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                    this.gridMainDbCommand.CommandText = "INSERT INTO tblMyActivity ([ActivityId], [UserId]) VALUES (@ActivityId, @UserId);";

                    try
                    {
                        if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                this.CloseMainDbConnection();
            }

            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public List<gridActivity> GetFavoriteActivities() // 11-13
        {
            List<gridActivity> lstActivityMnt = new List<gridActivity>();

            this.grdMutexx.WaitOne();

            if (this.OpenMainDbConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
                //this.gridMainDbCommand.CommandText = "SELECT tblActivity.* FROM tblActivity INNER JOIN tblMyActivity ON tblActivity.Id = tblFavorites.Id;"
                this.gridMainDbCommand.CommandText = "SELECT * FROM vMyFavorites WHERE UserId=@UserId AND [IsPublic]=1";

                try
                {

                    var dr = this.gridMainDbCommand.ExecuteReader();

                    while (dr.Read())
                    {
                        if (dr.GetBoolean("IsPublic"))
                        {
                            gridActivity hActivity = new gridActivity();
                            {
                                var withBlock = hActivity;
                                withBlock.Id = (int)dr["Id"];
                                withBlock.TeamId = (int)dr["TeamId"];
                                withBlock.ActName = (string)dr["Name"];
                                withBlock.Type = (string)dr["Type"];
                                withBlock.AHT = (string)dr["AHT"];
                                withBlock.LOBId = (int)dr["LOBId"];
                                withBlock.IsPublic = (bool)dr["IsPublic"];
                                withBlock.Process = (string)dr["Process"];
                                withBlock.ConfigId = Convert.ToInt32(dr["ConfigId"]);
                            }

                            lstActivityMnt.Add(hActivity);
                        }
                    }

                    dr.Close();
                }
                catch (Exception)
                {
                }

                this.CloseMainDbConnection();
            }

            this.grdMutexx.ReleaseMutex();


            return lstActivityMnt;
        }

        public bool DeleteActivityFromFavorites(gridActivity ObjgridActivity, int UserId)
        {

            bool temp = false;

            this.grdMutexx.WaitOne();

            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@ActivityId", ObjgridActivity.Id);
                this.gridMainDbCommand.Parameters.AddWithValue("@UserId", UserId);

                this.gridMainDbCommand.CommandText = "DELETE FROM tblMyActivity WHERE [ActivityId]=@ActivityId AND [UserId]=@UserId;";

                try
                {
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;
                }
                catch (Exception ex)
                {

                }
         

                this.CloseMainDbConnection();

            }

            this.grdMutexx.ReleaseMutex();

            return temp;


        }
    }
}
