using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;


namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        //public List<gridActivity> GetFavoriteActivities() // 11-13
        //{
        //    List<gridActivity> lstActivityMnt = new List<gridActivity>();

        //    this.grdMutexx.WaitOne();

        //    if (this.OpenLocalDbConnection())
        //    {
        //        this.gridLocalDbCommand.Parameters.Clear();
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@TeamId", this.grdData.CurrentUser.TeamId);
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@IsPublic", true);
        //        this.gridLocalDbCommand.CommandText = "SELECT * FROM vFAVORITES WHERE UserId=@UserId AND TeamId=@TeamId AND ([IsPublic]=Yes OR [IsPublic]=1);";

        //        try
        //        {
          
        //            var dr = ExecuteReaderLocal();

        //            while (dr.Read())
        //            {
        //                if (dr.GetBoolean("IsPublic"))
        //                {
        //                    gridActivity hActivity = new gridActivity();
        //                    {
        //                        var withBlock = hActivity;
        //                        withBlock.Id = (int)dr["Id"];
        //                        withBlock.TeamId = (int)dr["TeamId"];
        //                        withBlock.ActName = (string)dr["ActName"];
        //                        withBlock.Type = (string)dr["Type"];
        //                        withBlock.AHT = (string)dr["AHT"];
        //                        withBlock.LOBId = (int)dr["LOBId"];
        //                        withBlock.IsPublic = (bool)dr["IsPublic"];
        //                        withBlock.Process = (string)dr["Process"];
        //                        withBlock.ConfigId = (int)dr["ConfigId"];

        //                        withBlock.StandardAct = (bool)dr["StandardAct"];
        //                        withBlock.DailyDashboard = (bool)dr["DailyDashboard"];
        //                        withBlock.DailyScorecard = (bool)dr["DailyScorecard"];
        //                        withBlock.RiskIssue = (bool)dr["RiskIssue"];
        //                    }

        //                    lstActivityMnt.Add(hActivity);
        //                }
        //            }

        //            dr.Close();
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        this.CloseLocalDbConnection();
        //    }

        //    this.grdMutexx.ReleaseMutex();


        //    return lstActivityMnt;
        //}

        //public bool AddActivityToFavorites(gridActivity ObjgridActivity)
        //{

        //    bool temp = false;

        //    this.grdMutexx.WaitOne();

        //    if (this.OpenLocalDbConnection())
        //    {

        //        this.gridLocalDbCommand.Parameters.Clear();
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@Id", ObjgridActivity.Id);
        //        this.gridLocalDbCommand.CommandText = "SELECT * FROM tblFavorites WHERE [UserId]=@UserId AND [Id]=@Id;";

        //        try
        //        {
        //            OleDbDataReader dr = this.ExecuteReaderLocal();
        //            if (dr.Read())
        //                temp = true;
        //            dr.Close();
        //        }
        //        catch (Exception ex)
        //        {

        //        }

        //        if (temp == false)
        //        {

        //            this.gridLocalDbCommand.Parameters.Clear();
        //            this.gridLocalDbCommand.Parameters.AddWithValue("@Id", ObjgridActivity.Id);
        //            this.gridLocalDbCommand.Parameters.AddWithValue("@TeamId", ObjgridActivity.TeamId);
        //            this.gridLocalDbCommand.Parameters.AddWithValue("@UserId", this.grdData.CurrentUser.EmpNo);
        //            this.gridLocalDbCommand.CommandText = "INSERT INTO tblFavorites ([Id], [TeamId], [UserId]) VALUES (@Id, @TeamId, @UserId);";

        //            try
        //            {
        //                if (this.ExecuteNonQueryLocal() > 0)
        //                    temp = true;
        //            }
        //            catch (Exception ex)
        //            {

        //            }

        //        }

        //        this.CloseLocalDbConnection();

        //    }

        //    this.grdMutexx.ReleaseMutex();

        //    return temp;

        //}

        //public bool DeleteActivityFromFavorites(gridActivity ObjgridActivity, int UserId)
        //{

        //    bool temp = false;

        //    this.grdMutexx.WaitOne();

        //    if (this.OpenLocalDbConnection())
        //    {

        //        this.gridLocalDbCommand.Parameters.Clear();
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@Id", ObjgridActivity.Id);
        //        this.gridLocalDbCommand.Parameters.AddWithValue("@UserId", UserId);

        //        this.gridLocalDbCommand.CommandText = "DELETE FROM tblFavorites WHERE [Id]=@Id AND [UserId]=@UserId;";

        //        try
        //        {
        //            if (this.ExecuteNonQueryLocal() > 0)
        //                temp = true;
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        try
        //        {
        //            this.gridLocalDbCommand.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {

        //        }

        //        this.CloseLocalDbConnection();

        //    }

        //    this.grdMutexx.ReleaseMutex();

        //    return temp;


        //}

    }

}
